using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class TestRM : MonoBehaviourPunCallbacks
{
    [Header("��ư")]
    [SerializeField] private Button startButton;
    private void Start()
    {
        startButton.onClick.AddListener(GameStart);
    }
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
    // ������ ���� ���� ���� �� ȣ��
    public override void OnConnectedToMaster()
    {
        // Ÿ�ӽ������� GUID�� �ٿ��� �� �̸� �浹 ����!
        string roomName = "PSHTestRoom";
        PhotonNetwork.JoinOrCreateRoom(
            roomName,
            new RoomOptions { MaxPlayers = 4 },
            TypedLobby.Default
        );
    }
    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Test Battle");
    }
}
