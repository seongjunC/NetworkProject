using Photon.Pun;
using System;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static event Action<DeformableTerrain> OnMapLoaded;

    [Header("�� ������ ����")]
    [Tooltip("�ν����Ϳ��� �� �����յ��� ������� �Ҵ��ؾ� �մϴ�.")]
    [SerializeField] private GameObject[] mapPrefabs;

    private void Start()
    {
        // �迭�� ����ְų� �Ҵ���� �ʾҴ��� Ȯ��
        if (mapPrefabs == null || mapPrefabs.Length == 0)
        {
            Debug.LogError("[MapManager] �� ������ �迭(Map Prefabs)�� ����ֽ��ϴ�! �ν����Ϳ��� �Ҵ����ּ���.");
            return;
        }
        LoadMapFromRoomProperties();
    }

    private void LoadMapFromRoomProperties()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("[MapManager] ���� �濡 ������ ���� �ʾ� ���� �ε��� �� �����ϴ�.");
            return;
        }

        var props = PhotonNetwork.CurrentRoom.CustomProperties;
        if (props.TryGetValue("Map", out object mapIndexObject))
        {
            int mapIndex = Convert.ToInt32(mapIndexObject);
            Debug.Log($"[MapManager] �濡�� �ε��� �� �ε����� ã�ҽ��ϴ�: {mapIndex}");

            if (mapIndex >= 0 && mapIndex < mapPrefabs.Length)
            {
                if (mapPrefabs[mapIndex] == null)
                {
                    Debug.LogError($"[MapManager] �� ������ �迭�� �ε��� {mapIndex}�� ����ֽ��ϴ�(null).");
                    return;
                }

                Debug.Log($"[MapManager] �� ������ '{mapPrefabs[mapIndex].name}' ������ �õ��մϴ�.");
                GameObject mapInstance = Instantiate(mapPrefabs[mapIndex], Vector3.zero, Quaternion.identity);

                DeformableTerrain terrain = mapInstance.GetComponentInChildren<DeformableTerrain>();

                if (terrain != null)
                {
                    Debug.Log("[MapManager] DeformableTerrain ������Ʈ�� ���������� ã�ҽ��ϴ�. OnMapLoaded �̺�Ʈ�� ȣ���մϴ�.");
                    OnMapLoaded?.Invoke(terrain);
                }
                else
                {
                    Debug.LogError($"[MapManager] �� ������ '{mapPrefabs[mapIndex].name}' �Ǵ� �� �ڽ� ������Ʈ���� DeformableTerrain ������Ʈ�� ã�� �� �����ϴ�!");
                }
            }
            else
            {
                Debug.LogError($"[MapManager] ��ȿ���� ���� Map �ε����Դϴ�: {mapIndex}");
            }
        }
        else
        {
            Debug.LogError("[MapManager] �� �Ӽ����� 'Map' Ű�� ã�� �� �����ϴ�.");
        }
    }
}