using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaResultSlot : MonoBehaviour
{
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text tankNameText;
    [SerializeField] Image tankIcon;

    public void SetUp(GachaResult data)
    {
        TankData tankData = data.GetTankData();

        timeText.text = data.GetFormattedTime();
        tankNameText.text = tankData.tankMetaName;
        tankNameText.color = Utils.GetColor(tankData.rank);

        tankIcon.sprite = tankData.Icon;
    }
}
