using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [Header("Setting Button")]
    [SerializeField] Button gameSettingButton;
    [SerializeField] Button accountSettingButton;
    [SerializeField] Button closeButton;

    [Header("Setting Panel")]
    [SerializeField] GameObject gamePanel;
    [SerializeField] GameObject accountPanel;

    [Header("Account Button")]
    [SerializeField] Button nickNameChangeButton;
    [SerializeField] Button accountDeleteButton;

    #region LifeCycle
    private void Start()
    {
        accountDeleteButton.onClick.AddListener(OpenAccountDeletePanel);
        nickNameChangeButton.onClick.AddListener(OpenNickNameSelectPanel);

        closeButton.onClick.AddListener(Close);
        gameSettingButton.onClick.AddListener(SelectGameSetting);
        accountSettingButton.onClick.AddListener(SelectSettingSetting);
    }

    private void OnEnable()
    {
        SelectGameSetting();

        accountSettingButton.interactable = !PhotonNetwork.InRoom;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            Close();
    }
    #endregion

    public void Show() => gameObject.SetActive(true);
    private void Close() => gameObject.SetActive(false);

    private void SelectGameSetting()
    {
        gamePanel.SetActive(true);
        accountPanel.SetActive(false);
    }

    private void SelectSettingSetting()
    {
        gamePanel.SetActive(false);
        accountPanel.SetActive(true);
    }

    private void OpenNickNameSelectPanel()
    {
        gameObject.SetActive(false);
        Manager.UI.NickNameSelectPanel.Show();
    }

    private void OpenAccountDeletePanel()
    {
        gameObject.SetActive(false);
        Manager.UI.AccountDeletePanel.Show();
    }
}
