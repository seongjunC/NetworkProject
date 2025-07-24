using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOptionUI : MonoBehaviour
{
    [SerializeField] private Button saveButton;
    [SerializeField] private Button cancelButton;

    [Header("������ �г�")]
    [SerializeField] private GameObject mainmenuPanel;

    // Start is called before the first frame update
    void Start()
    {
        // ����Ǿ��ִ� ���� �ɼ� �ʱ�ȭ �۾�
    }

    private void Awake()
    {
        saveButton.onClick.AddListener(OnClickSave);
        cancelButton.onClick.AddListener(OnClickCancel);
    }
    private void OnClickSave()
    {
        //To do : ���� �ɼ� ���� ���� ����
    }
    private void OnClickCancel()
    {
        gameObject.SetActive(false);         // �ɼ� �г� �ݱ�
        mainmenuPanel.SetActive(true);       // ���� �޴� �г� ����
    }
}
