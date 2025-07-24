using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainmenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI welcomeText;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button playerInfoButton;
    [SerializeField] private Button exitButton;

    [Header("������ �г�")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject playerInfoPanel;

    private void Awake()
    {
        gameStartButton.onClick.AddListener(OnClickGameStart);
        optionButton.onClick.AddListener(OnClickOption);
        playerInfoButton.onClick.AddListener(OnClickPlayerInfo);
        exitButton.onClick.AddListener(OnClickExit);

    }
    // Start is called before the first frame update
    void Start()
    {
        //if (!string.IsNullOrEmpty(UserData.Nickname))
        //{
        //    welcomeText.text = $"Welcome, {UserData.Nickname}!";
        //}
        //else
        //{
        //    welcomeText.text = "Welcome!";
        //}

        welcomeText.text = "Welcome!";
    }

    private void OnClickGameStart()
    {
        gameObject.SetActive(false);         // Ÿ��Ʋ �г� �ݱ�
        lobbyPanel.SetActive(true);         // �κ� �г� ����
    }

    private void OnClickOption()
    {
        gameObject.SetActive(false);         // Ÿ��Ʋ �г� �ݱ�
        optionPanel.SetActive(true);         // �ɼ� �г� ����
    }

    private void OnClickPlayerInfo()
    {
        gameObject.SetActive(false);         // Ÿ��Ʋ �г� �ݱ�
        playerInfoPanel.SetActive(true);         // �÷��̾� ���� �г� ����
    }
    private void OnClickExit()
    {
        Application.Quit();
        Debug.Log("���� ���� �õ�");
    }

}
