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

        // �ӼӸ� üũ
        if (text.StartsWith("/w ") || text.StartsWith("/�� "))
        {
            HandleWhisper(text);
        }
        else
        {
            // �Ϲ� ä��
            photonView.RPC(nameof(SendChat), RpcTarget.All, PhotonNetwork.LocalPlayer, PhotonNetwork.NickName, text);
        }

        messageField.text = "";
        messageField.ActivateInputField();
    }

    private void HandleWhisper(string text)
    {
        // /w �г��� �޽���
        string[] parts = text.Split(' ');

        if (parts.Length < 3)   // 3���� ������ ��� �Ǵ�
        {
            AddLocalMessage("�ӼӸ� ����: /w �г��� �޽���");
            return;
        }

        string targetName = parts[1];
        string message = string.Join(" ", parts, 2, parts.Length - 2); // �޽����� ����

        // ��� �÷��̾� ã��
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
            AddLocalMessage($"[�ý���] '{targetName}' �г����� �÷��̾ ã�� �� �����ϴ�.");
            return;
        }

        // ���� ȭ�鿡�� ǥ��
        AddLocalMessage($"[�ӼӸ� �� {targetName}] {message}");

        // �ӼӸ� ����ڿ��Ը� ������
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
            sender = $"{sender}(��)";

        Instantiate(chatPrefab, chatContent).SetUp($"{sender} : {message}");

        Canvas.ForceUpdateCanvases();
    }

    [PunRPC]
    private void ReceiveWhisper(string sender, string message)
    {
        AddLocalMessage($"[�ӼӸ� �� {sender}] {message}");
    }

    public void ResetChat()
    {
        foreach (Transform chat in chatContent)
        {
            Destroy(chat.gameObject);
        }
    }
}
