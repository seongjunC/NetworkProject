using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField inputPassword;
    [SerializeField] Button applyButton;
    [SerializeField] Button closeButton;
    private string password;
    private string roomName;

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
    }

    private void OnEnable()
    {
        inputPassword.text = "";
        applyButton.onClick.AddListener(JoinRoomCheck);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            JoinRoomCheck();

        if (Input.GetKeyDown(KeyCode.Escape))
            Close();
    }

    public void SetUp(RoomInfo room)
    {
        roomName = room.Name;
        password = (string)room.CustomProperties["Password"];
    }

    private void JoinRoomCheck()
    {
        if (PasswordCheck())
        {
            applyButton.onClick.RemoveListener(JoinRoomCheck);            
            StartCoroutine(JoinRoomRoutine());
        }
        else
            Manager.UI.PopUpUI.Show("비밀번호가 일치하지 않습니다.");
    }

    private bool PasswordCheck()
    {
        return password == inputPassword.text ? true : false;
    }

    private void Close()
    {
        applyButton.onClick.RemoveListener(JoinRoomCheck);
        gameObject.SetActive(false);
    }
    private IEnumerator JoinRoomRoutine()
    {
        Manager.UI.FadeScreen.FadeIn(.5f);

        yield return new WaitForSeconds(.5f);

        PhotonNetwork.JoinRoom(roomName);

        yield return new WaitForSeconds(.5f);

        Manager.UI.FadeScreen.FadeOut(.5f);
        gameObject.SetActive(false);
    }
}
