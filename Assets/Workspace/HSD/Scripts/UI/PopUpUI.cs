using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI message;
    [SerializeField] Image image;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void Show(string message, Color? color = null)
    {
        if (color != null)
            this.message.color = color.Value;

        gameObject.SetActive(true);
        this.message.text = message;

        anim.SetTrigger("In");
    }
}
