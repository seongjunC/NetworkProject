using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public enum TeamType
{
    Ally,
    ALL
}

[System.Serializable]
public class ChatMessage
{
    public string senderName;
    public string message;
    public TeamType team;

    public ChatMessage(string sender, string msg, TeamType teamType)
    {
        senderName = sender;
        message = msg;
        team = teamType;
    }
}

public class BattleChatUI_Advanced : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject chatPanel;           // ChatPanel 전체
    [SerializeField] private CanvasGroup chatCanvasGroup;    // FadeOut 제어
    [SerializeField] private TextMeshProUGUI TeamTypeText; // 팀 타입 표시용 텍스트
    [SerializeField] private TMP_InputField chatInputField;  // 입력창
    [SerializeField] private ScrollRect scrollRect;          // ScrollView
    [SerializeField] private Transform contentParent;        // Content 오브젝트
    [SerializeField] private GameObject chatMessagePrefab;   // 메시지 프리팹

    [Header("Settings")]
    [SerializeField] private float fadeDelay = 3f;
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine fadeCoroutine;
    private bool isTyping = false;

    // 테스트용 플레이어 정보
    private string playerName = "Player1";
    private TeamType myTeam = TeamType.Ally;

    // 팀채팅 모드 스위치
    private bool isTeamChatMode = true;

    private List<GameObject> chatMessages = new List<GameObject>();

    void Start()
    {
        Debug.Log(gameObject.name);
        chatPanel.SetActive(false);
        chatCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        // Enter로 열고 닫기
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isTyping)
                OpenChatInput();
            else
                SubmitChat();
        }

        // Tab으로 채팅 모드 전환
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTeamChatMode = !isTeamChatMode;
            TeamTypeText.text = isTeamChatMode ? "팀채팅" : "전체채팅";
            Debug.Log("채팅 모드: " + (isTeamChatMode ? "팀채팅" : "전체채팅"));
        }
    }

    void OpenChatInput()
    {
        chatPanel.SetActive(true);
        chatCanvasGroup.alpha = 1f;
        chatInputField.gameObject.SetActive(true);
        chatInputField.text = "";
        chatInputField.ActivateInputField();
        isTyping = true;
        chatInputField.colors = new ColorBlock
        {
            normalColor = Color.white,
            highlightedColor = Color.white,
            pressedColor = Color.white,
            selectedColor = Color.white,
            disabledColor = Color.gray,
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
        

    }

    void SubmitChat()
    {
        string msg = chatInputField.text;

        // 입력이 비어도 닫히도록 처리
        if (!string.IsNullOrWhiteSpace(msg))
        {
            TeamType targetTeam = isTeamChatMode ? myTeam : TeamType.ALL;
            ChatMessage chatMsg = new ChatMessage(playerName, msg, targetTeam);

            ReceiveChatMessage(chatMsg); // 네트워크 전송 대신 로컬 수신 시뮬레이션
        }

        chatInputField.DeactivateInputField();
        isTyping = false;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutAfterDelay());
        chatInputField.text = "";
        chatInputField.colors = new ColorBlock
        {
            normalColor = Color.white,
            highlightedColor = Color.white,
            pressedColor = Color.white,
            selectedColor = Color.gray,
            disabledColor = Color.gray,
            colorMultiplier = 1f,
            fadeDuration = 0.1f
        };

    }

    public void ReceiveChatMessage(ChatMessage chatMsg)
    {
        GameObject go = Instantiate(chatMessagePrefab, contentParent);
        TMP_Text text = go.GetComponent<TMP_Text>();

        string prefix = chatMsg.team == TeamType.Ally ? "[팀]" : "[전체]";
        text.text = $"{prefix} {chatMsg.senderName}: {chatMsg.message}";

        // 팀별 색상
        if (chatMsg.team == TeamType.Ally)
            text.color = Color.green;
        else
            text.color = Color.red;

        chatMessages.Add(go);

        // 자동 스크롤
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    IEnumerator FadeOutAfterDelay()
    {
        yield return new WaitForSeconds(fadeDelay);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            chatCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        chatCanvasGroup.alpha = 0f;
        chatPanel.SetActive(false);
    }
}