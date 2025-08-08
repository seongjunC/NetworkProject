using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProbabilitySlot : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text percentText;

    public void SetUp(TankData data, float percent)
    {
        icon.sprite = data.Icon;
        nameText.text = data.tankMetaName;
        percentText.text = $"{percent.ToString("F5")}%";
    }
}
