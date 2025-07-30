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

        // �÷��̾� ����
        PhotonNetwork.Instantiate(
         "Prefabs/Test Tank", chosenPoint.position, chosenPoint.rotation);

        Debug.Log($"SpawnPlayer: '{chosenPoint.name}' ��ġ�� ���� �Ϸ�! �ε��� = {index}");

        var turnController = FindObjectOfType<MSKTurnController>();
        turnController.photonView.RPC("RPC_NotifySpawned", RpcTarget.All);
    }
}
