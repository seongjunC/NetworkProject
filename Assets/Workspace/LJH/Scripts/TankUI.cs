using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankUI : MonoBehaviour
{
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button outButton;

    [Header("연결할 패널")]
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
        gameObject.SetActive(false);         // 탱크 패널 닫기
        gachaPanel.SetActive(true);         // 가챠 패널 열기
    }
    private void OnClickOut()
    {
        gameObject.SetActive(false);         // 탱크 패널 닫기
        mainMenuPanel.SetActive(true);       // 메인 메뉴 패널 열기
    }
}
