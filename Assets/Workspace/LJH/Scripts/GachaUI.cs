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

    [Header("������ �г�")]
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject GachaPopupPanel;
    [SerializeField] private GameObject ComponentsPanel;
    [SerializeField] private GameObject GachaRecordPanel;
    [SerializeField] private GameObject TankPanel;
    // Start is called before the first frame update
    void Start()
    {
        gemText.text = "10000"; // �� �ؽ�Ʈ �ʱ�ȭ
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
        GachaPopupPanel.SetActive(true); // ��í �˾� �г� ����
        gachaTitleText.text = "��ũ 10���� �̱�";
        gachaCostText.text = "90";
    }
    private void OnClickOneGacha()
    {
        GachaPopupPanel.SetActive(true); // ��í �˾� �г� ����
        gachaTitleText.text = "��ũ 1ȸ �̱�";
        gachaCostText.text = "10";
    }
    private void OnClickComponents()
    {
        ComponentsPanel.SetActive(true); // ������Ʈ �г� ����        
    }
    private void OnClickGachaRecord()
    {
        GachaRecordPanel.SetActive(true); // ��í ��� �г� ����        
    }
    private void OnClickPickUp()
    {
        //��ũ �̴� ������ ���⿡ �߰�
        gemText.text = "0"; // �� �ؽ�Ʈ �ʱ�ȭ
    }
    private void OnClickCancel()
    {
        GachaPopupPanel.SetActive(false); // ��í �˾� �г� �ݱ�
    }
    private void OnClickComponentsOk()
    {
        ComponentsPanel.SetActive(false); // ������Ʈ �г� �ݱ�
    }
    private void OnClickGachaRecordOk()
    {
        GachaRecordPanel.SetActive(false); // ��í ��� �г� �ݱ�
    }
    private void OnClickTank()
    {
        gameObject.SetActive(false); // ��í UI �ݱ�
        TankPanel.SetActive(true); // ���� ��ũ �г� ����
    }

    private void OnClickOut()
    {
        gameObject.SetActive(false); // ��í UI �ݱ�
        MainMenuPanel.SetActive(true); // ���� �޴� �г� ����
    }
    private void OnClickPreviousPage()
    {
        //To do : ���� �������� �̵��ϴ� ���� ����
    }
    private void OnClickNextPage()
    {
        //To do : ������ �������� �̵��ϴ� ���� ����
    }

}
