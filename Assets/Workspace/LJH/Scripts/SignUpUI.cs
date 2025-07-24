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

    [Header("������ Ÿ��Ʋ �г�")]
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
        ValidateConfirmPassword(confirmPasswordField.text); // ����ȭ
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
            Manager.UI.PopUpUI.Show("�̸����� �Է����ּ���.");
            return;
        }
        if (string.IsNullOrEmpty(nicknameField.text))
        {
            Manager.UI.PopUpUI.Show("�г����� �Է����ּ���.");
            return;
        }
        if (string.IsNullOrEmpty(passwordField.text))
        {
            Manager.UI.PopUpUI.Show("��й�ȣ�� �Է����ּ���.");
            return;
        }
        if (string.IsNullOrEmpty(confirmPasswordField.text))
        {
            Manager.UI.PopUpUI.Show("��й�ȣ Ȯ���� �Է����ּ���.");
            return;
        }
        if (emailCheck.activeSelf && nicknameCheck.activeSelf &&
            passwordCheck.activeSelf && confirmCheck.activeSelf)
        {
            Debug.Log("ȸ������ ����");
            // ���� ȸ������ ó�� ����

            gameObject.SetActive(false);         // Ÿ��Ʋ �г� �ݱ�
        }
        else
        {
            Debug.Log("��� �׸��� �ùٸ��� �Է����ּ���.");
            // �ʿ�� PopUpUI ������ �ȳ�
        }
    }
    private void OnClickCancel()
    {
        gameObject.SetActive(false);         // Ÿ��Ʋ �г� �ݱ�
        titlePanel.SetActive(true);         // ȸ������ �г� ����
    }
}
