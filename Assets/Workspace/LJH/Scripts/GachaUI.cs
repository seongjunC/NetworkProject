using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI gachaTitleText;
    [SerializeField] private TextMeshProUGUI gachaCostText;
    [SerializeField] private TextMeshProUGUI gemText;
    [SerializeField] private Button tenGachaButton;
    [SerializeField] private Button oneGachaButton;
    [SerializeField] private Button componentsButton;
    [SerializeField] private Button gachaRecordButton;
    [SerializeField] private Button tankButton;
    [SerializeField] private Button outButton;
    [SerializeField] private Button pickUpButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button componentsOkButton;
    [SerializeField] private Button gachaRecordOKButton;
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;

    [Header("연결할 패널")]
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject GachaPopupPanel;
    [SerializeField] private GameObject ComponentsPanel;
    [SerializeField] private GameObject GachaRecordPanel;
    [SerializeField] private GameObject TankPanel;
    // Start is called before the first frame update
    void Start()
    {
        gemText.text = "10000"; // 잼 텍스트 초기화
        tenGachaButton.onClick.AddListener(OnClickTenGacha);
        oneGachaButton.onClick.AddListener(OnClickOneGacha);
        componentsButton.onClick.AddListener(OnClickComponents);
        gachaRecordButton.onClick.AddListener(OnClickGachaRecord);
        tankButton.onClick.AddListener(OnClickTank);
        outButton.onClick.AddListener(OnClickOut);
        pickUpButton.onClick.AddListener(OnClickPickUp);
        cancelButton.onClick.AddListener(OnClickCancel);
        componentsOkButton.onClick.AddListener(OnClickComponentsOk);
        gachaRecordOKButton.onClick.AddListener(OnClickGachaRecordOk);
        previousPageButton.onClick.AddListener(OnClickPreviousPage);
        nextPageButton.onClick.AddListener(OnClickNextPage);

    }
    private void OnClickTenGacha()
    {
        GachaPopupPanel.SetActive(true); // 가챠 팝업 패널 열기
        gachaTitleText.text = "탱크 10연속 뽑기";
        gachaCostText.text = "90";
    }
    private void OnClickOneGacha()
    {
        GachaPopupPanel.SetActive(true); // 가챠 팝업 패널 열기
        gachaTitleText.text = "탱크 1회 뽑기";
        gachaCostText.text = "10";
    }
    private void OnClickComponents()
    {
        ComponentsPanel.SetActive(true); // 컴포넌트 패널 열기        
    }
    private void OnClickGachaRecord()
    {
        GachaRecordPanel.SetActive(true); // 가챠 기록 패널 열기        
    }
    private void OnClickPickUp()
    {
        //탱크 뽑는 로직을 여기에 추가
        gemText.text = "0"; // 잼 텍스트 초기화
    }
    private void OnClickCancel()
    {
        GachaPopupPanel.SetActive(false); // 가챠 팝업 패널 닫기
    }
    private void OnClickComponentsOk()
    {
        ComponentsPanel.SetActive(false); // 컴포넌트 패널 닫기
    }
    private void OnClickGachaRecordOk()
    {
        GachaRecordPanel.SetActive(false); // 가챠 기록 패널 닫기
    }
    private void OnClickTank()
    {
        gameObject.SetActive(false); // 가챠 UI 닫기
        TankPanel.SetActive(true); // 보유 탱크 패널 열기
    }

    private void OnClickOut()
    {
        gameObject.SetActive(false); // 가챠 UI 닫기
        MainMenuPanel.SetActive(true); // 메인 메뉴 패널 열기
    }
    private void OnClickPreviousPage()
    {
        //To do : 왼쪽 페이지로 이동하는 로직 구현
    }
    private void OnClickNextPage()
    {
        //To do : 오른쪽 페이지로 이동하는 로직 구현
    }

}
