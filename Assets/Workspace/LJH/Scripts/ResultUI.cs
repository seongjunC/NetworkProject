using System.Collections;
using System.Collections.Generic;
using Game;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private Button okButton;

    [Header("������ �г�")]
    [SerializeField] private GameObject lobbyPanel;

    private Team Winner;
    private List<Player> winnerTeam;
    private List<Player> loserTeam;

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

    public void UpdateResult(Team _winner, List<Player> _winnerTeam, List<Player> _loserTeam)
    {
        Winner = _winner;
        winnerTeam = _winnerTeam;
        loserTeam = _loserTeam;
    }
}
