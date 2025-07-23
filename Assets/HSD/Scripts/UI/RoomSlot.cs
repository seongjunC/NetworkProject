using ExitGames.Client.Photon.StructWrapping;
using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSlot : MonoBehaviour
{    
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text roomCountText;
    [SerializeField] RawImage mapIcon;
    [SerializeField] Button button;
    private string roomName;
    private RoomInfo room;

    private void OnEnable()
    {
        button.onClick.AddListener(JoinRoom);
    }

    public void SetUp(RoomInfo _room)
    {
        this.room = _room;
        roomName = _room.Name;
        roomNameText.text = _room.Name;
        roomCountText.text = $"{_room.PlayerCount} / {_room.MaxPlayers}";
        Texture2D texture = Manager.Resources.Load<Texture2D>($"MapIcon/{((MapType)((int)_room.CustomProperties["Map"])).ToString()}");
        mapIcon.texture = texture;
    }

    public void Refresh()
    {
        if(room != null)
        {
            SetUp(room);
        }
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
        button.onClick.RemoveListener(JoinRoom);        
    }
}
