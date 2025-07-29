using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankUI : MonoBehaviour
{
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button outButton;
    [SerializeField] private Button promotionButton;

    [Header("������ �г�")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gachaPanel;
    [SerializeField] private GameObject promotionPanel;
    // Start is called before the first frame update
    void Start()
    {
        gachaButton.onClick.AddListener(OnClickGacha);
        outButton.onClick.AddListener(OnClickOut);
        promotionButton.onClick.AddListener(OnClickPromotion);
    }

    private void OnClickGacha()
    {
        gameObject.SetActive(false);         // ��ũ �г� �ݱ�
        gachaPanel.SetActive(true);         // ��í �г� ����
    }
    private void OnClickOut()
    {
        gameObject.SetActive(false);         // ��ũ �г� �ݱ�
        mainMenuPanel.SetActive(true);       // ���� �޴� �г� ����
    }
    private void OnClickPromotion()
    {
        gameObject.SetActive(false);         // ��ũ �г� �ݱ�
        promotionPanel.SetActive(true);      // ���θ�� �г� ����
    }
}
