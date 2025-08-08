using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLobbyPanel : MonoBehaviour
{
    [SerializeField] TMP_Text playerName;
    [SerializeField] TMP_Text winCount;
    [SerializeField] TMP_Text loseCount;
    private PlayerData data;

    #region LifeCycle   

    private void Start()
    {
        Manager.Firebase.OnLogOut += () => data = null;
    }

    private void OnEnable()
    {
        data ??= Manager.Data.PlayerData;

        UpdatePlayerName(data.Name);
        UpdateLoseCount(data.Lose);
        UpdateWinCount(data.Win);

        data.OnNameChanged += UpdatePlayerName;
        data.OnLoseChanged += UpdateLoseCount;
        data.OnWinChanged += UpdateWinCount;
    }

    private void OnDisable()
    {
        data.OnNameChanged -= UpdatePlayerName;
        data.OnLoseChanged -= UpdateLoseCount;
        data.OnWinChanged -= UpdateWinCount;
    }
    #endregion

    private void UpdatePlayerName(string _playerName)
    {
        playerName.text = _playerName;
    }
    private void UpdateWinCount(int _winCount)
    {
        winCount.text = _winCount.ToString();
    }
    private void UpdateLoseCount(int _loseCount)
    {
        loseCount.text = _loseCount.ToString();        
    }
}
