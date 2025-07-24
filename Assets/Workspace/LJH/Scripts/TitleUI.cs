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

    [Header("������ �г�")]
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
            gameObject.SetActive(false); // Ÿ��Ʋ UI �ݱ�

            // TODO: ���� ���� �Լ� ȣ��

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
        gameObject.SetActive(false);         // Ÿ��Ʋ �г� �ݱ�
        signUpPanel.SetActive(true);         // ȸ������ �г� ����
    }

    private bool IsValidLogin(string id, string pw)
    {
        // ���� �α��� ���� (��: ���� ��û ��), ������ ������ �ϵ��ڵ�
        return id == "admin" && pw == "1234";
    }

    private void OnClickExit()
    {
        Application.Quit();
        Debug.Log("���� ���� �õ�");
    }
}
