using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class InputFieldTabManager : MonoBehaviour
{
    [SerializeField] TMP_InputField[] fields;    
    private int idx;

    private void Update()
    {
        if(Tab())
        {
            idx = FindSelectField();

            if (idx == -1) return;

            idx++;

            if (idx >= fields.Length)
                idx = 0;

            fields[idx].ActivateInputField();
        }
    }

    private int FindSelectField()
    {
        for (int i = 0; i < fields.Length; i++)
        {
            if (fields[i].isFocused)
                return i;
        }
        return -1;
    }

    private bool Tab() => Input.GetKeyDown(KeyCode.Tab);
}
