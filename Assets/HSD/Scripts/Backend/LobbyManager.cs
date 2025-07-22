using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Button logOutButton;

    private void OnEnable()
    {
        logOutButton.onClick.AddListener(Manager.Firebase.LogOut);
    }
}
