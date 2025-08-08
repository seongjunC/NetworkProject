using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankToolTip : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    [SerializeField] Image tankIcon;

    [Header("UpgradeCount")]
    [SerializeField] Slider upgradeSlider;
    [SerializeField] TMP_Text upgradeCountText;

    [Header("Texts")]
    [SerializeField] TMP_Text tankNameText;
    [SerializeField] TMP_Text rankText;
    [SerializeField] TMP_Text levelText;
    [SerializeField] TMP_Text damageText;
    [SerializeField] TMP_Text maxMoveText;
    [SerializeField] TMP_Text hpText;

    [SerializeField] bool isMousePos;
    [Header("Offset_Limit")]
    [SerializeField] private float xLimit = 960;
    [SerializeField] private float yLimit = 540;
    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 150;

    public void ShowToolTip(TankData data, Color rankColor, Vector2 pos)
    {
        AdjustPosition(pos);

        gameObject.SetActive(true);

        data.InitStat();

        // 스텟
        hpText.text = data.maxHp.ToString("F0");
        damageText.text = data.damage.ToString("F0");
        maxMoveText.text = data.maxMove.ToString("F0");

        // 데이터
        levelText.text = $"Lv.{data.Level.ToString()}";
        rankText.text = data.rank.ToString();
        rankText.color = rankColor;
        tankIcon.sprite = data.Icon;
        tankNameText.text = data.tankMetaName;

        // 업그레이드
        int required = data.GetRequiredCountForNextLevel();
        int progress = data.GetProgressTowardsNextLevel();

        upgradeCountText.text = $"{progress} / {required}";
        upgradeSlider.maxValue = required;
        upgradeSlider.value = progress;   
    }

    public void CloseToolTip()
    {
        gameObject.SetActive(false);
    }


    public virtual void AdjustPosition(Vector2 screenPos)
    {
        Vector2 offset = Vector2.zero;
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);

        offset.x = (screenPos.x > screenSize.x / 2f) ? -xOffset : xOffset;
        offset.y = (screenPos.y > screenSize.y / 2f) ? -yOffset : yOffset;

        Vector2 finalScreenPos = screenPos + offset;

        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        RectTransform tooltipRect = GetComponent<RectTransform>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            finalScreenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out Vector2 localPos
        );

        tooltipRect.anchoredPosition = localPos;
    }
}
