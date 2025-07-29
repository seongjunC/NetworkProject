using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TestResult : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button okButton;

    [Header("������ �г�")]
    [SerializeField] private GameObject lobbyPanel;

    // Start is called before the first frame update
    void Start()
    {
        //if (!string.IsNullOrEmpty(PhotonNetwork.NickName))
        //{
        //   winnerText.text = $"{PhotonNetwork.Nickname}!";
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
