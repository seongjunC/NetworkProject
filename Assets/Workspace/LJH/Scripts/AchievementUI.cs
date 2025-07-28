using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    [SerializeField] private Button outButton;

    [Header("������ �г�")]
    [SerializeField] private GameObject mainmenuPanel;
    // Start is called before the first frame update
    void Start()
    {
        outButton.onClick.AddListener(OnClickOut);

    }
    private void OnClickOut()
    {
        gameObject.SetActive(false);         // ���� �г� �ݱ�
        mainmenuPanel.SetActive(true);       // ���� �޴� �г� ����
    }

}
