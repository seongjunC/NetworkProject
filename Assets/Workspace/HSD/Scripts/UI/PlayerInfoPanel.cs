using Firebase.Extensions;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoPanel : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text winRate;
    [SerializeField] TMP_Text winCount;
    [SerializeField] TMP_Text loseCount;
    [SerializeField] GameObject waitforMessage;
    [SerializeField] GameObject infoPanel;
    [SerializeField] GameObject namePanel;
    [SerializeField] Button exitButton;

    private int win;
    private int lose;

    #region LifeCycle
    private void Start()
    {
        exitButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
            gameObject.SetActive(false);
    }
    #endregion

    public void Show(Player player)
    {
        Init(false);        
        gameObject.SetActive(true);

        playerName.text = player.NickName;

        Manager.Database.root.Child("UserData").Child(player.GetUID()).Child("Data").Child("Win").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted) return;

            var snapshot = task.Result;

            win = (int)(long)snapshot.Value;

            winCount.text = win.ToString();

            if(task.IsCompleted)
            {
                Manager.Database.root.Child("UserData").Child(player.GetUID()).Child("Data").Child("Lose").GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted) return;

                    var snapshot = task.Result;

                    lose = (int)(long)snapshot.Value;

                    loseCount.text = lose.ToString();

                    Init(true);
                });
            }
        });     
    }

    private void Init(bool isEnded)
    {
        float percent = 0f;

        if (win + lose > 0)
        {
            percent = (win / (float)(win + lose)) * 100f;
        }

        winRate.text = $"{percent.ToString("F1")} %";

        waitforMessage.SetActive(!isEnded);
        namePanel.SetActive(isEnded);
        infoPanel.SetActive(isEnded);
        winRate.gameObject.SetActive(isEnded);
    }
}
