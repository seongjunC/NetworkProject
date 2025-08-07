using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class SpawnPlayer : MonoBehaviour
{
    private void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        PlayerSpawn();

        MSKTurnController.Instance.photonView.RPC(nameof(MSKTurnController.Instance.RPC_NotifySpawned), RpcTarget.All);
    }

    /// <summary>
    /// 플레이어 생성 -> 현재 선택중인 탱크로 데이터 세팅 ->
    /// </summary>
    private void PlayerSpawn()
    {
        int spawnCount = transform.childCount;
        if (spawnCount == 0)
        {
            Debug.LogWarning("SpawnPlayer: 자식으로 SpawnPoint가 없어요!");
            return;
        }

        // 0부터 spawnCount-1까지 랜덤 인덱스
        //int index = Random.Range(0, spawnCount);

        Player[] players = PhotonNetwork.PlayerList;
        int index = System.Array.IndexOf(players, PhotonNetwork.LocalPlayer);

        if (index < 0 || index >= spawnCount)
        {
            Debug.LogError($"SpawnPlayer: 잘못된 인덱스! index={index}, spawnCount={spawnCount}");
            return;
        }

        Transform chosenPoint = transform.GetChild(index);

        TankData data = Manager.Data.TankDataController.CurrentTank;

        PhotonNetwork.Instantiate($"Prefabs/{data.tankName}", chosenPoint.position, chosenPoint.rotation, 0,
            new object[] { data.tankName, data.Level });

        Debug.Log($"SpawnPlayer: '{chosenPoint.name}' 위치에 스폰 완료! 인덱스 = {index}");
    }
}
