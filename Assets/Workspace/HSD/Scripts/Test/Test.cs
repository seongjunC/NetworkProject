using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField] Sprite sprite;
    [SerializeField] Texture2D texture;
    [SerializeField] MapType map;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log(PhotonNetwork.LocalPlayer.UserId);
        }
    }
}
