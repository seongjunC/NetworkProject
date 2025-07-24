using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public SpriteRenderer mapSpriteRenderer;

    private void Start()
    {
        if (mapSpriteRenderer == null)
        {
            Debug.LogError("mapSpriteRenderer 할당안됨");
            return;
        }

        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;

        if (roomProps.ContainsKey("map"))
        {
            string mapSpritePath = (string)roomProps["map"];
            Debug.Log($"로드할 맵 경로 {mapSpritePath}");

            Sprite mapSprite = Resources.Load<Sprite>(mapSpritePath);

            if (mapSprite != null)
            {
                mapSpriteRenderer.sprite = mapSprite;
                Debug.Log("맵 스프라이트 할당 성공");
            }
            else
            {
                Debug.LogError($"해당 경로에서 스프라이트를 못찾음 {mapSprite}");
            }
        }
        else
        {
            Debug.LogError("룸 프로퍼티에서 맵 정보를 못찾음");
        }
    }
}
