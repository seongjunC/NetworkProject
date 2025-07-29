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
    [Header("버튼")]
    [SerializeField] private Button startButton;
    [SerializeField] private Button createButton;
    [SerializeField] private int index;
    private void Start()
    {
        startButton.onClick.AddListener(GameStart);
        createButton.onClick.AddListener(()=>CreateRoomWithMap(index));
    }
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
    // 마스터 서버 접속 성공 시 호출
    private void CreateRoomWithMap(int mapIndex)
    {
        var props = new Hashtable
        {
            {"Map", mapIndex}
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
