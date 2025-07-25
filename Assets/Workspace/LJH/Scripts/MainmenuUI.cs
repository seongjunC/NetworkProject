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

    [Header("연결할 패널")]
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
        gameObject.SetActive(false);         // 타이틀 패널 닫기
        lobbyPanel.SetActive(true);         // 로비 패널 열기
    }

    private void OnClickOption()
    {
        gameObject.SetActive(false);         // 타이틀 패널 닫기
        optionPanel.SetActive(true);         // 옵션 패널 열기
    }

    private void OnClickPlayerInfo()
    {
        gameObject.SetActive(false);         // 타이틀 패널 닫기
        playerInfoPanel.SetActive(true);         // 플레이어 정보 패널 열기
    }
    private void OnClickExit()
    {
        Application.Quit();
        Debug.Log("게임 종료 시도");
    }

}
