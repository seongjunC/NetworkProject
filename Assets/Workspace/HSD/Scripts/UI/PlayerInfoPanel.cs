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
    [SerializeField] TMP_Text winCount;
    [SerializeField] TMP_Text loseCount;
    [SerializeField] GameObject waitforMessage;
    [SerializeField] GameObject infoPanel;
    [SerializeField] GameObject namePanel;
    [SerializeField] Button exitButton;

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

        Manager.Database.root.Child("UserData").Child(player.GetUID()).Child("Win").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted) return;

            var snapshot = task.Result;

            int win = (int)(long)snapshot.Value;

            winCount.text = win.ToString();
        });
        Manager.Database.root.Child("UserData").Child(player.GetUID()).Child("Lose").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted) return;

            var snapshot = task.Result;

            int lose = (int)(long)snapshot.Value;

            loseCount.text = lose.ToString();

            Init(true);
        });
    }

    private void Init(bool isEnded)
    {
        waitforMessage.SetActive(!isEnded);
        playerName.gameObject.SetActive(isEnded);
        winCount.gameObject.SetActive(isEnded);
        loseCount.gameObject.SetActive(isEnded);
    }
}
