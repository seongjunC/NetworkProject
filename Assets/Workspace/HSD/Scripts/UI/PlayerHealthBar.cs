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
        UpdateHelathBar(player._hp);
        player.OnHealthChanged += UpdateHelathBar;
    }

    private void OnDisable()
    {
        player.OnHealthChanged -= UpdateHelathBar;
    }

    private void UpdateHelathBar(float value)
    {
        if(slider.maxValue != player._data.maxHp)
            slider.maxValue = player._data.maxHp;

        slider.value = value;
        healthText.text = $"{Mathf.CeilToInt(value)} / {Mathf.CeilToInt(slider.maxValue)}";
    }
}
