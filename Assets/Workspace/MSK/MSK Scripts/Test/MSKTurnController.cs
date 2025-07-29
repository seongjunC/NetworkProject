using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MSKTurnController : MonoBehaviourPunCallbacks
{
    private Queue<PlayerInfo> turnQueue = new();
    private List<PlayerInfo> nextCycle = new();
    PlayerController[] tanks;
    private UnityEvent OnGameEnded;
    private PlayerInfo currentPlayer;
    private Room room;
    public PhotonView PhotonView { get; private set; }

    private void Start()
    {
        PhotonView = GetComponent<PhotonView>();
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
        if (turnQueue.Count == 0)
        {
            if (nextCycle.Count <= 1)
            {
                photonView.RPC("RPC_GameEnded", RpcTarget.All, currentPlayer.ActorNumber);
                return;
            }
            else
            {
                turnQueue = new Queue<PlayerInfo>(nextCycle);
                nextCycle.Clear();
            }
        }
        currentPlayer = turnQueue.Dequeue();

        if (currentPlayer.isDead)
        {
            StartNextTurn();
            return;
        }

        photonView.RPC("RPC_SetCameraTarget", RpcTarget.All, currentPlayer.ActorNumber);
        photonView.RPC("StartTurnForPlayer", RpcTarget.All, currentPlayer.ActorNumber);
    }

    public void TurnFinished() 
    {
        // if (photonView.IsMine)
            photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    public void RPC_Spawned()
    {
        GameStart();
    }

    [PunRPC]
    private void RPC_TurnFinished(int actorNumber)
    {
        if (currentPlayer != null && currentPlayer.ActorNumber == actorNumber)
        {
            nextCycle.Add(currentPlayer);
            StartNextTurn();
            Debug.Log("조건문");
        }
    }

    [PunRPC]
    private void RPC_GameEnded(int someInt)
    {
        Debug.Log("게임 종료!");
    }

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
            EnableCurrentPlayer();
        }
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
}
