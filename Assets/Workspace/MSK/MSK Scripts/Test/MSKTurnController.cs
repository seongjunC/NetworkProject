using System.Collections.Generic;
using System.Linq;
using Game;
using Photon.Pun;
using Photon.Realtime;
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

    private Queue<PlayerInfo> turnQueue = new();
    private List<PlayerInfo> nextCycle = new();
    private int blueRemain;
    private int redRemain;
    [SerializeField] List<PlayerController> tanks = new();
    private Dictionary<PlayerController, Fire> fireMap = new();
    private UnityEvent OnGameEnded;
    private PlayerInfo currentPlayer;
    private Room room;
    private float turnTimer = 0f;
    private bool isTurnRunning = false;

    private bool isGameStart = false;
    private int spawnedCount = 0;
    private int expectedPlayerCount => PhotonNetwork.CurrentRoom.PlayerCount;

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
            PhotonView view = controller.GetComponent<PhotonView>();
            string owner = view != null && view.Owner != null ? view.Owner.NickName : "null";
        }

        room = PhotonNetwork.CurrentRoom;
        InitializePlayerEvents();
        if (CustomProperty.GetTurnRandom(room))
        {
            List<Player> shuffledPlayerList = PhotonNetwork.PlayerList.ToList();
            for (int i = shuffledPlayerList.Count - 1; i > 0; i--)
            {
                int index = Random.Range(0, i + 1);
                Player temp = shuffledPlayerList[i];
                shuffledPlayerList[i] = shuffledPlayerList[index];
                shuffledPlayerList[index] = temp;
            }
            foreach (var nowPlayer in shuffledPlayerList)
            {
                turnQueue.Enqueue(new PlayerInfo(nowPlayer));
                if (CustomProperty.GetTeam(nowPlayer) == Game.Team.Red) redRemain++;
                else blueRemain++;
            }
        }
        else
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                Debug.Log($"{p.NickName} 큐에 추가됨");
                turnQueue.Enqueue(new PlayerInfo(p));
                if (CustomProperty.GetTeam(p) == Game.Team.Red) redRemain++;
                else blueRemain++;
            }
        }
        isGameStart = true;
        StartNextTurn();
    }

    private void StartNextTurn()
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
            photonView.RPC("RPC_CycleEnd", RpcTarget.MasterClient);
            turnQueue = new Queue<PlayerInfo>(nextCycle);
            nextCycle.Clear();
        }
        currentPlayer = turnQueue.Dequeue();
        Debug.Log($"{currentPlayer.NickName} 큐에서 방출됨");

        if (GetPlayerController(currentPlayer.player)._isDead)
        {
            return;
        }

        turnTimer = 0f;
        isTurnRunning = true;

        Debug.Log("이 아래 다음 턴 있다.");

        //  카메라 추적 추가부분
        photonView.RPC("RPC_SetCameraTarget", RpcTarget.All, currentPlayer.ActorNumber);
        photonView.RPC("StartTurnForPlayer", RpcTarget.All, currentPlayer.ActorNumber);
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
        if (!photonView.IsMine) return;
        Debug.Log("RPC_TurnFinished 호출됨");
        Debug.Log($"currentPlayer : {currentPlayer.ActorNumber}");
        Debug.Log($"actorNumber : {actorNumber}");
        if (currentPlayer != null && currentPlayer.ActorNumber == actorNumber)
        {
            Debug.Log("RPC_TurnFinished 조건문 실행됨");
            isTurnRunning = false;
            photonView.RPC("RPC_InitTank", RpcTarget.All, currentPlayer.ActorNumber);
            nextCycle.Add(currentPlayer);
            StartNextTurn();
        }
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
        // Firebase에 게임 결과를 업로드
    }

    // 이부분 실제로 RPC 받는지 확인
    [PunRPC]
    private void RPC_PlayerDead(int actorNumber)
    {
        var tank = tanks.Find(t => t.photonView.Owner.ActorNumber == actorNumber);
        if (tank != null)
        {
            OnPlayerDied(tank);
            tanks.Remove(tank);
        }
    }

    [PunRPC]
    void StartTurnForPlayer(int actorNumber)
    {
        Debug.Log("StartTurnForPlayer");
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            Debug.Log("턴 시작");
            EnableCurrentPlayer();
        }
    }
    void EnableCurrentPlayer()
    {
        PlayerController playerCon = GetPlayerController(currentPlayer.player);

        Debug.Log("EnableCurrentPlayer");
        // 내 로컬 플레이어만 처리
        if (playerCon != null && playerCon.photonView.IsMine)
        {
            Debug.Log("EnableCurrentPlayer ResetTurn 호출됨");
            playerCon.ResetTurn();
        }
        else
        {
            Debug.Log("EndPlayerTurn 호출됨");
            playerCon?.EndPlayerTurn();
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
    public PlayerController GetPlayerController(Player player)
    {
        foreach (var tank in tanks)
        {
            if (tank == null) continue;
            PhotonView view = tank.GetComponent<PhotonView>();
            if (view == null)
            {
                Debug.LogWarning($"{tank.name} : PhotonView 없음");
                continue;
            }

            if (view.Owner == null)
            {
                Debug.LogWarning($"{tank.name} : PhotonView.Owner가 null임");
                continue;
            }

            Debug.Log($"탐색 중: {tank.name}, Owner.ActorNumber = {view.Owner.ActorNumber}, 찾는 ActorNumber = {player.ActorNumber}");

            if (view.Owner.ActorNumber == player.ActorNumber)
            {
                return tank;
            }
        }
        return null;
    }

    public PlayerController GetLocalPlayerController()
    {
        return GetPlayerController(PhotonNetwork.LocalPlayer);
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
        foreach (var tank in tanks)
        {
            //  중복 이벤트 등록 방지
            tank.OnPlayerDied = null;
            if (!tank.photonView.IsMine)
            {
                tank.OnPlayerDied += () => OnPlayerDied(tank);
            }
        }
    }
    private void OnPlayerDied(PlayerController player)
    {
        Team team = CustomProperty.GetTeam(player.photonView.Owner);
        if (team == Team.Red) redRemain--;
        else blueRemain--;

        Debug.Log($"[MSKTurn] 팀 {team} 남은 인원: {(team == Team.Red ? redRemain : blueRemain)}");
        tanks.RemoveAll(t => t == null);
        tanks.Remove(player);
        if (redRemain <= 0 || blueRemain <= 0)
        {
            Team winner = redRemain <= 0 ? Team.Blue : Team.Red;
            photonView.RPC("RPC_GameEnded", RpcTarget.All, winner);
        }
    }

    public void TurnFinished()
    {
        if (photonView.IsMine)
            photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    void RPC_SetCameraTarget(int actorNumber)
    {
        Debug.Log("플레이어를 따라갑니다.");
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
        Debug.Log("생성된 탄을 따라갑니다.");
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
            Debug.Log($"탱크 탐색: {t.name}, ActorNumber: {view?.Owner?.ActorNumber}");
        }

        spawnedCount++;
        if (spawnedCount == PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log("모든 플레이어 탱크 생성 완료, 게임 시작");
            GameStart();
        }
    }
    public bool IsMyTurn()
    {
        return currentPlayer != null && currentPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber;
    }
    #endregion
}
