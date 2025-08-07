using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] PlayerController player;
    [SerializeField] Slider slider;
    [SerializeField] TMP_Text healthText;

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
