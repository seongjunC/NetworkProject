using Photon.Pun;
using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static event Action<DeformableTerrain> OnMapLoaded;

    [Header("맵 프리팹 설정")]
    [Tooltip("인스펙터에서 맵 프리팹들을 순서대로 할당해야 합니다.")]
    [SerializeField] private GameObject[] mapPrefabs;

    private void Start()
    {
        // 배열이 비어있거나 할당되지 않았는지 확인
        if (mapPrefabs == null || mapPrefabs.Length == 0)
        {
            Debug.LogError("[MapManager] 맵 프리팹 배열(Map Prefabs)이 비어있습니다! 인스펙터에서 할당해주세요.");
            return;
        }
        LoadMapFromRoomProperties();
    }

    private void LoadMapFromRoomProperties()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("[MapManager] 포톤 방에 접속해 있지 않아 맵을 로드할 수 없습니다.");
            return;
        }

        var props = PhotonNetwork.CurrentRoom.CustomProperties;
        if (props.TryGetValue("Map", out object mapIndexObject))
        {
            int mapIndex = Convert.ToInt32(mapIndexObject);
            Debug.Log($"[MapManager] 방에서 로드할 맵 인덱스를 찾았습니다: {mapIndex}");

            if (mapIndex >= 0 && mapIndex < mapPrefabs.Length)
            {
                if (mapPrefabs[mapIndex] == null)
                {
                    Debug.LogError($"[MapManager] 맵 프리팹 배열의 인덱스 {mapIndex}가 비어있습니다(null).");
                    return;
                }

                Debug.Log($"[MapManager] 맵 프리팹 '{mapPrefabs[mapIndex].name}' 생성을 시도합니다.");
                GameObject mapInstance = Instantiate(mapPrefabs[mapIndex], Vector3.zero, Quaternion.identity);

                DeformableTerrain terrain = mapInstance.GetComponentInChildren<DeformableTerrain>();

                if (terrain != null)
                {
                    Debug.Log("[MapManager] DeformableTerrain 컴포넌트를 성공적으로 찾았습니다. OnMapLoaded 이벤트를 호출합니다.");
                    OnMapLoaded?.Invoke(terrain);
                }
                else
                {
                    Debug.LogError($"[MapManager] 맵 프리팹 '{mapPrefabs[mapIndex].name}' 또는 그 자식 오브젝트에서 DeformableTerrain 컴포넌트를 찾을 수 없습니다!");
                }
            }
            else
            {
                Debug.LogError($"[MapManager] 유효하지 않은 Map 인덱스입니다: {mapIndex}");
            }
        }
        else
        {
            Debug.LogError("[MapManager] 방 속성에서 'Map' 키를 찾을 수 없습니다.");
        }
    }
}