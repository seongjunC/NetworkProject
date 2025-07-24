using Firebase.Auth;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginNetworkManager : MonoBehaviourPunCallbacks
{
    public override void OnConnected()
    {
        base.OnConnected();

        Debug.Log("OnConnected");
    }
}
