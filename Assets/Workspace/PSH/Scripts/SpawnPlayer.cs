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
            Debug.LogWarning("SpawnPlayer: 자식으로 SpawnPoint가 없어요!");
            return;
        }

        // 0부터 spawnCount-1까지 랜덤 인덱스
        int index = Random.Range(0, spawnCount);
        Transform chosenPoint = transform.GetChild(index);

        // 플레이어 스폰
        PhotonNetwork.Instantiate(
         "Prefabs/Test Tank", chosenPoint.position, chosenPoint.rotation);

        Debug.Log($"SpawnPlayer: '{chosenPoint.name}' 위치에 스폰 완료! 인덱스 = {index}");
    }
}
