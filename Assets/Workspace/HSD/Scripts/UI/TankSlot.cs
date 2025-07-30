using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TankSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image tankIcon;
    [SerializeField] Image tankRankImage;
    [SerializeField] TankData tankData;
    private TankToolTip tankToolTip;

    public void SetUp(TankData data, TankToolTip tooltip, Color slotColor)
    {
        tankData = data;
        tankToolTip = tooltip;

        tankIcon.sprite = tankData.icon;
        tankRankImage.color = slotColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tankData == null) return;

        tankToolTip.ShowToolTip(tankData);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tankToolTip.CloseToolTip();
    }    
}
