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

    [Header("연결할 패널")]
    [SerializeField] private GameObject mainmenuPanel;

    // Start is called before the first frame update
    void Start()
    {
        // 플레이어의 저장된 닉네임 초기화 작업

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
        //To do : 플레이어 정보 저장 로직 구현
    }
    private void OnClickCancel()
    {
        gameObject.SetActive(false);         // 옵션 패널 닫기
        mainmenuPanel.SetActive(true);       // 메인 메뉴 패널 열기
    }
}
