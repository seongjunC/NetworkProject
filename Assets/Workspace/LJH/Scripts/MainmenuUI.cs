using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainmenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI welcomeText;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button gachaButton; // Gacha ��ư �߰�
    [SerializeField] private Button TankButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button playerInfoButton;
    [SerializeField] private Button achievementButton;
    [SerializeField] private Button exitButton;
    

    [Header("������ �г�")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject optionPanel;
    [SerializeField] private GameObject playerInfoPanel;
    [SerializeField] private GameObject GachaPanel;
    [SerializeField] private GameObject TankPanel;
    [SerializeField] private GameObject achievementPanel;

    private void Awake()
    {
        gameStartButton.onClick.AddListener(OnClickGameStart);
        gachaButton.onClick.AddListener(OnClickGacha);
        TankButton.onClick.AddListener(OnClickTank);
        optionButton.onClick.AddListener(OnClickOption);
        playerInfoButton.onClick.AddListener(OnClickPlayerInfo);
        achievementButton.onClick.AddListener(OnClickAchievement);
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
    private void OnClickGacha()
    {
        gameObject.SetActive(false);         // Ÿ��Ʋ �г� �ݱ�
        GachaPanel.SetActive(true);         // ��í �г� ����
    }
    private void OnClickTank()
    {
        gameObject.SetActive(false);         // Ÿ��Ʋ �г� �ݱ�
        TankPanel.SetActive(true);         // ��ũ �г� ����
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
    private void OnClickAchievement()
    {
        gameObject.SetActive(false);         // Ÿ��Ʋ �г� �ݱ�
        achievementPanel.SetActive(true);         // ���� �г� ����
    }

    private void OnClickExit()
    {
        Application.Quit();
        Debug.Log("���� ���� �õ�");
    }

}
