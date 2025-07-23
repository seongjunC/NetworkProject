using Game;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSlot : MonoBehaviour
{
    [SerializeField] Image mapIcon;        
    private MapType mapType;

    public void SetUp(MapType _mapType)
    {
        mapIcon.sprite = Manager.Resources.Load<Sprite>($"MapIcon/{mapType.ToString()}");
        mapType = _mapType;
    }

    public void MapSelect()
    {
        PhotonNetwork.CurrentRoom.CustomProperties["Map"] = mapType;
    }
}
