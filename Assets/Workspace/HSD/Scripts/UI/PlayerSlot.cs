using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
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

    [Header("ReadyColor")]
    [SerializeField] Sprite readySpite;
    [SerializeField] Sprite defaultSprite;

    [Header("TeamColor")]
    [SerializeField] Color redColor;
    [SerializeField] Color blueColor;

    private Player player;

    #region LifeCycle
    private void OnEnable()
    {
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

        playerName.text = player.NickName;

        masterPanel.color = PhotonNetwork.IsMasterClient ? Color.white : Color.clear;

        readyPanel.sprite = player.GetReady() ? readySpite : defaultSprite;

        teamPanel.color = player.GetTeam() == Game.Team.Red ? redColor : blueColor;
    }

    public void ViewPlayerInfo()
    {
        Manager.UI.PlayerInfoPanel.Show(player);
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
