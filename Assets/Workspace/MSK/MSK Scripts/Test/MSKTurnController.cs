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

    public void GameStart()
    {
        turnQueue.Clear();
        nextCycle.Clear();
        tanks = FindObjectsOfType<PlayerController>();

        foreach (var tank in tanks)
        {
            PhotonView view = tank.GetComponent<PhotonView>();
            string owner = view != null && view.Owner != null ? view.Owner.NickName : "null";
            Debug.Log($"탱크 발견 - 닉네임: {owner}, ActorNumber: {view?.Owner?.ActorNumber}");
        }

        int playerCount = PhotonNetwork.CountOfPlayers;
        room = PhotonNetwork.CurrentRoom;

        if (CustomProperty.GetTurnRandom(room))
        {
            List<int> randNumList = new();
            for (int i = 0; i < playerCount; i++)
            {
                randNumList.Add(i + 1);
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
                Debug.Log($"기본 순서 플레이어 추가: ActorNumber={p.ActorNumber}");
                turnQueue.Enqueue(new PlayerInfo(p));
            }
        }

        StartNextTurn();
    }

    private void StartNextTurn()
    {
        Debug.Log($"StartNextTurn 호출 - 남은 플레이어 수: {turnQueue.Count}");

        if (turnQueue.Count == 0)
        {
            Debug.Log($"턴 큐 비었음, 다음 사이클 플레이어 수: {nextCycle.Count}");
            if (nextCycle.Count <= 1)
            {
                photonView.RPC("RPC_GameEnded", RpcTarget.All, currentPlayer.ActorNumber);
                return;
            }
            else
            {
                turnQueue = new Queue<PlayerInfo>(nextCycle);
                nextCycle.Clear();
                Debug.Log("다음 사이클로 턴 큐 갱신");
            }
        }

        currentPlayer = turnQueue.Dequeue();
        Debug.Log($"현재 턴 플레이어: ActorNumber={currentPlayer.ActorNumber}, isDead={currentPlayer.isDead}");

        if (currentPlayer.isDead)
        {
            Debug.Log($"플레이어 {currentPlayer.ActorNumber} 사망 상태, 다음 턴으로 이동");
            StartNextTurn();
            return;
        }
        photonView.RPC("StartTurnForPlayer", RpcTarget.All, currentPlayer.ActorNumber);
    }

    [PunRPC]
    public void RPC_Spawned()
    {
        Debug.Log("게임 시작");
        GameStart();
    }

    [PunRPC]
    private void RPC_TurnFinished(int actorNumber)
    {
        Debug.Log($"RPC_TurnFinished 호출: ActorNumber={actorNumber}");
        if (currentPlayer != null && currentPlayer.ActorNumber == actorNumber)
        {
            nextCycle.Add(currentPlayer);
            Debug.Log($"플레이어 {actorNumber} 턴 종료, nextCycle에 추가됨");
            StartNextTurn();
        }
    }

    [PunRPC]
    private void RPC_GameEnded()
    {
        Debug.Log("게임 종료!");
    }

    [PunRPC]
    private void RPC_PlayerDead()
    {
        Debug.Log($"플레이어 {currentPlayer.ActorNumber} 사망 처리");
        currentPlayer.isDead = true;
    }

    [PunRPC]
    void StartTurnForPlayer(int actorNumber)
    {
        Debug.Log($"StartTurnForPlayer 호출: ActorNumber={actorNumber}, LocalPlayer={PhotonNetwork.LocalPlayer.ActorNumber}");
        if (PhotonNetwork.LocalPlayer.ActorNumber == actorNumber)
        {
            Debug.Log("내 턴 시작");
            EnableCurrentPlayer();
        }
    }

    void EnableCurrentPlayer()
    {
        Debug.Log("EnableCurrentPlayer 호출");
        foreach (PlayerController player in tanks)
        {
            PhotonView view = player.GetComponent<PhotonView>();
            if (view != null && view.Owner != null)
            {
                if (view.Owner.ActorNumber == currentPlayer.ActorNumber)
                {
                    Debug.Log($"플레이어 {currentPlayer.ActorNumber} 컨트롤 활성화");
                    player.EnableControl(true);
                }
                else
                {
                    player.EnableControl(false);
                }
            }
        }
    }


}
