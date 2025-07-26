using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpUI_Action : MonoBehaviour
{
    [SerializeField] TMP_Text message;
    [SerializeField] Button yes;
    [SerializeField] Button no;

    private event Action yesAction;
    private event Action noAction;

    private void Start()
    {
        yes.onClick.AddListener(Yes);
        no.onClick.AddListener(No);
    }

    public void Show(string _message, Action _yes = null, Action _no = null)
    {       
        yesAction = _yes;
        noAction = _no;
        yesAction += Close;
        noAction += Close;
        message.text = _message;

        gameObject.SetActive(true);
    }

    private void Yes() => yesAction?.Invoke();
    private void No() => noAction?.Invoke();
    private void Close() => gameObject.SetActive(false);
}
