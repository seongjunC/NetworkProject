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
    //  맵 생성이후 플레이어를 등록하도록
    private void OnMapGenerated(DeformableTerrain terrain)
    {
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
            // TODO : 로비 설정에 따른 다른 플레이어 탱크 스폰
            // 방법 1 스폰 시 탱크의 정보를 가져와서 스폰
            // 방법 2 프리팹 자식으로 4개의 탱크를 가지고, 플레이어가 선택한 탱크만 활성화
        }
    }*/

    [PunRPC]
    //    private void RPC_SpawnTank(Vector3 spawnPos, int tankTypeIndex)
    private void RPC_SpawnTank(Vector3 spawnPos)
    {

        PhotonNetwork.Instantiate("Prefabs/Test Tank", spawnPos, Quaternion.identity);
        /*
         * TODO : 탱크 로비설정에 따라서 생성하기
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
