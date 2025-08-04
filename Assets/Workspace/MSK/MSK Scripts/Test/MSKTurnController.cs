using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;


public class MSKTurnController : MonoBehaviourPunCallbacks
{
    // 배틀 씬 내부에서 전역 접근 설계
    public static MSKTurnController Instance { get; private set; }

    [Header("보상")]
    [SerializeField] int winnerTeamReward = 100;
    [SerializeField] int loserTeamReward = 50;
    [Header("턴 제한")]
    [SerializeField] float turnLimit = 5f;
    [Header("아이템 생성기")]
    [SerializeField] ItemSpawner itemSpawner;
    [Header("사이클 종료시 생성할 아이템의 개수")]
    [SerializeField] int itemCount;
    [Header("UI 관련")]
    [SerializeField] ResultUI ResultPanel;
    [SerializeField] TextMeshProUGUI CountText;

    [Header("탱크 리스트 확인용")]
    [SerializeField] List<PlayerController> tanks = new();
    [SerializeField] GameObject Arrow;

    private Vector3 arrowOffset = new Vector3(-0.3f, 3f, 0);
    private GameObject curArrow;

    private Queue<PlayerInfo> turnQueue = new();
    private List<PlayerInfo> nextCycle = new();

    private int blueRemain = 0;
    private int redRemain = 0;
    private int spawnedCount = 0;

    private Dictionary<PlayerController, Fire> fireMap = new();
    private Dictionary<int, PlayerInfo> allPlayers = new Dictionary<int, PlayerInfo>();
    private PlayerInfo currentPlayer;
    private Room room;

    private float turnTimer = 0f;
    private bool isTurnRunning = false;
    private bool isGameStart = false;
    private bool isGameEnd = false;
    private UnityEvent OnGameEnded;

    #region Unity LifeCycle
    private void Awake()
    {
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

            // 모든 클라이언트에 남은 시간 텍스트 갱신
            photonView.RPC("RPC_UpdateTimerText", RpcTarget.All, turnTimer);

            if (turnTimer <= 0f)
            {
                photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }
    }
    #endregion


    public void GameStart()
    {
        turnQueue.Clear();
        nextCycle.Clear();
        tanks.Clear();
        fireMap.Clear();
        room = PhotonNetwork.CurrentRoom;

        foreach (var controller in FindObjectsOfType<PlayerController>())
        {
            tanks.Add(controller);
            Fire fire = controller.GetComponent<Fire>();
            if (fire != null)
                fireMap[controller] = fire;
            PhotonView view = controller.GetComponent<PhotonView>();
            string owner = view != null && view.Owner != null ? view.Owner.NickName : "null";
        }
        IEnumerable<PlayerInfo> players = allPlayers.Values;

        if (room.GetTurnRandom())
            players = players.OrderBy(_ => Random.value);

        foreach (var playerInfo in PhotonNetwork.PlayerList)
        {
            var team = playerInfo.GetTeam();

            if (team == Team.Red)
                redRemain++;
            else
                blueRemain++;
        }
        Debug.Log($"[QueueAdd] turnQueue 갱신 완료: redRemain={redRemain}, blueRemain={blueRemain}");

        isGameStart = true;
        InitializePlayerEvents();
        QueueAdd();
        StartNextTurn();
    }

    private void StartNextTurn()
    {
        Debug.Log("[StartNextTurn] : 턴 진행 시작");
        if (turnQueue.Count <= 0)
        {
            photonView.RPC("RPC_CycleEnd", RpcTarget.MasterClient);
            QueueAdd();
            nextCycle.Clear();
        }

        currentPlayer = turnQueue.Dequeue();

        if (GetPlayerController(currentPlayer.player.ActorNumber)._isDead)
        {
            StartNextTurn();
            return;
        }

        turnTimer = turnLimit;
        isTurnRunning = true;
        photonView.RPC("RPC_SetCameraTarget", RpcTarget.All, currentPlayer.ActorNumber);
        photonView.RPC("StartTurnForPlayer", RpcTarget.All, currentPlayer.ActorNumber);
    }
    private void QueueAdd()
    {
        turnQueue.Clear();
        IEnumerable<PlayerInfo> players = allPlayers.Values;
        foreach (var playerInfo in players)
        {
            turnQueue.Enqueue(playerInfo);
        }

        Debug.Log($"[QueueAdd] turnQueue 갱신 완료: {turnQueue.Count}명 / redRemain={redRemain}, blueRemain={blueRemain}");
    }

    private void EnableCurrentPlayer()
    {
        foreach (var playerCon in tanks)
        {
            if (playerCon == null) continue;
            if (IsMyTurn() && playerCon.photonView.IsMine)
            {
                Debug.Log($"[EnableCurrentPlayer] : {photonView.Owner.NickName} 통제 가능 호출됨");
                playerCon.EnableControl(true);
                playerCon.ResetTurn();
            }
            else
            {
                Debug.Log($"[EnableCurrentPlayer] : {photonView.Owner.NickName} 통제 박탈 호출됨");
                playerCon.EnableControl(false);
                playerCon.EndPlayerTurn();
            }
        }
    }

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

