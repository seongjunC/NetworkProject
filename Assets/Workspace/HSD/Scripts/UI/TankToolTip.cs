using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankToolTip : MonoBehaviour
{
    [SerializeField] Image tankIcon;

    [Header("Texts")]
    [SerializeField] TMP_Text tankNameText;
    [SerializeField] TMP_Text rankText;
    [SerializeField] TMP_Text damageText;
    [SerializeField] TMP_Text maxMoveText;
    [SerializeField] TMP_Text hpText;

    public void ShowToolTip(TankData data)
    {
        gameObject.SetActive(true);

        hpText.text = data.maxHp.ToString();
        rankText.text = data.rank.ToString();
        tankIcon.sprite = data.icon;
        damageText.text = data.damage.ToString();
        maxMoveText.text = data.maxMove.ToString();
        tankNameText.text = data.tankName;
    }

    public void CloseToolTip()
    {
        gameObject.SetActive(false);
    }
}
