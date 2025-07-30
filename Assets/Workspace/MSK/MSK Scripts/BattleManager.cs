using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBattleManager : MonoBehaviourPun
{
    // [SerializeField] private List<Transform> _spawnPoint;
    [SerializeField] Button _turnEndButton;
    [SerializeField] private MSKTurnController _turnController;

    private PlayerController _playerController;


    #region Unity LifeCycle
    private void Start()
    {
        _turnEndButton.onClick.AddListener(TestTurnEnd);
        // PlayerSpawn();

    }
    private void OnEnable()
    {
        MapManager.OnMapLoaded += OnMapGenerated;
    }

    private void OnDisable()
    {
        MapManager.OnMapLoaded -= OnMapGenerated;
    }

    #endregion
    //  �� �������� �÷��̾ ����ϵ���
    private void OnMapGenerated(DeformableTerrain terrain)
    {
        // _turnController.photonView.RPC("RPC_Spawned", RpcTarget.All);
    }

    private void TestTurnEnd()
    {
        if (_playerController != null)
            _playerController.EndPlayerTurn();

        if (_turnController.IsMyTurn())
            _turnController.TurnFinished();
    }
    private void PlayerAttacked()
    {
        if (_playerController._hp <= 0)
        {
            // 사망 처리
            _turnController.photonView.RPC("RPC_PlayerDead", RpcTarget.All);
            // 죽은 유저가 자신의 턴이었다면, 턴도 종료
            if (_turnController.IsMyTurn())
            {
                _turnController.TurnFinished();
            }
            return;
        }
        // 살아있고 내 턴이면 턴 종료
        if (_turnController.IsMyTurn())
        {
            _turnController.TurnFinished();
        }
    }

    public void RegisterPlayer(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.OnPlayerAttacked += PlayerAttacked;
    }

    /*
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
    }*/

    [PunRPC]
    //    private void RPC_SpawnTank(Vector3 spawnPos, int tankTypeIndex)
    private void RPC_SpawnTank(Vector3 spawnPos)
    {

        PhotonNetwork.Instantiate("Prefabs/Test Tank", spawnPos, Quaternion.identity);
        /*
         * TODO : ��ũ �κ����� ���� �����ϱ�
        TankType tankType = (TankType)tankTypeIndex;
        if (_tankPrefabPaths.TryGetValue(tankType, out string prefabPath))
        {
            PhotonNetwork.Instantiate(prefabPath, spawnPos, Quaternion.identity);
        }
        */
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
