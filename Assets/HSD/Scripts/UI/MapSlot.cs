using Game;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSlot : MonoBehaviour
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
        mapIcon.texture = Manager.Resources.Load<Texture2D>($"MapIcon/{mapType.ToString()}");
        mapType = _mapType;
    }

    public void MapSelect()
    {
        PhotonNetwork.CurrentRoom.CustomProperties["Map"] = mapType;
    }
}
