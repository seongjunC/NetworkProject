using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewordSlot : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text rewordText;

    public void SetUp(Sprite itemIcon, string itemName, int amount, Rank rank = Rank.S)
    {
        icon.sprite = itemIcon;
        rewordText.color = Utils.GetColor(rank);
        rewordText.text = $"{itemName}x{amount}";
    }
}
