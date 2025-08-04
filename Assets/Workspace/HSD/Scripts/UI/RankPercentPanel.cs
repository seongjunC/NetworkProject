using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class RankPercentPanel : MonoBehaviour
{
    [SerializeField] TMP_Text percentText;
    [SerializeField] TMP_Text rankText;

    public void SetUp(float percent, Rank rank, VertexGradient gradient)
    {
        percentText.text = $"{percent.ToString("F5")}%";

        rankText.text = rank.ToString();
        rankText.colorGradient = gradient;
    }
}
