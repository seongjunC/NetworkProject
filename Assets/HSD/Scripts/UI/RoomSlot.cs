using ExitGames.Client.Photon.StructWrapping;
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
    [SerializeField] Image mapIcon;
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
        mapIcon.sprite = Manager.Resources.Load<Sprite>($"MapIcon/{room.CustomProperties["Map"]}");
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(roomName);
        button.onClick.RemoveListener(JoinRoom);        
    }
}
