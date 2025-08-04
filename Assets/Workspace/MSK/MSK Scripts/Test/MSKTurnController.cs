using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] float turnLimit = 30f;
    [Header("아이템 생성기")]
    [SerializeField] ItemSpawner itemSpawner;
    [Header("사이클 종료시 생성할 아이템의 개수")]
    [SerializeField] int itemCount;
    [Header("게임 종료 판넬")]
    [SerializeField] ResultUI ResultPanel;
    [Header("탱크 리스트 확인용")]
    [SerializeField] List<PlayerController> tanks = new();
    [SerializeField] GameObject Arrow;
    private Vector3 arrowOffset = new Vector3(-0.3f, 3f, 0);
    private GameObject curArrow;

    private Queue<PlayerInfo> turnQueue = new();
    private List<PlayerInfo> nextCycle = new();
    private HashSet<int> DeadPlayer = new();

    private int blueRemain = 0;
    private int redRemain = 0;
    private int spawnedCount = 0;

    private Dictionary<PlayerController, Fire> fireMap = new();
    private Dictionary<int, PlayerInfo> allPlayers = new Dictionary<int, PlayerInfo>();
    private PlayerInfo currentPlayer;
    private Room room;
    IEnumerable<PlayerInfo> players;

    private float turnTimer = 0f;
    private bool isTurnRunning = false;
    private bool isGameStart = false;


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
        /*
        if (!isGameStart)
            return;

        if (!PhotonNetwork.IsMasterClient || !isTurnRunning) return;

        turnTimer += Time.deltaTime;

        if (turnTimer >= turnLimit)
        {
            photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }*/
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
        players = allPlayers.Values;

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
        QueueAdd(players);
        StartNextTurn();
    }

    private void GameEndCheck()
    {
        if (blueRemain <= 0 || redRemain <= 0)
        {
            Team winnerTeam = blueRemain == 0 ? Team.Blue : Team.Red;
            Debug.Log($"게임 종료!\n {(winnerTeam == Team.Red ? "레드" : "블루")}팀의 승리");
            photonView.RPC("RPC_GameEnded", RpcTarget.All, winnerTeam);
            return;
        }
        Debug.Log($"GameEndCheck : 블루팀 : {blueRemain} , 레드팀 : {redRemain}");
    }

    private void StartNextTurn()
    {

        GameEndCheck();
        if (turnQueue.Count <= 0)
        {
            photonView.RPC("RPC_CycleEnd", RpcTarget.MasterClient);
            QueueAdd(players);
        }

        currentPlayer = turnQueue.Dequeue();

        if (DeadPlayer.Contains(currentPlayer.ActorNumber))
        {
            Debug.Log($"{currentPlayer}는 사망하여 다음턴으로 이동");
            StartNextTurn();
            return;
        }

        turnTimer = 0f;
        isTurnRunning = true;
        photonView.RPC("TurnNotice", RpcTarget.All, currentPlayer.ActorNumber);
        photonView.RPC("RPC_SetCameraTarget", RpcTarget.All, currentPlayer.ActorNumber);
        photonView.RPC("StartTurnForPlayer", RpcTarget.All, currentPlayer.ActorNumber);
    }
    private void QueueAdd(IEnumerable<PlayerInfo> players)
    {
        turnQueue.Clear();
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
                playerCon.EnableControl(true);
                playerCon.ResetTurn();
                Debug.Log($"{playerCon.photonView} 턴턴턴턴");
            }
            else
            {
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
    private Player ReturnMVP(Team WinnerTeam)
    {
        Player mvp = null;
        float mvpScore = float.MinValue;
        foreach (PlayerInfo p in allPlayers.Values)
        {
            if (p.player.GetTeam() != WinnerTeam) continue;
            float playerScore = 0;
            playerScore += Mathf.Min(p.damageDealt * 0.01f, 50) + p.KillCount * 5;
            if (DeadPlayer != null && DeadPlayer.Contains(p.ActorNumber)) playerScore -= 10;

            if (playerScore > mvpScore)
            {
                mvpScore = playerScore;
                mvp = p.player;
                continue;
            }
            if (Mathf.Approximately(playerScore, mvpScore))
            {
                if (Random.value < 0.5f)
                {
                    mvp = p.player;
                }
            }
        }
        return mvp;
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
        StartNextTurn();
    }

    // TODO : 추후 UI, FireBase와 연결
    [PunRPC]
    private void RPC_GameEnded(Team winnerTeam)
    {
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
        Player MVPPlayer = ReturnMVP(winnerTeam);
        ResultPanel.photonView.RPC("UpdateResult", RpcTarget.All, winnerTeam, MVPPlayer.ActorNumber);
    }

    // 이부분 실제로 RPC 받는지 확인
    [PunRPC]
    public void RPC_PlayerDead(int actorNumber)
    {
        var tank = tanks.Find(t => t.photonView.Owner.ActorNumber == actorNumber);
        if (tank != null)
        {
            tank.photonView.RPC("RPC_PCDead", RpcTarget.All);
            OnPlayerDied(tank);
            tanks.Remove(tank);
        }
        GameEndCheck();

    }

    [PunRPC]
    private void StartTurnForPlayer(int actorNumber)
    {

        // 현재 턴 대상 강제 지정
        if (allPlayers.TryGetValue(actorNumber, out var info))
        {
            currentPlayer = info;
        }
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            Manager.UI.PopUpUI.Show($"{currentPlayer.player.NickName}님의 턴입니다.", Color.green);
            EnableCurrentPlayer();
            SpawnArrowCurrentPlayer();
        }
    }

    [PunRPC]
    private void RPC_InitTank(int actorNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            GetLocalPlayerFire().InitBuff();
    }
    [PunRPC]
    private void RPC_RecordDamage(int damage)
    {
        currentPlayer.ToDealDamage(damage);
        Debug.Log($"{currentPlayer.NickName}가 {damage}의 데미지를 가함!");
    }
    #endregion

    #region MSK added

    [PunRPC]
    private void TurnNotice(int actorNumber)
    {
        //TODO : 턴 시작 공지 부분으로 UI와 연결하는 작업이 필요합니다.
        foreach (var player in tanks)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.Owner != null && view.Owner.ActorNumber == actorNumber)
            {
                Debug.Log($"[TurnNotice] 턴: {view.Owner.ActorNumber}");
                break;
            }
        }
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

        currentPlayer.RecordKillCount();
        DeadPlayer.Add(player.myInfo.ActorNumber);
        Debug.Log($"[MSKTurn] 팀 {team} 남은 인원: {(team == Team.Red ? redRemain : blueRemain)}");

        tanks.Remove(player);
        tanks.RemoveAll(t => t == null);
    }

    public void TurnFinished()
    {
        photonView.RPC("RPC_TurnFinished", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
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
            tank.OnPlayerDied = null;
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
