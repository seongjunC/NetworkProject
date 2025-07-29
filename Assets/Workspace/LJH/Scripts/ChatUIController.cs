using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatUIController : MonoBehaviour
{
    [SerializeField] private GameObject chatPanel;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private CanvasGroup chatCanvasGroup;

    [Header("자동 사라짐 설정")]
    [SerializeField] private float fadeDelay = 3f;
    [SerializeField] private float fadeDuration = 1f;

    private Coroutine fadeCoroutine;
    private bool isTyping = false;

    void Start()
    {
        chatPanel.SetActive(false);
        chatCanvasGroup.alpha = 0f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isTyping)
            {
                OpenChatInput();
            }
            else
            {
                SubmitChat();
            }
        }
    }

    void OpenChatInput()
    {
        chatPanel.SetActive(true);
        chatCanvasGroup.alpha = 1f;
        chatInput.gameObject.SetActive(true);
        chatInput.text = "";
        chatInput.ActivateInputField();
        isTyping = true;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);
    }

    void SubmitChat()
    {
        // 빈 채팅도 닫히게 하기
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            Debug.Log($"[Chat] {chatInput.text}");
            // 로그 추가 코드 가능
        }

        chatInput.DeactivateInputField();
        isTyping = false;

        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeOutAfterDelay());
    }

    IEnumerator FadeOutAfterDelay()
    {
        yield return new WaitForSeconds(fadeDelay);

        float startAlpha = chatCanvasGroup.alpha;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            chatCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, timer / fadeDuration);
            yield return null;
        }

        chatCanvasGroup.alpha = 0f;
        chatPanel.SetActive(false);
    }
}
