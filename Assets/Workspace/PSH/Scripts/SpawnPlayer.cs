using Photon.Pun;
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

        var turnController = FindObjectOfType<MSKTurnController>();
        turnController.photonView.RPC("RPC_NotifySpawned", RpcTarget.All);
    }

    /// <summary>
    /// �÷��̾� ���� -> ���� �������� ��ũ�� ������ ���� ->
    /// </summary>
    private void PlayerSpawn()
    {
        int spawnCount = transform.childCount;
        if (spawnCount == 0)
        {
            Debug.LogWarning("SpawnPlayer: �ڽ����� SpawnPoint�� �����!");
            return;
        }

        // 0���� spawnCount-1���� ���� �ε���
        //int index = Random.Range(0, spawnCount);
        int index = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Transform chosenPoint = transform.GetChild(index);

        TankData data = Manager.Data.TankDataController.CurrentTank;

        PhotonNetwork.Instantiate("Prefabs/Test Tank", chosenPoint.position, chosenPoint.rotation, 0,
            new object[] { data.tankName, data.Level });

        Debug.Log($"SpawnPlayer: '{chosenPoint.name}' ��ġ�� ���� �Ϸ�! �ε��� = {index}");
    }
}
