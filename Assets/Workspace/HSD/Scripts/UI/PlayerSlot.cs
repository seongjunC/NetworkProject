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

    [Header("ReadyColor")]
    [SerializeField] Sprite readySpite;
    [SerializeField] Sprite defaultSprite;

    [Header("TeamColor")]
    [SerializeField] Color redColor;
    [SerializeField] Color blueColor;

    public void SetUp(Player player)
    {
        playerName.text = player.NickName;

        masterPanel.color = PhotonNetwork.IsMasterClient ? Color.white : Color.clear;

        readyPanel.sprite = player.GetReady() ? readySpite : defaultSprite;

        teamPanel.color = player.GetTeam() == Game.Team.Red ? redColor : blueColor;

        Manager.Database.root.Child("UserData").Child(player.GetUID()).Child("Win").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted) return;

            DataSnapshot snapshot = task.Result;

            int win = (int)(long)snapshot.Value;
        });
        Manager.Database.root.Child("UserData").Child(player.GetUID()).Child("Lose").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted) return;

            DataSnapshot snapshot = task.Result;

            int lose = (int)(long)snapshot.Value;
        });
    }

    public void ViewPlayerInfo()
    {
        // 정보 보기
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
