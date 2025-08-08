using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatAnimController : MonoBehaviour
{
    [SerializeField] Chat chat;
    [SerializeField] Animator anim;

    private void Start()
    {
        chat.messageField.onSelect.AddListener(_ => ShowChat(true));
        chat.messageField.onDeselect.AddListener(_ => ShowChat(false));
    }

    private void ShowChat(bool isShow)
    {
        if(isShow)
            anim.SetTrigger("In");  
        else
            anim.SetTrigger("Out");
    }    
}
