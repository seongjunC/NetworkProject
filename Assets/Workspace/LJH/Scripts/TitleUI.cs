using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TitleUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField idInput;
    [SerializeField] private TMP_InputField pwInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button signUpButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("연결할 패널")]
    [SerializeField] private GameObject signUpPanel;
    [SerializeField] private GameObject mainmenuPanel;

    private void Awake()
    {
        loginButton.onClick.AddListener(OnLoginClicked);
        signUpButton.onClick.AddListener(OnSignUpClicked);
        exitButton.onClick.AddListener(OnClickExit);
        errorText.gameObject.SetActive(false);
    }

    private void OnLoginClicked()
    {
        string id = idInput.text;
        string pw = pwInput.text;

        if (IsValidLogin(id, pw))
        {
            Manager.UI.PopUpUI.Show("Successfully logged in!", Color.green);
            gameObject.SetActive(false); // 타이틀 UI 닫기

            // TODO: 게임 시작 함수 호출

            gameObject.SetActive(false);
            mainmenuPanel.SetActive(true);
        }
        else
        {
            Manager.UI.PopUpUI.Show("Invalid ID or password.", Color.green);
            errorText.gameObject.SetActive(true);
        }
    }
    private void OnSignUpClicked()
    {
        gameObject.SetActive(false);         // 타이틀 패널 닫기
        signUpPanel.SetActive(true);         // 회원가입 패널 열기
    }

    private bool IsValidLogin(string id, string pw)
    {
        // 실제 로그인 로직 (예: 서버 요청 등), 지금은 간단히 하드코딩
        return id == "admin" && pw == "1234";
    }

    private void OnClickExit()
    {
        Application.Quit();
        Debug.Log("게임 종료 시도");
    }
}
