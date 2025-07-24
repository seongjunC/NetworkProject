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
            Debug.LogError("mapSpriteRenderer �Ҵ�ȵ�");
            return;
        }

        var roomProps = PhotonNetwork.CurrentRoom.CustomProperties;

        if (roomProps.ContainsKey("map"))
        {
            string mapSpritePath = (string)roomProps["map"];
            Debug.Log($"�ε��� �� ��� {mapSpritePath}");

            Sprite mapSprite = Resources.Load<Sprite>(mapSpritePath);

            if (mapSprite != null)
            {
                mapSpriteRenderer.sprite = mapSprite;
                Debug.Log("�� ��������Ʈ �Ҵ� ����");
            }
            else
            {
                Debug.LogError($"�ش� ��ο��� ��������Ʈ�� ��ã�� {mapSprite}");
            }
        }
        else
        {
            Debug.LogError("�� ������Ƽ���� �� ������ ��ã��");
        }
    }
}
