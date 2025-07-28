using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private Button outButton;

    [Header("연결할 패널")]
    [SerializeField] private GameObject mainmenuPanel;
    // Start is called before the first frame update
    void Start()
    {
        outButton.onClick.AddListener(OnClickOut);

    }
    private void OnClickOut()
    {
        gameObject.SetActive(false);         // 업적 패널 닫기
        mainmenuPanel.SetActive(true);       // 메인 메뉴 패널 열기
    }

}
