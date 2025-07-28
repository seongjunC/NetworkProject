using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class TestRM : MonoBehaviourPunCallbacks
{
    [Header("버튼")]
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
    // 마스터 서버 접속 성공 시 호출
    public override void OnConnectedToMaster()
    {
        // 타임스탬프나 GUID를 붙여서 방 이름 충돌 방지!
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
