using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private Transform _spawnPoint;


    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    //  �׽�Ʈ �뵵�̱� ������ ��� ����
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnCreatedRoom() { }

    // �� ���� ��
    public override void OnJoinedRoom()
    {
        Debug.Log("TestNetworkManager : OnJoinedRoom, ���� �Ϸ�");
        //  �г����� �÷��̾� ��ȣ�� ��ü
        PhotonNetwork.LocalPlayer.NickName = $"Player_{PhotonNetwork.LocalPlayer.ActorNumber}";

        // PC ����
        PlayerSpawn();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient == PhotonNetwork.LocalPlayer)
        {
        }
    }

    private IEnumerator MonsterSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
        }
    }

    //  �÷��̾� ����
    private void PlayerSpawn()
    {
        Vector3 spawnPos = _spawnPoint.position;
        PhotonNetwork.Instantiate("Prefabs/Test Tank", spawnPos, Quaternion.identity);
    }
    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log($"TestNetworkManager : OnPlayerEnteredRoom, {player.NickName} ���� �Ϸ�");
    }
}