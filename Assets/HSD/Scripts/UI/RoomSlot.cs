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

    private void OnEnable()
    {
        button.onClick.AddListener(JoinRoom);
    }

    public void SetUp(RoomInfo room)
    {
        roomName = room.Name;
        roomNameText.text = room.Name;
        roomCountText.text = $"{room.PlayerCount} / {room.MaxPlayers}";
        Texture2D texture = Manager.Resources.Load<Texture2D>($"MapIcon/{((MapType)((int)room.CustomProperties["Map"])).ToString()}");
        mapIcon.texture = texture;
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
        button.onClick.RemoveListener(JoinRoom);        
    }
}
