using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chat : MonoBehaviourPun
{
    [SerializeField] TMP_InputField messageField;
    [SerializeField] Transform chatContent;
    [SerializeField] ChatText chatPrefab;

    void Start()
    {
        messageField.onEndEdit.AddListener(Chating);
    }

    public void Chating()
    {
        photonView.RPC(nameof(SendChating), RpcTarget.All, PhotonNetwork.NickName, messageField.text);
        messageField.text = "";
        messageField.ActivateInputField();
    }

    private void Chating(string message)
    {
        if (!Input.GetKeyDown(KeyCode.Return)) return;

        Debug.Log($"닉네임 : {PhotonNetwork.NickName}");
        Debug.Log($"네임 필드 : {messageField.text}");
        if (messageField.text == "") return;

        photonView.RPC(nameof(SendChating), RpcTarget.All, PhotonNetwork.NickName, messageField.text);
        messageField.text = "";
        messageField.ActivateInputField();
    }

    public void ResetChat()
    {
        foreach(Transform chat in chatContent)
        {
            Destroy(chat.gameObject);
        }
    }
    [PunRPC]
    private void SendChating(string sender, string message)
    {
        Instantiate(chatPrefab, chatContent).SetUp($"{sender} : {message}");
        Canvas.ForceUpdateCanvases();
    }
}
