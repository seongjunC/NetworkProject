using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldSoundManager : MonoBehaviour
{
    public AudioClip inputSound;
    public AudioClip deleteSound;

    private void Start()
    {
        TMP_InputField[] inputFields = FindObjectsOfType<TMP_InputField>(true);

        foreach (var field in inputFields)
        {
            if (field.GetComponent<InputFieldSound>() != null)
                continue;

            var sound = field.gameObject.AddComponent<InputFieldSound>();
            sound.inputSound = inputSound;
            sound.deleteSound = deleteSound;
        }

        InputFieldTabManager[] inputFieldTabManagers = FindObjectsOfType<InputFieldTabManager>();

        foreach (var tabManager in inputFieldTabManagers)
        {
            tabManager.typing = inputSound;
        }
    }
}
