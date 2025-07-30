using System.Collections.Generic;
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
    PlayerController[] tanks;
    private UnityEvent OnGameEnded;
    private PlayerInfo currentPlayer;
    private Room room;
    private float turnTimer = 0f;
    private bool isTurnRunning = false;

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
        if (!PhotonNetwork.IsMasterClient || !isTurnRunning) return;

        turnTimer += Time.deltaTime;

        if (turnTimer >= turnLimit)
        {
            photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    public void GameStart()
    {
        turnQueue.Clear();
        nextCycle.Clear();

        tanks = FindObjectsOfType<PlayerController>();

        foreach (var tank in tanks)
        {
            PhotonView view = tank.GetComponent<PhotonView>();
            string owner = view != null && view.Owner != null ? view.Owner.NickName : "null";
        }

        int playerCount = PhotonNetwork.CountOfPlayers;
        room = PhotonNetwork.CurrentRoom;

        if (CustomProperty.GetTurnRandom(room))
        {
            List<int> randNumList = new();
            for (int i = 0; i < playerCount; i++)
            {
                randNumList.Add(i);
            }
            for (int i = playerCount - 1; i > 0; i--)
            {
                int index = Random.Range(0, i + 1);
                int temp = randNumList[i];
                randNumList[i] = randNumList[index];
                randNumList[index] = temp;
            }
            for (int i = 0; i < playerCount; i++)
            {
                Player nowPlayer = PhotonNetwork.PlayerList[randNumList[i]];
                turnQueue.Enqueue(new PlayerInfo(nowPlayer));
                if (CustomProperty.GetTeam(nowPlayer) == Game.Team.Red) redRemain++;
                else blueRemain++;
            }
        }
        else
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                turnQueue.Enqueue(new PlayerInfo(p));
            }
        }

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

        if (currentPlayer.isDead)
        {
            return;
        }
        turnTimer = 0f;
        isTurnRunning = true;

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
        if (currentPlayer != null && currentPlayer.ActorNumber == actorNumber)
        {
            isTurnRunning = false;
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
    private void RPC_PlayerDead()
    {
        currentPlayer.isDead = true;
    }


    [PunRPC]
    void StartTurnForPlayer(int actorNumber)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            Debug.Log("턴 시작");
            EnableCurrentPlayer();
        }
    }

    void EnableCurrentPlayer()
    {
        foreach (PlayerController player in tanks)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.Owner != null)
            {
                if (view.Owner.ActorNumber == currentPlayer.ActorNumber)
                {
                    player.ResetTurn();
                }
                else
                {
                    player.EndPlayerTurn();
                }
            }
        }
    }



    #region MSK added
    private void InitializePlayerEvents()
    {
        foreach (var tank in tanks)
        {
            if (tank.photonView.IsMine) continue;
            tank.OnPlayerDied += () => OnPlayerDied(tank);
        }
    }
    private void OnPlayerDied(PlayerController player)
    {
        Team team = CustomProperty.GetTeam(player.photonView.Owner);
        if (team == Team.Red) redRemain--;
        else blueRemain--;

        Debug.Log($"[MSKTurn] 팀 {team} 남은 인원: {(team == Team.Red ? redRemain : blueRemain)}");

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
        tanks = FindObjectsOfType<PlayerController>();
        // 턴 플레이어 찾기
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
    public void RPC_Spawned()
    {
        GameStart();
    }
    #endregion
}
