using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankToolTip : MonoBehaviour
{
    [SerializeField] Image tankIcon;

    [Header("UpgradeCount")]
    [SerializeField] Slider upgradeSlider;
    [SerializeField] TMP_Text upgradeCountText;

    [Header("Texts")]
    [SerializeField] TMP_Text tankNameText;
    [SerializeField] TMP_Text rankText;    
    [SerializeField] TMP_Text damageText;
    [SerializeField] TMP_Text maxMoveText;
    [SerializeField] TMP_Text hpText;

    [SerializeField] bool isMousePos;
    [Header("Offset_Limit")]
    [SerializeField] private float xLimit = 960;
    [SerializeField] private float yLimit = 540;
    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 150;

    private void Start()
    {
        upgradeSlider.maxValue = Manager.Data.TankInventoryData.needUpgradeCount;
    }

    public void ShowToolTip(TankData data, Color rankColor, Vector2 pos)
    {
        AdjustPosition(pos);

        gameObject.SetActive(true);

        // ����
        hpText.text = data.maxHp.ToString();
        damageText.text = data.damage.ToString();
        maxMoveText.text = data.maxMove.ToString();

        // ������
        rankText.text = data.rank.ToString();
        rankText.color = rankColor;
        tankIcon.sprite = data.icon;
        tankNameText.text = data.tankName;

        // ���׷��̵�
        upgradeCountText.text = $"{data.count} / {Manager.Data.TankInventoryData.needUpgradeCount}";
        upgradeSlider.value = data.count;
    }

    public void CloseToolTip()
    {
        gameObject.SetActive(false);
    }
    

    public virtual void AdjustPosition(Vector2 _pos)
    {
        Vector2 pos = Vector2.zero;
        if (isMousePos)
            pos = Input.mousePosition;
        else
            pos = _pos;


        float newXoffset = 0;
        float newYoffset = 0;

        if (pos.x > xLimit)
            newXoffset = -xOffset;
        else
            newXoffset = xOffset;

        if (pos.y > yLimit)
            newYoffset = -yOffset;
        else
            newYoffset = yOffset;

        transform.position = new Vector2(pos.x + newXoffset, pos.y + newYoffset);
    }
}
