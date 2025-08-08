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
        // 마지막 메시지 불러오기
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!string.IsNullOrEmpty(beforeMessage))
            {
                messageField.text = beforeMessage;
                messageField.caretPosition = messageField.text.Length; // 커서 맨 끝으로
                messageField.ActivateInputField(); // 다시 포커스
            }
        }

        if (messageField.isFocused && messageField.text.StartsWith("/r") && Input.GetKeyDown(KeyCode.Space))
        {
            if (!string.IsNullOrEmpty(beforeWhisperPlayerName))
            {
                messageField.text = $"/귓 {beforeWhisperPlayerName} ";
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

        // 귓속말 체크
        if (text.StartsWith("/w ") || text.StartsWith("/귓 "))
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

        beforeWhisperPlayerName = targetName;

        // 본인 화면에도 표시
        AddLocalMessage($"[귓속말 → {targetName}] {message}");

        // 귓속말 대상자에게만 보내기
        photonView.RPC(nameof(ReceiveWhisper), targetPlayer, PhotonNetwork.NickName, message);
    }

    private void AddLocalMessage(string msg)
    {
        Instantiate(chatPrefab, chatContent).SetUp(msg, whisperColor);
    }

    private void UpdateChatTypeText()
    {
        chatTypeText.text = chatType == ChatType.Team ? "팀" : "전체";
    }

    [PunRPC]
    private void SendChat(Player senderPlayer, string sender, string message, ChatType type)
    {
        if (PhotonNetwork.LocalPlayer == senderPlayer)
            sender = $"{sender}(나)";
        
        string prefix = type == ChatType.Team ? "[팀]" : "[전체]";
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
        AddLocalMessage($"[귓속말 ← {sender}] {message}");

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
