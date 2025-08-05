using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GemPanel : MonoBehaviour
{
    [SerializeField] TMP_Text gemAmount;

    private void OnEnable()
    {
        UpdateGem(Manager.Data.PlayerData.Gem);
        Manager.Data.PlayerData.OnGemChanged += UpdateGem;
    }

    private void OnDisable()
    {
        Manager.Data.PlayerData.OnGemChanged -= UpdateGem;
    }

    private void UpdateGem(int amount)
    {
        gemAmount.text = amount.ToString();
    }
}
