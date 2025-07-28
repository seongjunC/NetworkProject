using Photon.Pun;
using System;
using UnityEngine;

public class MapManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private SpriteRenderer mapSpriteRenderer;
    // 미리 정의해 둔 경로 배열
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
                    Debug.LogError($"스프라이트 못 찾음: {mapPaths[idx]}");
            }
            else
            {
                Debug.LogError($"유효하지 않은 Map 인덱스: {idx}");
            }
        }
        else
        {
            Debug.LogError("룸 프로퍼티에 Map 키가 없음");
        }
    }
}
