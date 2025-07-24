using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    [Header("Input Fields")]
    public TMP_InputField nicknameField;

    public GameObject nicknameCheck;
    public GameObject nicknameCross;

    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;

    [Header("������ �г�")]
    [SerializeField] private GameObject mainmenuPanel;

    // Start is called before the first frame update
    void Start()
    {
        // �÷��̾��� ����� �г��� �ʱ�ȭ �۾�

        nicknameField.onValueChanged.AddListener(ValidateNickname);
    }

    private void Awake()
    {
        saveButton.onClick.AddListener(OnClickSave);
        cancelButton.onClick.AddListener(OnClickCancel);
    }
    void ValidateNickname(string input)
    {
        bool isValid = input.Length >= 3;
        nicknameCheck.SetActive(isValid);
        nicknameCross.SetActive(!isValid);
    }
    private void OnClickSave()
    {
        //To do : �÷��̾� ���� ���� ���� ����
    }
    private void OnClickCancel()
    {
        gameObject.SetActive(false);         // �ɼ� �г� �ݱ�
        mainmenuPanel.SetActive(true);       // ���� �޴� �г� ����
    }
}
