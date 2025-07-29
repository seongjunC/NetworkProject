using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainmenuUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI welcomeText;
    [SerializeField] private Button gameStartButton;
    [SerializeField] private Button gachaButton; // Gacha 버튼 추가
    [SerializeField] private Button TankButton;
    [SerializeField] private Button optionButton;
    [SerializeField] private Button playerInfoButton;
    [SerializeField] private Button achievementButton;
    [SerializeField] private Button exitButton;
    

    [Header("연결할 패널")]
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
        gameObject.SetActive(false);         // 타이틀 패널 닫기
        lobbyPanel.SetActive(true);         // 로비 패널 열기
    }
    private void OnClickGacha()
    {
        gameObject.SetActive(false);         // 타이틀 패널 닫기
        GachaPanel.SetActive(true);         // 가챠 패널 열기
    }
    private void OnClickTank()
    {
        gameObject.SetActive(false);         // 타이틀 패널 닫기
        TankPanel.SetActive(true);         // 탱크 패널 열기
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
    private void OnClickAchievement()
    {
        gameObject.SetActive(false);         // 타이틀 패널 닫기
        achievementPanel.SetActive(true);         // 업적 패널 열기
    }

    private void OnClickExit()
    {
        Application.Quit();
        Debug.Log("게임 종료 시도");
    }

}
