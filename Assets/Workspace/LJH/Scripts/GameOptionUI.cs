using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionUI : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;

    [Header("연결할 패널")]
    [SerializeField] private GameObject mainmenuPanel;

    // Start is called before the first frame update
    void Start()
    {
        // 저장되어있는 게임 옵션 초기화 작업
    }

    private void Awake()
    {
        saveButton.onClick.AddListener(OnClickSave);
        cancelButton.onClick.AddListener(OnClickCancel);
    }
    private void OnClickSave()
    {
        //To do : 게임 옵선 저장 로직 구현
    }
    private void OnClickCancel()
    {
        gameObject.SetActive(false);         // 옵션 패널 닫기
        mainmenuPanel.SetActive(true);       // 메인 메뉴 패널 열기
    }
}
