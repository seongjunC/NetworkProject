using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;


public class MSKTurnController : MonoBehaviourPunCallbacks
{
    // 배틀 씬 내부에서 전역 접근 설계
    public static MSKTurnController Instance { get; private set; }

    [Header("보상")]
    [SerializeField] int winnerTeamReward = 100;
    [SerializeField] int loserTeamReward = 50;
    [SerializeField] int MVPTeamReward = 150;
    [Header("턴 제한")]
    [SerializeField] float turnLimit = 15f;
    [Header("아이템 생성기")]
    [SerializeField] ItemSpawner itemSpawner;
    [Header("사이클 종료시 생성할 아이템의 개수")]
    [SerializeField] int itemCount;
    [Header("최대 스폰 아이템 개수")]
    [SerializeField] private int maxSpawnedItems = 10;
    [Header("게임 종료 판넬")]
    [SerializeField] ResultUI ResultPanel;
    [SerializeField] InGameUI inGameUI;
    [SerializeField] TextMeshProUGUI CountText;

    [Header("탱크 리스트 확인용")]
    [SerializeField] List<PlayerController> tanks = new();
    [SerializeField] GameObject Arrow;
    [SerializeField] private TestBattleManager testBattleManager;


    #region private
    // 화살표 관련 설정
    private Vector3 arrowOffset = new Vector3(-0.3f, 3f, 0);
    private GameObject curArrow;

    // 내부 자료 구조
    private Queue<PlayerInfo> turnQueue = new();
    private List<PlayerInfo> nextCycle = new();
    private Dictionary<PlayerController, Fire> fireMap = new();
    private IEnumerable<PlayerInfo> players;
    private HashSet<int> _highlightAcks = new HashSet<int>();
    private LinkedList<GameObject> _spawnedItems = new();


    // 포톤 설정
    private PlayerInfo currentPlayer;
    private Room room;
    // 부울 변수
    private bool isGameStart = false;
    private bool isTurnEnd = false;
    // 내부 변수
    private int spawnedCount = 0;
    private float turnTimer = 15f;

    #endregion
    #region public

    public bool isTurnRunning { get; private set; } = false;
    public bool isGameEnd = false;
    public event System.Action<PlayerController, PlayerInfo> OnPlayerDied;
    public Dictionary<int, PlayerInfo> allPlayers = new Dictionary<int, PlayerInfo>();
    public HashSet<int> DeadPlayer = new();
    #endregion


