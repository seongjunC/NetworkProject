using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Chat : MonoBehaviourPun
{
    [SerializeField] TMP_InputField messageField;
    [SerializeField] Transform chatContent;
    [SerializeField] ChatText chatPrefab;

    private ChatType chatType = ChatType.All;

    [Header("Color")]
    [SerializeField] Color whisperColor;

    void Start()
    {
        messageField.onEndEdit.AddListener(OnMessageSubmitted);
    }

    private void OnMessageSubmitted(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;

        // 귓속말 체크
        if (text.StartsWith("/w ") || text.StartsWith("/귓 "))
        {
            HandleWhisper(text);
        }
        else
        {
            // 일반 채팅
            photonView.RPC(nameof(SendChat), RpcTarget.All, PhotonNetwork.LocalPlayer, PhotonNetwork.NickName, text);
        }

        messageField.text = "";
        messageField.ActivateInputField();
    }

    private void HandleWhisper(string text)
    {
        // /w 닉네임 메시지
        string[] parts = text.Split(' ');

        if (parts.Length < 3)   // 3개로 나눠서 양식 판단
        {
            AddLocalMessage("귓속말 형식: /w 닉네임 메시지");
            return;
        }

        string targetName = parts[1];
        string message = string.Join(" ", parts, 2, parts.Length - 2); // 메시지만 골라넴

        // 대상 플레이어 찾기
        Player targetPlayer = null;
        foreach (var p in PhotonNetwork.PlayerList)
        {
            if (p.NickName == targetName)
            {
                targetPlayer = p;
                break;
            }
        }

        if (targetPlayer == null)
        {
            AddLocalMessage($"[시스템] '{targetName}' 닉네임의 플레이어를 찾을 수 없습니다.");
            return;
        }

        // 본인 화면에도 표시
        AddLocalMessage($"[귓속말 → {targetName}] {message}");

        // 귓속말 대상자에게만 보내기
        photonView.RPC(nameof(ReceiveWhisper), targetPlayer, PhotonNetwork.NickName, message);
    }

    private void AddLocalMessage(string msg)
    {
        Instantiate(chatPrefab, chatContent).SetUp(msg, whisperColor);
    }

    [PunRPC]
    private void SendChat(Player senderPlayer, string sender, string message)
    {
        if (PhotonNetwork.LocalPlayer == senderPlayer)
            sender = $"{sender}(나)";

        Instantiate(chatPrefab, chatContent).SetUp($"{sender} : {message}");

        Canvas.ForceUpdateCanvases();
    }

    [PunRPC]
    private void ReceiveWhisper(string sender, string message)
    {
        AddLocalMessage($"[귓속말 ← {sender}] {message}");
    }

    public void ResetChat()
    {
        foreach (Transform chat in chatContent)
        {
            Destroy(chat.gameObject);
        }
    }
}