    #region PunRPC
    // TODO: 추후 아이템 생성 등과 연결
    [PunRPC]
    private void RPC_CycleEnd()
    {
        for (int i = 0; i < itemCount; i++)
        {
            itemSpawner.SpawnRandomItem();
        }
    }

    [PunRPC]
    private void RPC_TurnFinished(int actorNumber)
    {
        Debug.Log("[RPC_TurnFinished] : 작업 진행 ");
        isTurnRunning = false;
        photonView.RPC("RPC_InitTank", RpcTarget.All, currentPlayer.ActorNumber);
        if (curArrow != null)
            Destroy(curArrow);

        if (turnQueue.Count == 0 && nextCycle.Count > 0)
        {
            foreach (var player in nextCycle)
                turnQueue.Enqueue(player);
            nextCycle.Clear();
        }

        if (tanks.Count <= 0)
            return;

        StartNextTurn();
    }

    // TODO : 추후 UI, FireBase와 연결
    [PunRPC]
    private void RPC_GameEnded(Team winnerTeam)
    {
        if (!PhotonNetwork.IsMasterClient && isGameEnd)
            return;

        isGameEnd = true;
        Debug.Log("게임 종료!");
        Team myTeam = CustomProperty.GetTeam(PhotonNetwork.LocalPlayer);
        PlayerData data = Manager.Data.PlayerData;
        int reward;
        if (myTeam == winnerTeam)
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
        photonView.RPC("ResultActivate", RpcTarget.All);
        ResultPanel.photonView.RPC("UpdateResult", RpcTarget.All, winnerTeam);
    }

    // 이부분 실제로 RPC 받는지 확인
    [PunRPC]
    public void RPC_PlayerDead(int actorNumber)
    {
        Debug.Log("[RPC_PlayerDead] : 호출됨");
        var tank = tanks.Find(t => t.photonView.Owner.ActorNumber == actorNumber);
        if (tank != null)
        {
            tank.photonView.RPC("RPC_PCDead", RpcTarget.All);
            OnPlayerDied(tank);
            tanks.Remove(tank);
        }
    }

    [PunRPC]
    private void StartTurnForPlayer(int actorNumber)
    {
        Debug.Log("[StartTurnForPlayer] : 호출됨");
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            Manager.UI.PopUpUI.Show($"{currentPlayer.player.NickName}님의 턴입니다.", Color.green);
            SpawnArrowCurrentPlayer();
            EnableCurrentPlayer();
        }
    }

    [PunRPC]
    private void RPC_InitTank(int actorNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            GetLocalPlayerFire().InitBuff();
    }
    #endregion

    #region MSK added
    [PunRPC]
    public void RPC_UpdateTimerText(float remainingTime)
    {
        CountText.text = $"{remainingTime:F0}";
    }
    private void InitializePlayerEvents()
    {
        allPlayers.Clear();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            var info = new PlayerInfo(player);
            allPlayers[player.ActorNumber] = info;
        }
        RegisterPlayerEvents();
    }
    private void OnPlayerDied(PlayerController player)
    {
        Team team = CustomProperty.GetTeam(player.photonView.Owner);
        if (team == Team.Red) redRemain--;
        else blueRemain--;

        Debug.Log($"[MSKTurn] 팀 {team} 남은 인원: {(team == Team.Red ? redRemain : blueRemain)}");
        player.OnPlayerDied = null;
        tanks.Remove(player);
        tanks.RemoveAll(t => t == null);
        if (redRemain <= 0 || blueRemain <= 0)
        {
            Team winner = redRemain <= 0 ? Team.Blue : Team.Red;
            photonView.RPC("RPC_GameEnded", RpcTarget.All, winner);
        }
    }

    public void TurnFinished()
    {
        Debug.Log("[TurnFinished] : 턴 종료 호출됨");
        photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }
    [PunRPC]
    private void ResultActivate()
    {
        ResultPanel.gameObject.SetActive(true);
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
    private void RPC_SetBulletTarget(int bulletViewID)
    {
        PhotonView bulletView = PhotonView.Find(bulletViewID);
        if (bulletView != null)
        {
            CameraController.Instance.vcamBullet.Follow = bulletView.transform;
            CameraController.Instance.vcamBullet.Priority = 20;
        }
    }

    [PunRPC]
    private void RPC_NotifySpawned()
    {
        spawnedCount++;
        Debug.Log($"[MSKTurnController] 플레이어 스폰 완료 수: {spawnedCount}/{PhotonNetwork.CurrentRoom.PlayerCount}");

        if (spawnedCount >= PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("[MSKTurnController] 모든 플레이어가 스폰 완료됨 → GameStart()");
            GameStart();
        }
    }

    //  사망 이벤트 등록용
    private void RegisterPlayerEvents()
    {
        foreach (var tank in tanks)
        {
            tank.OnPlayerDied += () =>
            {
                photonView.RPC("RPC_PlayerDead", RpcTarget.MasterClient, tank.photonView.Owner.ActorNumber);
            };
        }
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
    #endregion
}
