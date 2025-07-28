using Firebase.Database;
using Firebase.Extensions;
using Game;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] Image readyPanel;
    [SerializeField] Image teamPanel;
    [SerializeField] Image masterPanel;
    [SerializeField] Button infoButton;
    [SerializeField] Button playerCloseConnectionButton;

    [Header("ReadyColor")]
    [SerializeField] Sprite readySpite;
    [SerializeField] Sprite defaultSprite;

    [Header("TeamColor")]
    [SerializeField] Color redColor;
    [SerializeField] Color blueColor;
    [SerializeField] Color waitColor;

    private Player player;
    public event Action<Player> OnKick;

    #region LifeCycle
    private void OnEnable()
    {
        PhotonNetwork.EnableCloseConnection = true;
        infoButton.onClick.AddListener(ViewPlayerInfo);
    }

    private void OnDisable()
    {
        infoButton.onClick.RemoveListener(ViewPlayerInfo);
    }
    #endregion

    public void SetUp(Player player)
    {   
        this.player = player;

        if (player != PhotonNetwork.MasterClient)
            playerCloseConnectionButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        else
            playerCloseConnectionButton.gameObject.SetActive(false);

        playerName.text = player.NickName;

        playerName.color = PhotonNetwork.LocalPlayer == player ? Color.green : Color.white;
        masterPanel.color = PhotonNetwork.MasterClient == player ? Color.white : Color.clear;        
        
        Team team =player.GetTeam();
        Debug.Log(team);
        switch (team)
        {
            case Team.Red: 
                teamPanel.color = redColor;
                break;
            case Team.Blue:
                teamPanel.color = blueColor;
                break;
            case Team.Wait:
                teamPanel.color = waitColor;
                break;
        }
        Debug.Log(teamPanel.color);
        readyPanel.sprite = player.GetReady() ? readySpite : defaultSprite;
    }

    public void ViewPlayerInfo()
    {
        Manager.UI.PlayerInfoPanel.Show(player);
    }

    public void TryCloseConnection()
    {
        Manager.UI.PopUpUI_Action.Show($"정말로 {player.NickName} 플레이어를 강퇴 하시겠습니까?", CloseConnection);
    }

    private void CloseConnection()
    {
        OnKick?.Invoke(player);
    }

    public void UpdateReady(Color color)
    {
        readyPanel.color = color;
    }

    public void UpdateTeam(Color color)
    {
        teamPanel.color = color;
    }
}
