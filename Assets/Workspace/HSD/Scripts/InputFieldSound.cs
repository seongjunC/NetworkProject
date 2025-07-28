using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldSound : MonoBehaviour
{
    public TMP_InputField inputField;

    public AudioClip inputSound;
    public AudioClip deleteSound;

    private string previousText = "";

    void Start()
    {        
        inputField ??= GetComponent<TMP_InputField>();

        previousText = inputField.text;

        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    void OnValueChanged(string newText)
    {
        int diff = newText.Length - previousText.Length;

        if (diff > 0)
        {
            Manager.Audio.PlaySFX(inputSound, Vector3.zero, 1, Random.Range(.7f, 1.1f));
        }
        else
        {
            Manager.Audio.PlaySFX(deleteSound, Vector3.zero, 1);
        }

        previousText = newText;
    }
}
