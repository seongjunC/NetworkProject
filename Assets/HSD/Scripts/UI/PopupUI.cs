using System.Collections;
using TMPro;
using UnityEngine;

public class PopUpUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI message;

    private Coroutine showRoutine;
    private YieldInstruction delay;

    private void Awake()
    {
        delay = new WaitForSeconds(1f);
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
        yield return StartCoroutine(Utils.Fade(message, 0, 1));
        yield return delay;
        yield return StartCoroutine(Utils.Fade(message, 1, 0));

        gameObject.SetActive(false);
    }
}
