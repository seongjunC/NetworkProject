using Firebase.Database;
using Firebase.Extensions;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlot : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text winText;
    [SerializeField] TMP_Text loseText;
    [SerializeField] Image panel;
    [SerializeField] Image teamPanel;

    public void SetUp(Player player)
    {
        playerName.text = player.NickName;

        Manager.Database.root.Child("UserData").Child(player.GetUID()).Child("Win").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted) return;

            DataSnapshot snapshot = task.Result;

            int win = (int)(long)snapshot.Value;
            winText.text = $"Win : {win}";
        });
        Manager.Database.root.Child("UserData").Child(player.GetUID()).Child("Lose").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted) return;

            DataSnapshot snapshot = task.Result;

            int lose = (int)(long)snapshot.Value;
            loseText.text = $"Lose : {lose}";
        });
    }

    public void ViewPlayerInfo()
    {
        // 정보 보기
    }

    public void UpdateReady(Color color)
    {
        panel.color = color;
    }

    public void UpdateTeam(Color color)
    {
        teamPanel.color = color;
    }
}
