using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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
        var props = new Hashtable
        {
            {"Map", 2}
        };
        var options = new RoomOptions
        {
            MaxPlayers = 4,
            CustomRoomProperties = props,
        };
        PhotonNetwork.JoinOrCreateRoom("PSHTestRoom", options, TypedLobby.Default);
    }
    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Test Battle");
    }
}
