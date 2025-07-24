using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField emailField;
    public TMP_InputField nicknameField;
    public TMP_InputField passwordField;
    public TMP_InputField confirmPasswordField;

    [Header("Validation Images")]
    public GameObject emailCheck;
    public GameObject emailCross;
    public GameObject nicknameCheck;
    public GameObject nicknameCross;
    public GameObject passwordCheck;
    public GameObject passwordCross;
    public GameObject confirmCheck;
    public GameObject confirmCross;

    [SerializeField] private Button okButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private TextMeshProUGUI errorText;

    [Header("연결할 타이틀 패널")]
    [SerializeField] private GameObject titlePanel;

    private void Awake()
    {
        okButton.onClick.AddListener(OnClickOK);
        cancelButton.onClick.AddListener(OnClickCancel);
        errorText.gameObject.SetActive(false);
    }
    private void Start()
    {
        emailField.onValueChanged.AddListener(ValidateEmail);
        nicknameField.onValueChanged.AddListener(ValidateNickname);
        passwordField.onValueChanged.AddListener(ValidatePassword);
        confirmPasswordField.onValueChanged.AddListener(ValidateConfirmPassword);
    }

    void ValidateEmail(string input)
    {
        bool isValid = input.Contains("@") && input.Contains(".");
        emailCheck.SetActive(isValid);
        emailCross.SetActive(!isValid);
    }

    void ValidateNickname(string input)
    {
        bool isValid = input.Length >= 3;
        nicknameCheck.SetActive(isValid);
        nicknameCross.SetActive(!isValid);
    }

    void ValidatePassword(string input)
    {
        bool isValid = input.Length >= 6;
        passwordCheck.SetActive(isValid);
        passwordCross.SetActive(!isValid);
        ValidateConfirmPassword(confirmPasswordField.text); // 동기화
    }

    void ValidateConfirmPassword(string input)
    {
        bool isValid = input == passwordField.text && input.Length >= 6;
        confirmCheck.SetActive(isValid);
        confirmCross.SetActive(!isValid);
    }

    public void OnClickOK()
    {
        if (string.IsNullOrEmpty(emailField.text))
        {
            Manager.UI.PopUpUI.Show("이메일을 입력해주세요.");
            return;
        }
        if (string.IsNullOrEmpty(nicknameField.text))
        {
            Manager.UI.PopUpUI.Show("닉네임을 입력해주세요.");
            return;
        }
        if (string.IsNullOrEmpty(passwordField.text))
        {
            Manager.UI.PopUpUI.Show("비밀번호를 입력해주세요.");
            return;
        }
        if (string.IsNullOrEmpty(confirmPasswordField.text))
        {
            Manager.UI.PopUpUI.Show("비밀번호 확인을 입력해주세요.");
            return;
        }
        if (emailCheck.activeSelf && nicknameCheck.activeSelf &&
            passwordCheck.activeSelf && confirmCheck.activeSelf)
        {
            Debug.Log("회원가입 성공");
            // 실제 회원가입 처리 로직

            gameObject.SetActive(false);         // 타이틀 패널 닫기
        }
        else
        {
            Debug.Log("모든 항목을 올바르게 입력해주세요.");
            // 필요시 PopUpUI 등으로 안내
        }
    }
    private void OnClickCancel()
    {
        gameObject.SetActive(false);         // 타이틀 패널 닫기
        titlePanel.SetActive(true);         // 회원가입 패널 열기
    }
}
