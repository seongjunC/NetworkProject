using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] TMP_Text text;

    private void Update()
    {
        text.text = PhotonNetwork.NetworkClientState.ToString();
    }
}
