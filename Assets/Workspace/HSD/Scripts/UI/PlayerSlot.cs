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
    [SerializeField] Image myPanel;
    [SerializeField] Button infoButton;
    [SerializeField] Button playerCloseConnectionButton;

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

        playerCloseConnectionButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        playerName.text = player.NickName;

        myPanel.color = PhotonNetwork.LocalPlayer == player ? Color.green : Color.clear;
        masterPanel.color = PhotonNetwork.IsMasterClient ? Color.white : Color.clear;
        teamPanel.color = player.GetTeam() == Game.Team.Red ? redColor : blueColor;
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
        PhotonNetwork.CloseConnection(player);
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
