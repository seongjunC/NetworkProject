using ExitGames.Client.Photon.StructWrapping;
using Game;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomSlot : MonoBehaviour
{
    [SerializeField] Image roomPanel;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text roomCountText;
    [SerializeField] TMP_Text roomVisibleText;
    [SerializeField] RawImage mapIcon;
    [SerializeField] Button button;
    private string roomName;
    private RoomInfo room;

    public event Action<RoomInfo> OnPasswordRoomSelected;

    private void OnEnable()
    {
        button.onClick.AddListener(JoinRoom);
    }

    public void SetUp(RoomInfo _room)
    {
        this.room = _room;
        roomName = _room.Name;

        // UI
        roomNameText.text = _room.Name;
        roomCountText.text = $"{_room.PlayerCount} / {_room.MaxPlayers}";
        mapIcon.texture = Manager.Resources.Load<Texture2D>($"MapIcon/{((MapType)((int)_room.CustomProperties["Map"])).ToString()}");
        roomVisibleText.text = (string)_room.CustomProperties["Password"] == null ? "Lock : false" : "Lock : true";        

        if (_room.CustomProperties.TryGetValue("Full", out object value))
        {
            roomPanel.color = (bool)value ? Color.red : Color.green;
        }
        else
            roomPanel.color = Color.green;
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
        if ((string)room.CustomProperties["Password"] != null)
        {
            OnPasswordRoomSelected?.Invoke(room);
            return;
        }

        PhotonNetwork.JoinRoom(roomName);
        button.onClick.RemoveListener(JoinRoom);        
    }
}
