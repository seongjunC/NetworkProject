using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SpawnPlayer : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        yield return StartCoroutine(PlayerSpawn());

        MSKTurnController.Instance.photonView.RPC(nameof(MSKTurnController.Instance.RPC_NotifySpawned), RpcTarget.All);
    }

    /// <summary>
    /// 플레이어를 중복되지 않는 랜덤 위치에 생성합니다.
    /// 마스터 클라이언트가 스폰 지점을 섞어 CustomProperties에 저장하고, 모든 클라이언트는 이를 기반으로 스폰합니다.
    /// </summary>
    private IEnumerator PlayerSpawn()
    {
        int spawnCount = transform.childCount;
        if (spawnCount == 0)
        {
            Debug.LogWarning("SpawnPlayer: 자식으로 SpawnPoint가 없어요!");
            yield break;
        }

        // 마스터 클라이언트가 스폰 지점 순서를 섞어서 CustomProperties에 저장
        if (PhotonNetwork.IsMasterClient)
        {
            List<int> spawnIndices = new List<int>();
            for (int i = 0; i < spawnCount; i++)
            {
                spawnIndices.Add(i);
            }

            // 리스트를 랜덤하게 섞음
            var random = new System.Random();
            spawnIndices = spawnIndices.OrderBy(x => random.Next()).ToList();

            // 섞인 리스트를 CustomProperties에 저장
            Hashtable roomProps = new Hashtable
            {
                { "spawnIndices", spawnIndices.ToArray() }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomProps);
            Debug.Log("마스터 클라이언트가 스폰 지점을 섞었습니다: " + string.Join(", ", spawnIndices));
        }

        // 모든 클라이언트는 CustomProperties가 설정될 때까지 대기
        while (PhotonNetwork.CurrentRoom.CustomProperties["spawnIndices"] == null)
        {
            Debug.Log("스폰 지점 정보 대기 중...");
            yield return null; // 다음 프레임까지 대기
        }

        // 저장된 스폰 지점 리스트를 가져옴
        int[] shuffledIndices = (int[])PhotonNetwork.CurrentRoom.CustomProperties["spawnIndices"];

        // 자신의 플레이어 순서(ActorNumber는 1부터 시작하므로 플레이어 리스트의 인덱스를 사용)
        Player[] players = PhotonNetwork.PlayerList;
        int playerIndex = System.Array.IndexOf(players, PhotonNetwork.LocalPlayer);

        if (playerIndex < 0 || playerIndex >= shuffledIndices.Length)
        {
            Debug.LogError($"SpawnPlayer: 잘못된 플레이어 인덱스! playerIndex={playerIndex}, shuffledIndices.Length={shuffledIndices.Length}");
            yield break;
        }

        // 섞인 리스트에서 자신의 스폰 위치 인덱스를 가져옴
        int spawnPointIndex = shuffledIndices[playerIndex];
        Transform chosenPoint = transform.GetChild(spawnPointIndex);

        TankData data = Manager.Data.TankDataController.CurrentTank;

        PhotonNetwork.Instantiate($"Prefabs/{data.tankName}", chosenPoint.position, chosenPoint.rotation, 0,
            new object[] { data.tankName, data.Level });

        Debug.Log($"SpawnPlayer: '{chosenPoint.name}' 위치에 스폰 완료! 플레이어 인덱스 = {playerIndex}, 스폰 위치 인덱스 = {spawnPointIndex}");
    }
}
