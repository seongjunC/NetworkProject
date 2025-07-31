using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] Image image;

    private Coroutine showRoutine;
    private YieldInstruction delay;

    private void Awake()
    {
        delay = new WaitForSeconds(.7f);
    }

    public void Show(string message, Color? color = null)
    {
        if (color != null)
            this.message.color = color.Value;

        gameObject.SetActive(true);
        this.message.text = message;

        if(showRoutine != null)
        {
            StopCoroutine(showRoutine);
            showRoutine = null;
        }
        showRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        StartCoroutine(Utils.Fade(image, 0, .9f));
        yield return StartCoroutine(Utils.Fade(message, 0, 1));
        yield return delay;
        StartCoroutine(Utils.Fade(image, .9f, 0));
        yield return StartCoroutine(Utils.Fade(message, 1, 0));
        message.color = Color.white;
        gameObject.SetActive(false);
    }
}
