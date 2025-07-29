using ExitGames.Client.Photon;
using Game;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestMapSlot : MonoBehaviour
{
    [SerializeField] RawImage mapIcon;
    [SerializeField] Button button;
    private MapType mapType;

    private void OnEnable()
    {
        button.onClick.AddListener(MapSelect);
    }

    private void OnDisable()
    {
        button.onClick.RemoveListener(MapSelect);
    }

    public void SetUp(MapType _mapType)
    {
        mapType = _mapType;
        mapIcon.texture = Manager.Resources.Load<Texture2D>($"MapIcon/{mapType.ToString()}");
    }

    public void MapSelect()
    {        
        PhotonNetwork.CurrentRoom.SetMap((int)mapType);
    }
}
