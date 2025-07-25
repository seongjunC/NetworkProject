using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBattleManager : MonoBehaviourPun
{
    [SerializeField] private List<Transform> _spawnPoint;
    [SerializeField] Button _turnEndButton;
    [SerializeField] private MSKTurnController _turnController;
    private PlayerController _playerController;
    private TestNetworkManager _networkManager;

    private void Start()
    {
        _turnEndButton.onClick.AddListener(TestTurnEnd);
        PlayerSpawn();
        _turnController.photonView.RPC("RPC_Spawned", RpcTarget.All);
    }

    private void PlayerAttacked()
    {
        _turnController.photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    private void TestTurnEnd()
    {
        if (_playerController != null)
            _playerController.EndPlayerTurn();
        _turnController.photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }


    public void RegisterPlayer(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.OnPlayerAttacked += PlayerAttacked;
    }

    private void PlayerSpawn()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

        for (int i = 0; i < players.Length; i++)
        {
            if (i >= _spawnPoint.Count)
                break;

            Vector3 spawnPos = _spawnPoint[i].position;
            photonView.RPC("RPC_SpawnTank", players[i], spawnPos);
        }
    }

    [PunRPC]
    private void RPC_SpawnTank(Vector3 spawnPos)
    {
        PhotonNetwork.Instantiate("Prefabs/Test Tank", spawnPos, Quaternion.identity);
    }
}
