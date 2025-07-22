using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Button logOutButton;
    [SerializeField] GameObject nickNameSelectPanel;

    public override void OnEnable()
    {
        base.OnEnable();

        if(Manager.Data.PlayerData.Name == "")
            nickNameSelectPanel.SetActive(true);
    }
}
