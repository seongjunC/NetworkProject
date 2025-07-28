using Photon.Pun;
using System;
using UnityEngine;

public class MapManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private SpriteRenderer mapSpriteRenderer;
    // �̸� ������ �� ��� �迭
    private readonly string[] mapPaths = {
        "Maps/Map1",
        "Maps/Map2",
        "Maps/Map3"
    };

    private void Start()
    {
        mapSpriteRenderer = GetComponent<SpriteRenderer>();
        LoadMapByIndex();
    }

    private void LoadMapByIndex()
    {
        if (!PhotonNetwork.InRoom) return;

        var props = PhotonNetwork.CurrentRoom.CustomProperties;
        if (props.TryGetValue("Map", out object idxObj))
        {
            int idx = Convert.ToInt32(idxObj);
            if (idx >= 0 && idx < mapPaths.Length)
            {
                Sprite spr = Resources.Load<Sprite>(mapPaths[idx]);
                if (spr != null)
                    mapSpriteRenderer.sprite = spr;
                else
                    Debug.LogError($"��������Ʈ �� ã��: {mapPaths[idx]}");
            }
            else
            {
                Debug.LogError($"��ȿ���� ���� Map �ε���: {idx}");
            }
        }
        else
        {
            Debug.LogError("�� ������Ƽ�� Map Ű�� ����");
        }
    }
}
