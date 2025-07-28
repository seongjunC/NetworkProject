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
    private void TestTurnEnd()
    {
        if (_playerController != null)
            _playerController.EndPlayerTurn();
        _turnController.TurnFinished();

    }
    private void PlayerAttacked()
    {
        _turnController.photonView.RPC("RPC_TurnFinished", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        if (_playerController._hp <= 0)
            _turnController.photonView.RPC("RPC_PlayerDead", RpcTarget.All);
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
            // TODO : �κ� ������ ���� �ٸ� �÷��̾� ��ũ ����
            // ��� 1 ���� �� ��ũ�� ������ �����ͼ� ����
            // ��� 2 ������ �ڽ����� 4���� ��ũ�� ������, �÷��̾ ������ ��ũ�� Ȱ��ȭ
        }
    }

    [PunRPC]
    private void RPC_SpawnTank(Vector3 spawnPos, int tankTypeIndex)
    {
        TankType tankType = (TankType)tankTypeIndex;
        if (_tankPrefabPaths.TryGetValue(tankType, out string prefabPath))
        {
            PhotonNetwork.Instantiate(prefabPath, spawnPos, Quaternion.identity);
        }
    }

    private readonly Dictionary<TankType, string> _tankPrefabPaths = new()
    {
        { TankType.Tank1, "Prefabs/Tank1" },
        { TankType.Tank2, "Prefabs/Tank2" },
        { TankType.Tank3, "Prefabs/Tank3" },
        { TankType.Tank4, "Prefabs/Tank4" },
    };
}

public enum TankType
{
    Tank1,
    Tank2,
    Tank3,
    Tank4
}
