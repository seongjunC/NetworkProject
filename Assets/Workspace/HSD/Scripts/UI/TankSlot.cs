using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TankSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [SerializeField] Image tankIcon;
    [SerializeField] Image tankRankImage;
    [SerializeField] TankData tankData;
    private TankToolTip tankToolTip;
    private Color color;

    public void SetUp(TankData data, TankToolTip tooltip, Color slotColor)
    {
        tankData = data;
        tankToolTip ??= tooltip;

        tankIcon.sprite = tankData.Icon;
        
        color = slotColor;
        tankRankImage.color = slotColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tankData == null || tankToolTip == null) return;
        Vector2 pos = (Vector2)transform.position;

        tankToolTip.ShowToolTip(tankData, color, pos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tankToolTip == null) return;
        tankToolTip.CloseToolTip();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (tankData == null) return;

        Manager.Data.TankDataController.SetSelectTank(tankData);
    }
}
