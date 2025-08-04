using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static Unity.Burst.Intrinsics.X86.Avx;

public class FastLogin : MonoBehaviour
{
    GameObject InsideRoomPanel;
    [SerializeField] TMP_InputField EmailInputField;
    [SerializeField] TMP_InputField PWInputField;

    [SerializeField] string Id;
    [SerializeField] string Password;

    
    void Start()
    {
        EmailInputField = GameObject.Find("EmailInputField (TMP)").GetComponent<TMP_InputField>();
        PWInputField = GameObject.Find("PWInputField (TMP)").GetComponent<TMP_InputField>();

        EmailInputField.text = Id;
        PWInputField.text = Password;
    }
}
