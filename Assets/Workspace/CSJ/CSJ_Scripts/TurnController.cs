using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;

public class TurnController : MonoBehaviourPunCallbacks
{
    private Queue<PlayerInfo> turnQueue = new();
    private List<PlayerInfo> nextCycle = new();
    PlayerController[] tanks;
    private UnityEvent OnGameEnded;
    private PlayerInfo currentPlayer;
    private Room room;
    [SerializeField] float turnLimit = 30f;
    private float turnTimer = 0f;
    private bool isTurnRunning = false;

    void Update()
    {
        if (!PhotonNetwork.IsMasterClient || !isTurnRunning) return;

        turnTimer += Time.deltaTime;

        if (turnTimer >= turnLimit)
        {
            photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }

    private void GameStart()
    {
        turnQueue.Clear();
        nextCycle.Clear();
        tanks = FindObjectsOfType<PlayerController>();

        int playerCount = PhotonNetwork.CountOfPlayers;
        room = PhotonNetwork.CurrentRoom;
        // 추후 연계하여 조절
        if (CustomProperty.GetTurnRandom(room))
        {
            List<int> randNumList = new();
            for (int i = 0; i < playerCount; i++)
            {
                randNumList[i] = i + 1;
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
        // TODO : 로직에서 turnQueue가 1, nextCycle이 0일때 문제 발생 -> 수정 예정 
        if (turnQueue.Count == 0)
        {
            if (nextCycle.Count <= 1)
            {
                Debug.Log("게임 종료");
                photonView.RPC("RPC_GameEnded", RpcTarget.All);
                return;
            }
            else
            {
                photonView.RPC("RPC_CycleEnd", RpcTarget.MasterClient);
                turnQueue = new Queue<PlayerInfo>(nextCycle);
                nextCycle.Clear();
            }
        }

        currentPlayer = turnQueue.Dequeue();

        if (currentPlayer.isDead)
        {
            return;
        }
        turnTimer = 0f;
        isTurnRunning = true;
        photonView.RPC("StartTurnForPlayer", RpcTarget.All, currentPlayer.ActorNumber);

    }

    [PunRPC]
    private void RPC_CycleEnd()
    {

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

    [PunRPC]
    private void RPC_GameEnded()
    {
        Debug.Log("게임 종료!");

        // UI 교체
        // 게임 오버 UI 출력
        // Firebase에 게임 결과를 업로드
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
