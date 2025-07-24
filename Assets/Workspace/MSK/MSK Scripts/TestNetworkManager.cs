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

    //  테스트 용도이기 때문에 즉시 연결
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnCreatedRoom() { }

    // 방 입장 시
    public override void OnJoinedRoom()
    {
        Debug.Log("TestNetworkManager : OnJoinedRoom, 입장 완료");
        //  닉네임을 플레이어 번호로 대체
        PhotonNetwork.LocalPlayer.NickName = $"Player_{PhotonNetwork.LocalPlayer.ActorNumber}";

        // PC 스폰
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

    //  플레이어 스폰
    private void PlayerSpawn()
    {
        Vector3 spawnPos = _spawnPoint.position;
        PhotonNetwork.Instantiate("Prefabs/Test Tank", spawnPos, Quaternion.identity);
    }
    public override void OnPlayerEnteredRoom(Player player)
    {
        Debug.Log($"TestNetworkManager : OnPlayerEnteredRoom, {player.NickName} 입장 완료");
    }
}