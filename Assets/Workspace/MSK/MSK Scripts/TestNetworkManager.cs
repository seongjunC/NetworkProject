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
        // �̱��� �ߺ� ����
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

    // �� ���� ��
    public override void OnJoinedRoom()
    {
        Debug.Log("TestNetworkManager : OnJoinedRoom, ���� �Ϸ�");
        //  �г����� �÷��̾� ��ȣ�� ��ü
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
        Debug.Log($"TestNetworkManager : OnPlayerEnteredRoom, {player.NickName} ���� �Ϸ�");
    }
}