using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class FastLogin : MonoBehaviour
{
    GameObject InsideRoomPanel;
    [SerializeField] TMP_InputField EmailInputField;
    [SerializeField] TMP_InputField PWInputField;

    [SerializeField] string Id;
    [SerializeField] string Password;

    IEnumerator Start()
    {
        yield return new WaitForSeconds(.1f);
        EmailInputField.text = Id;
        PWInputField.text = Password;
    }
}
