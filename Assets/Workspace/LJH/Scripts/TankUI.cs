using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankUI : MonoBehaviour
{
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button outButton;

    [Header("������ �г�")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gachaPanel;
    // Start is called before the first frame update
    void Start()
    {
        gachaButton.onClick.AddListener(OnClickGacha);
        outButton.onClick.AddListener(OnClickOut);

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
}
