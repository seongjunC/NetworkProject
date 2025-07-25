using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class MSKTestRoomManager : MonoBehaviour
{
    [Header("¹öÆ°")]
    [SerializeField] private Button startButton;
    private void Start()
    {
        startButton.onClick.AddListener(GameStart);
    }
    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("MSK Test Scene");
    }
}
