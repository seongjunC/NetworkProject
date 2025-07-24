using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button okButton;

    [Header("������ �г�")]
    [SerializeField] private GameObject lobbyPanel;

    // Start is called before the first frame update
    void Start()
    {
        //if (!string.IsNullOrEmpty(UserData.Nickname))
        //{
        //    winnerText.text = $"{UserData.Nickname}!";
        //}
        winnerText.text = "test!";
        okButton.onClick.AddListener(OnClickOK);
    }
    private void OnClickOK()
    {
        gameObject.SetActive(false);         // �ɼ� �г� �ݱ�
        lobbyPanel.SetActive(true);       // ���� �޴� �г� ����
    }
}