    #region Unity LifeCycle
    private void Awake()
    {
        Manager.Game.State = State.Game;

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && isTurnRunning && isGameStart && !isGameEnd)
        {
            turnTimer -= Time.deltaTime;
            turnTimer = Mathf.Max(0f, turnTimer);

            // 모든 클라이언트에 남은 시간 텍스트 갱신, 코드 위치 변경 필요...
            photonView.RPC("RPC_UpdateTimerText", RpcTarget.All, turnTimer);

            if (turnTimer <= 0f)
            {
                Debug.Log("시간 제한으로 턴 종료");
                testBattleManager.TestTurnEnd();
            }
        }
    }
    #endregion

    // 초기화 함수
    private void ClearInit()
    {
        turnQueue.Clear();
        nextCycle.Clear();
        tanks.Clear();
        fireMap.Clear();
        room = PhotonNetwork.CurrentRoom;
        allPlayers.Clear();
    }

    private void GameStart()
    {
        ClearInit();
        InitializePlayerEvents();
        Manager.Game.GameStart();

        foreach (var controller in FindObjectsOfType<PlayerController>())
        {
            tanks.Add(controller);
            Fire fire = controller.GetComponent<Fire>();
            if (fire != null)
                fireMap[controller] = fire;
            PhotonView view = controller.GetComponent<PhotonView>();
            string owner = view != null && view.Owner != null ? view.Owner.NickName : "null";
        }
        isGameStart = true;

        if (PhotonNetwork.IsMasterClient)
            SetRandomTurn();
    }

    private void SetRandomTurn()
    {
        room = PhotonNetwork.CurrentRoom;

        //  랜덤 설정
        var actorNumbers = allPlayers.Values
            .OrderBy(_ => room.GetTurnRandom() ? Random.value : 0f)
            .Select(p => p.ActorNumber)
            .ToArray();
        photonView.RPC("RPC_ApplyRandomTurn", RpcTarget.All, actorNumbers);
    }

    private void StartNextTurn()
    {
        // 게임 종료 시 동작 중지
        if (isGameEnd)
            return;

        currentPlayer = turnQueue.Dequeue();
        if (DeadPlayer.Contains(currentPlayer.ActorNumber))
        {
            Debug.Log($"{currentPlayer}는 사망하여 다음턴으로 이동");
            StartNextTurn();
            return;
        }
        isTurnRunning = true;
        isTurnEnd = false;

        turnTimer = turnLimit;

        nextCycle.Add(currentPlayer);

        photonView.RPC("RPC_SetCameraTarget", RpcTarget.All, currentPlayer.ActorNumber);
        photonView.RPC("StartTurnForPlayer", RpcTarget.All, currentPlayer.ActorNumber);

        //턴 시작시 바람
        if (PhotonNetwork.IsMasterClient)
        {
            WindManager.Instance.GenerateNewWind();
        }
    }


    private void QueueAdd(IEnumerable<PlayerInfo> players)
    {
        turnQueue.Clear();
        foreach (var playerInfo in players)
        {
            turnQueue.Enqueue(playerInfo);
        }
        Debug.Log($"[QueueAdd] turnQueue 갱신 완료: {turnQueue.Count}명 / redRemain={testBattleManager.redRemain}, blueRemain={testBattleManager.blueRemain}");
    }
    private void EnableCurrentPlayer()
    {
        foreach (var playerCon in tanks)
        {
            if (playerCon == null) continue;
            if (IsMyTurn() && playerCon.photonView.IsMine)
            {
                playerCon.EnableControl(true);
                playerCon.ResetTurn();
            }
            else
            {
                playerCon.EnableControl(false);
                playerCon.EndPlayerTurn();
            }
        }
    }
    private void SpawnArrowCurrentPlayer()
    {
        if (curArrow != null)
            Destroy(curArrow);
        var pc = GetPlayerController(currentPlayer.ActorNumber);
        if (pc == null)
        {
            Debug.LogError("플레이어 탐색 실패, 화살표 스폰 불가");
            return;
        }

        Vector3 spawnPos = pc.transform.position + arrowOffset;
        curArrow = Instantiate(Arrow, spawnPos, Quaternion.identity);

        var ArrowCon = curArrow.GetComponent<ArrowController>();
        ArrowCon.target = pc.transform;
        ArrowCon.offset = arrowOffset;
    }

    #region IEnumerator
    private IEnumerator WaitForFinish(IEnumerator Func, List<Transform> targets)
    {

        yield return StartCoroutine(Func);

        isTurnRunning = true;
        turnTimer = turnLimit;
    }

    private IEnumerator HighlightThenAck(int[] viewIDs)
    {
        var targets = new List<Transform>();
        foreach (int id in viewIDs)
        {
            var pv = PhotonView.Find(id);
            if (pv != null && pv.transform != null)
                targets.Add(pv.transform);
        }


        yield return StartCoroutine(WaitForFinish(CameraController.Instance.HighlightRoutine(targets, totalDuration: 3f), targets));

        photonView.RPC(nameof(RPC_HighlightAck),
        RpcTarget.MasterClient,
        PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private IEnumerator HandleCycleEnd()
    {
        RPC_TimeStop();
        yield return new WaitForSeconds(1f);

        var dropIDs = new List<int>();
        for (int i = 0; i < itemCount; i++)
        {
            Debug.Log($"아이템 생성 itemCount : {itemCount}, 현재 i : {i}");
            var item = itemSpawner.SpawnRandomItem();
            if (item != null)
            {
                dropIDs.Add(item.GetPhotonView().ViewID);
                _spawnedItems.AddLast(item);

                while (_spawnedItems.Count > maxSpawnedItems)
                {
                    var old = _spawnedItems.First.Value;
                    _spawnedItems.RemoveFirst();
                    if (old != null)
                    {
                        Debug.Log($"{spawnedCount}초과, {old.name} 삭제");
                        Destroy(old);
                    }
                }
            }
        }
        photonView.RPC(
            nameof(RPC_HighlightDroppedItems),
            RpcTarget.All,
            dropIDs.ToArray()
        );
    }
    #endregion

    #region public 
    public PlayerController GetPlayerController(int actorNumber)
    {
        foreach (var tank in tanks)
        {
            if (tank == null) continue;
            PhotonView view = tank.GetComponent<PhotonView>();
            if (view == null || view.Owner == null) continue;
            if (view.Owner.ActorNumber == actorNumber)
            {
                return tank;
            }
        }
        return null;
    }
    public void EndButtonInteractable()
    {
        if (IsMyTurn())
            testBattleManager.SetTurnEndButton(true);
        else
            testBattleManager.SetTurnEndButton(false);
    }
    public PlayerController GetLocalPlayerController()
    {
        return GetPlayerController(PhotonNetwork.LocalPlayer.ActorNumber);
    }
    public Fire GetFireMap(PlayerController controller)
    {
        return fireMap[controller];
    }
    public Fire GetLocalPlayerFire()
    {
        return GetFireMap(GetLocalPlayerController());
    }

    public void TurnFinished()
    {
        Debug.Log($"일반 [TurnFinished]{currentPlayer.player.NickName}님의 턴 종료 ");
        if (curArrow != null)
            Destroy(curArrow);
        photonView.RPC("RPC_TurnFinished", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    public void TurnFinished(int ownerActnum)
    {
        Debug.Log($"공격 [TurnFinished]{currentPlayer.player.NickName}님의 턴 종료 ");
        Debug.Log($"[TurnFinished] : {ownerActnum}");
        if (curArrow != null)
            Destroy(curArrow);
        photonView.RPC("RPC_TurnFinished", RpcTarget.MasterClient, ownerActnum);
    }

    public bool IsMyTurn()
    {
        if (currentPlayer == null)
        {
            Debug.LogWarning("[IsMyTurn] currentPlayer가 null입니다.");
            return false;
        }

        int localActor = PhotonNetwork.LocalPlayer.ActorNumber;
        int currentActor = currentPlayer.ActorNumber;
        return currentActor == localActor;
    }


    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (isGameEnd) return;
        base.OnPlayerLeftRoom(otherPlayer);

        Debug.Log($"플레이어 퇴장 : ActorNumber = {otherPlayer.ActorNumber}");
        var controller = GetPlayerController(otherPlayer.ActorNumber);
        if (controller != null)
        {
            controller.PlayerDead();
        }
        foreach (var tank in tanks)
        {
            if (tank.myInfo.player.ActorNumber == otherPlayer.ActorNumber)
            {
                tanks.Remove(tank);
                allPlayers.Remove(otherPlayer.ActorNumber);
                break;
            }
        }
    }

    public void OnItemDestroyed(GameObject item)
    {
        if (_spawnedItems.Contains(item))
        {
            _spawnedItems.Remove(item);
            Debug.Log($"아이템 삭제, {_spawnedItems.Count}만큼 박스 남음");
        }
        else
        {
            Debug.Log($"소환된 아이템 목록에 {item} 없음");
        }
    }

    #endregion


    #region PunRPC
    //  턴 사이클 시작
    [PunRPC]
    private void RPC_ApplyRandomTurn(int[] orderedActor)
    {
        var orderedList = new List<PlayerInfo>();

        foreach (var actorNumber in orderedActor)
        {
            if (allPlayers.TryGetValue(actorNumber, out var playerInfo))
            {
                orderedList.Add(playerInfo);
            }
        }

        //정렬 후 게임 시작
        players = orderedList;
        QueueAdd(players);
        isGameStart = true;
        turnTimer = turnLimit;
        StartNextTurn();
    }

    [PunRPC]
    public void RPC_UseItem(int actorNumber, int slotIndex)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        var info = allPlayers[actorNumber];
        Debug.Log($"{info.NickName}의 아이템 사용");

        if (actorNumber != currentPlayer.ActorNumber) return;

        info.ItemUse(slotIndex);

        Debug.Log("동기화 호출");

        photonView.RPC(nameof(RPC_SyncUseItem), RpcTarget.All, actorNumber, slotIndex);
    }
    [PunRPC]
    private void RPC_SyncUseItem(int actorNumber, int slotIndex)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            Debug.Log("ClearSlot");
            inGameUI.ClearSlot(slotIndex + 1);
        }
    }
    // TODO: 추후 아이템 생성 등과 연결
    [PunRPC]
    private void RPC_CycleEnd()
    {
        // 혹시 또 호출되면 바로 탈출
        if (!PhotonNetwork.IsMasterClient) return;

        Debug.Log("CycleEnd 호출");
        _highlightAcks.Clear();
        StartCoroutine(HandleCycleEnd());
    }

    [PunRPC]
    private void RPC_HighlightDroppedItems(int[] viewIDs)
    {
        StartCoroutine(HighlightThenAck(viewIDs));

    }


    [PunRPC]
    private void RPC_HighlightAck(int actorNumber)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _highlightAcks.Add(actorNumber);

        if (_highlightAcks.Count >= PhotonNetwork.PlayerList.Length)
        {
            Debug.Log("모두 ACK 완료 → 다음 턴 진행");
            turnTimer = turnLimit;
            StartNextTurn();
        }


    }

    [PunRPC]
    private void RPC_TurnFinished(int actorNumber)
    {
        if (isTurnEnd) return;
        Debug.Log("RPC 턴 종료 호출");
        isTurnEnd = true;
        isTurnRunning = false;
        photonView.RPC("RPC_InitTank", RpcTarget.All, currentPlayer.ActorNumber);
        if (turnQueue.Count == 0 && nextCycle.Count > 0)
        {
            foreach (var player in nextCycle)
                turnQueue.Enqueue(player);
            nextCycle.Clear();
            if (PhotonNetwork.IsMasterClient)
                photonView.RPC("RPC_CycleEnd", RpcTarget.MasterClient);
        }
        else
        {
            turnTimer = turnLimit;
            StartNextTurn();
        }


    }
    [PunRPC]
    public void RPC_GameEnded(Team winnerTeam, int mvpActor)
    {
        if (isGameEnd) return;
        isTurnRunning = false;
        isGameEnd = true;
        Debug.Log("게임 종료!");
        Team myTeam = CustomProperty.GetTeam(PhotonNetwork.LocalPlayer);
        PlayerData data = Manager.Data.PlayerData;
        int reward;

        if (mvpActor == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            reward = MVPTeamReward;
        }
        else if (myTeam == winnerTeam)
        {
            reward = winnerTeamReward;
        }
        else
        {
            reward = loserTeamReward;
        }
        data.GemGain(reward);

        // TODO : UI 교체
        // 게임 오버 UI 출력
        List<Player> winners = new();
        List<Player> losers = new();

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.GetTeam() == winnerTeam)
                winners.Add(player);
            else
                losers.Add(player);
        }

        ResultPanel.UpdateResult(winnerTeam, mvpActor);

    }


    // 이부분 실제로 RPC 받는지 확인
    [PunRPC]
    public void RPC_PlayerDead(int actorNumber)
    {
        if (!DeadPlayer.Contains(actorNumber))
        {
            DeadPlayer.Add(actorNumber);
        }
        var tank = tanks.Find(t => t.photonView.Owner.ActorNumber == actorNumber);
        if (tank != null)
        {
            OnPlayerDied?.Invoke(tank, currentPlayer);
            photonView.RPC(nameof(RPC_RemoveDead), RpcTarget.All, actorNumber);
        }
        else
        {
            testBattleManager.HandlePlayerExit(actorNumber);
        }


    }

    [PunRPC]
    public void RPC_RemoveDead(int actorNumber)
    {
        var tank = tanks.Find(t => t.photonView.Owner.ActorNumber == actorNumber);
        if (tank != null)
        {
            tanks.Remove(tank);
            tank.gameObject.SetActive(false);
        }
    }

    [PunRPC]
    private void StartTurnForPlayer(int actorNumber)
    {

        // 현재 턴 대상 강제 지정
        if (allPlayers.TryGetValue(actorNumber, out var info))
            currentPlayer = info;

        //  턴 종료 버튼 활성화
        EndButtonInteractable();
        Manager.UI.PopUpUI.Show($"{currentPlayer.player.NickName}님의 턴입니다.", Color.green);

        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            EnableCurrentPlayer();
            SpawnArrowCurrentPlayer();
            inGameUI.SetItemButtonInteractable(true);
        }


    }

    [PunRPC]
    private void RPC_InitTank(int actorNumber)
    {
        if (curArrow != null)
            Destroy(curArrow);
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            GetLocalPlayerFire().InitBuff();
    }


    [PunRPC]
    private void RPC_RecordDamage(float damage)
    {
        currentPlayer.ToDealDamage(damage);
        Debug.Log($"{currentPlayer.NickName}가 {damage}의 데미지를 가함!");
    }
    #endregion

    #region MSK added
    private void InitializePlayerEvents()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            var info = new PlayerInfo(player);
            allPlayers[player.ActorNumber] = info;
        }
    }

    [PunRPC]
    public void RPC_UpdateTimerText(float remainingTime)
    {
        if (isGameEnd)
            return;
        CountText.text = $"{remainingTime:F0}";
    }

    [PunRPC]
    private void RPC_SetCameraTarget(int actorNumber)
    {
        foreach (var player in tanks)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.Owner != null && view.Owner.ActorNumber == actorNumber)
            {
                CameraController.Instance.vcamPlayer.Follow = player.transform;
                CameraController.Instance.vcamPlayer.Priority = 20;
                break;
            }
        }
    }
    [PunRPC]
    public void RPC_NotifySpawned()
    {
        spawnedCount++;
        Debug.Log($"[MSKTurnController] 플레이어 스폰 완료 수: {spawnedCount}/{PhotonNetwork.CurrentRoom.PlayerCount}");

        if (spawnedCount >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("[MSKTurnController] 모든 플레이어가 스폰 완료됨 → GameStart()");
            GameStart();
        }
    }
    [PunRPC]
    private void RPC_TimeStop()
    {
        isTurnRunning = false;
        turnTimer = turnLimit;
    }
    #endregion
}