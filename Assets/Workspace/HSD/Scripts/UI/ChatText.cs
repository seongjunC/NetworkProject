using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatText : MonoBehaviour
{
    [SerializeField] TMP_Text chat;

    public void SetUp(string message)
    {
        chat.text = message;
    }
}
