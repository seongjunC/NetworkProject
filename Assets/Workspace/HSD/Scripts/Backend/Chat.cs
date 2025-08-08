using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Chat : MonoBehaviourPun
{
    public static bool isChating;
    public TMP_InputField messageField;
    [SerializeField] Transform chatContent;
    [SerializeField] ChatText chatPrefab;
    [SerializeField] TMP_Text chatTypeText;

    [Header("Color")]
    [SerializeField] Color whisperColor;
    [SerializeField] Color teamChatColor;

    private ChatType chatType = ChatType.All;
    private string beforeMessage;
    private string beforeWhisperPlayerName;

    void Start()
    {
        messageField.onEndEdit.AddListener(OnMessageSubmitted);
    }

    private void OnEnable()
    {
        UpdateChatTypeText();
        messageField.onSelect.AddListener((_) => isChating = true);
        messageField.onDeselect.AddListener((_) => isChating = false);
    }

    private void OnDisable()
    {
        messageField.onSelect.RemoveAllListeners();
        messageField.onDeselect.RemoveAllListeners();
        isChating = false;
    }

    private void Update()
    {
        // ������ �޽��� �ҷ�����
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!string.IsNullOrEmpty(beforeMessage))
            {
                messageField.text = beforeMessage;
                messageField.caretPosition = messageField.text.Length; // Ŀ�� �� ������
                messageField.ActivateInputField(); // �ٽ� ��Ŀ��
            }
        }

        if (messageField.isFocused && messageField.text.StartsWith("/r") && Input.GetKeyDown(KeyCode.Space))
        {
            if (!string.IsNullOrEmpty(beforeWhisperPlayerName))
            {
                messageField.text = $"/�� {beforeWhisperPlayerName} ";
                messageField.caretPosition = messageField.text.Length;
            }
        }

        if(Input.GetKeyDown(KeyCode.Return))
        {
            messageField.ActivateInputField();
        }

        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(chatType == ChatType.All)
            {
                chatType = ChatType.Team;                
            }
            else
            {
                chatType = ChatType.All;                
            }

            UpdateChatTypeText();
        }
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
            photonView.RPC(nameof(SendChat), RpcTarget.All, PhotonNetwork.LocalPlayer, PhotonNetwork.NickName, text, chatType);
        }

        beforeMessage = messageField.text;
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

        beforeWhisperPlayerName = targetName;

        // ���� ȭ�鿡�� ǥ��
        AddLocalMessage($"[�ӼӸ� �� {targetName}] {message}");

        // �ӼӸ� ����ڿ��Ը� ������
        photonView.RPC(nameof(ReceiveWhisper), targetPlayer, PhotonNetwork.NickName, message);
    }

    private void AddLocalMessage(string msg)
    {
        Instantiate(chatPrefab, chatContent).SetUp(msg, whisperColor);
    }

    private void UpdateChatTypeText()
    {
        chatTypeText.text = chatType == ChatType.Team ? "��" : "��ü";
    }

    [PunRPC]
    private void SendChat(Player senderPlayer, string sender, string message, ChatType type)
    {
        if (PhotonNetwork.LocalPlayer == senderPlayer)
            sender = $"{sender}(��)";
        
        string prefix = type == ChatType.Team ? "[��]" : "[��ü]";
        sender = $"{prefix} {sender}";

        if (type == ChatType.Team)
        {
            if(senderPlayer.GetTeam() == PhotonNetwork.LocalPlayer.GetTeam())
            {
                Instantiate(chatPrefab, chatContent).SetUp($"{sender} : {message}", teamChatColor);
            }
        }
        else
        {
            Instantiate(chatPrefab, chatContent).SetUp($"{sender} : {message}");
        }        

        Canvas.ForceUpdateCanvases();
    }

    [PunRPC]
    private void ReceiveWhisper(string sender, string message)
    {
        AddLocalMessage($"[�ӼӸ� �� {sender}] {message}");

        beforeWhisperPlayerName = sender;
    }        

    public void ResetChat()
    {
        foreach (Transform chat in chatContent)
        {
            Destroy(chat.gameObject);
        }
    }
}
