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
    [Header("Sprite")]
    [SerializeField] Sprite lockSprite;
    [SerializeField] Sprite unlockSprite;
    [Space]

    [SerializeField] Image roomPanel;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] TMP_Text roomCountText;
    [SerializeField] Image roomPasswordImage;
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
        roomPasswordImage.sprite = (string)_room.CustomProperties["Password"] == null ? unlockSprite : lockSprite;

        if (_room.CustomProperties.TryGetValue("Full", out object value))
        {
            roomPanel.color = (bool)value ? Color.red : Color.white;
        }
        else
            roomPanel.color = Color.white;
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
        if ((bool)room.CustomProperties["Full"] == true)
        {
            Manager.UI.PopUpUI.Show("방 인원이 가득 찼습니다.");
            return;
        }

        if ((bool)room.CustomProperties["GameStart"] == true)
        {
            Manager.UI.PopUpUI.Show("이미 게임이 시작된 방입니다.");
            return;
        }

        if ((string)room.CustomProperties["Password"] != null)
        {
            OnPasswordRoomSelected?.Invoke(room);
            return;
        }
        button.onClick.RemoveListener(JoinRoom);

        StartCoroutine(JoinRoomRoutine());
    }

    private IEnumerator JoinRoomRoutine()
    {
        Manager.UI.FadeScreen.FadeIn(.5f);

        yield return new WaitForSeconds(.5f);

        PhotonNetwork.JoinRoom(roomName);       
    }
}
