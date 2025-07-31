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


    [SerializeField] ResultUI ResultPanel;

    private Queue<PlayerInfo> turnQueue = new();
    private List<PlayerInfo> nextCycle = new();
    private int blueRemain = 0;
    private int redRemain = 0;
    [SerializeField] List<PlayerController> tanks = new();
    private Dictionary<PlayerController, Fire> fireMap = new();
    private UnityEvent OnGameEnded;
    private PlayerInfo currentPlayer;
    private Room room;
    private float turnTimer = 0f;
    private bool isTurnRunning = false;

    private bool isGameStart = false;
    private int spawnedCount = 0;
    private Dictionary<int, PlayerInfo> allPlayers = new Dictionary<int, PlayerInfo>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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
            {
                fireMap[controller] = fire;
            }
            else
            {
                Debug.LogError("Fire 를 찾을 수 없습니다.");
            }
            // turn 순서를 셔플할지 여부 결정
            IEnumerable<PlayerInfo> players = allPlayers.Values;

            if (room == null)
            {
                Debug.Log("room = null 입니다!!!");
            }

            if (room.GetTurnRandom())
            {
                players = players.OrderBy(_ => Random.value); // 셔플
            }

            foreach (var playerInfo in PhotonNetwork.PlayerList)
            {
                // 카운트는 한 번만 초기화하고 재계산
                var team = playerInfo.GetTeam();

              
                if (team == Team.Red) 
                    redRemain++;
                else 
                    blueRemain++;
            }
            Debug.Log($"[QueueAdd] turnQueue 갱신 완료: redRemain={redRemain}, blueRemain={blueRemain}");
            PhotonView view = controller.GetComponent<PhotonView>();
            string owner = view != null && view.Owner != null ? view.Owner.NickName : "null";

        }
        InitializePlayerEvents();
        QueueAdd();
        isGameStart = true;
        StartNextTurn();
    }

    public void StartNextTurn()
    {
        if (blueRemain <= 0 || redRemain <= 0)
        {
            Team winnerTeam = blueRemain == 0 ? Team.Blue : Team.Red;
            Debug.Log($"게임 종료!\n {(winnerTeam == Team.Red ? "레드" : "블루")}팀의 승리");
            photonView.RPC("RPC_GameEnded", RpcTarget.All, winnerTeam);
            return;
        }

        if (turnQueue.Count <= 0)
        {
            Debug.Log("새로운 턴이 시작됩니다.");
            photonView.RPC("RPC_CycleEnd", RpcTarget.MasterClient);
            turnQueue.Clear();
            QueueAdd();
        }

        currentPlayer = turnQueue.Dequeue();

        if (GetPlayerController(currentPlayer.player.ActorNumber)._isDead)
        {
            StartNextTurn();
            return;
        }

        turnTimer = 0f;
        isTurnRunning = true;

        Debug.Log("이 아래 다음 턴 있다.");

        //  카메라 추적 추가부분
        photonView.RPC("RPC_SetCameraTarget", RpcTarget.All, currentPlayer.ActorNumber);
        photonView.RPC("StartTurnForPlayer", RpcTarget.All, currentPlayer.ActorNumber);
    }
    private void QueueAdd()
    {
        turnQueue.Clear(); // 항상 새로 구성

        // turn 순서를 셔플할지 여부 결정
        IEnumerable<PlayerInfo> players = allPlayers.Values;
        foreach (var playerInfo in players)
        {
            turnQueue.Enqueue(playerInfo);
        }

        Debug.Log($"[QueueAdd] turnQueue 갱신 완료: {turnQueue.Count}명 / redRemain={redRemain}, blueRemain={blueRemain}");
    }
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
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("RPC_TurnFinished는 MasterClient에서만 처리됩니다.");
            return;
        }

        Debug.Log($"RPC_TurnFinished 호출됨 by MasterClient. currentPlayer: {currentPlayer?.ActorNumber}, actorNumber: {actorNumber}");
        isTurnRunning = false;

        photonView.RPC("RPC_InitTank", RpcTarget.All, currentPlayer.ActorNumber);

        // nextCycle에 플레이어가 남아 있으면 turnQueue에 추가
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
            {
                winners.Add(player);

            }
            else
            {
                losers.Add(player);
            }
        }
        ResultPanel.UpdateResult(winnerTeam);
    }

    // 이부분 실제로 RPC 받는지 확인
    [PunRPC]
    public void RPC_PlayerDead(int actorNumber)
    {
        var tank = tanks.Find(t => t.photonView.Owner.ActorNumber == actorNumber);
        var team = CustomProperty.GetTeam(tank.myInfo.player);
        tank.OnPlayerDied = null;

        if (team == Team.Red)
        {
            Debug.Log("레드팀 감소");
            redRemain--;
        }
        else
        {
            Debug.Log("블루팀 감소");
            blueRemain--;
        }
        if (tank != null)
        {
            OnPlayerDied(tank);
            tanks.Remove(tank);
        }
    }

    [PunRPC]
    void StartTurnForPlayer(int actorNumber)
    {
        Debug.Log($"[StartTurnForPlayer] 내 ActorNumber: {PhotonNetwork.LocalPlayer.ActorNumber}, 턴 대상: {actorNumber}");

        // 현재 턴 대상 강제 지정
        if (allPlayers.TryGetValue(actorNumber, out var info))
        {
            currentPlayer = info;
        }

        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            EnableCurrentPlayer();
        }
    }


    void EnableCurrentPlayer()
    {
        foreach (var playerCon in tanks)
        {
            if (playerCon.photonView.IsMine)
            {
                Debug.Log($"[EnableCurrentPlayer] 내 탱크: {(IsMyTurn() ? "조작 가능" : "조작 불가")}");
            }
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

    /*
     * void EnableCurrentPlayer()
    {
        PlayerController currentTank = GetPlayerController(currentPlayer.player);
        if (currentTank == null)
        {
            Debug.LogError("EnableCurrentPlayer: 현재 턴 플레이어의 컨트롤러가 null입니다.");
            return;
        }
        foreach (PlayerController player in tanks)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view == null || view.Owner == null) continue;

            if (player == currentTank)
            {
                player.ResetTurn();
            }
            else
            {
                player.EndPlayerTurn();
            }
        }
    }
     
*/
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

    [PunRPC]
    void RPC_InitTank(int actorNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
            GetLocalPlayerFire().InitBuff();
    }

    #region MSK added
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
        photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    void RPC_SetCameraTarget(int actorNumber)
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
    void RPC_SetBulletTarget(int bulletViewID)
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

    [PunRPC]
    public void RPC_Spawned()
    {
        foreach (var t in tanks)
        {
            var view = t.GetComponent<PhotonView>();
        }

        spawnedCount++;
        if (spawnedCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("모든 플레이어 탱크 생성 완료, 게임 시작");
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
                photonView.RPC("RPC_PlayerDead", RpcTarget.All, tank.photonView.Owner.ActorNumber);
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

        Debug.Log($"[IsMyTurn] LocalActor: {localActor}, CurrentTurnActor: {currentActor}");

        return currentActor == localActor;
    }
    #endregion
}
