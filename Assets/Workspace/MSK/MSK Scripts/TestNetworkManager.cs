using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNetworkManager : MonoBehaviourPunCallbacks
{
    public static TestNetworkManager Instance { get; private set; }

    [SerializeField] private string _roomName;

    private void Awake()
    {
        // 싱글톤 중복 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom(_roomName, new RoomOptions(), TypedLobby.Default);
    }
    
    public override void OnCreatedRoom() { }

    // 방 입장 시
    public override void OnJoinedRoom()
    {
        Debug.Log("TestNetworkManager : OnJoinedRoom, 입장 완료");
        //  닉네임을 플레이어 번호로 대체
        PhotonNetwork.LocalPlayer.NickName = $"Player_{PhotonNetwork.LocalPlayer.ActorNumber}";
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
        }
    }


    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log($"TestNetworkManager : OnPlayerEnteredRoom, {player.NickName} 입장 완료");
    }
}