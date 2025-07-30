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
    private RectTransform rectTransform;
    private TankToolTip tankToolTip;
    private Color color;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetUp(TankData data, TankToolTip tooltip, Color slotColor)
    {
        tankData = data;
        tankToolTip = tooltip;

        tankIcon.sprite = tankData.icon;
        
        color = slotColor;
        tankRankImage.color = slotColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tankData == null) return;
        Vector2 pos = (Vector2)transform.position + new Vector2(-rectTransform.rect.width / 2, rectTransform.rect.height / 2);

        tankToolTip.ShowToolTip(tankData, color, pos);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tankToolTip.CloseToolTip();
    }    
}
