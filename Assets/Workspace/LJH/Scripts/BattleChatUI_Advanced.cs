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
    [SerializeField] private GameObject chatPanel;           // ChatPanel ��ü
    [SerializeField] private CanvasGroup chatCanvasGroup;    // FadeOut ����
    [SerializeField] private TextMeshProUGUI TeamTypeText; // �� Ÿ�� ǥ�ÿ� �ؽ�Ʈ
    [SerializeField] private TMP_InputField chatInputField;  // �Է�â
    [SerializeField] private ScrollRect scrollRect;          // ScrollView
    [SerializeField] private Transform contentParent;        // Content ������Ʈ
    [SerializeField] private GameObject chatMessagePrefab;   // �޽��� ������

    [Header("Settings")]
    [SerializeField] private float fadeDelay = 3f;
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine fadeCoroutine;
    private bool isTyping = false;

    // �׽�Ʈ�� �÷��̾� ����
    private string playerName = "Player1";
    private TeamType myTeam = TeamType.Ally;

    // ��ä�� ��� ����ġ
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
        // Enter�� ���� �ݱ�
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isTyping)
                OpenChatInput();
            else
                SubmitChat();
        }

        // Tab���� ä�� ��� ��ȯ
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            isTeamChatMode = !isTeamChatMode;
            TeamTypeText.text = isTeamChatMode ? "��ä��" : "��üä��";
            Debug.Log("ä�� ���: " + (isTeamChatMode ? "��ä��" : "��üä��"));
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

        // �Է��� �� �������� ó��
        if (!string.IsNullOrWhiteSpace(msg))
        {
            TeamType targetTeam = isTeamChatMode ? myTeam : TeamType.ALL;
            ChatMessage chatMsg = new ChatMessage(playerName, msg, targetTeam);

            ReceiveChatMessage(chatMsg); // ��Ʈ��ũ ���� ��� ���� ���� �ùķ��̼�
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

        string prefix = chatMsg.team == TeamType.Ally ? "[��]" : "[��ü]";
        text.text = $"{prefix} {chatMsg.senderName}: {chatMsg.message}";

        // ���� ����
        if (chatMsg.team == TeamType.Ally)
            text.color = Color.green;
        else
            text.color = Color.red;

        chatMessages.Add(go);

        // �ڵ� ��ũ��
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