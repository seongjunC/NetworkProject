using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/ItemEffect/WealthOfPotion")]
public class WealthOfPotion : ItemEffectSO
{
    [Header("젬 획득 개수")]
    [SerializeField]
    private int GemAmount;

    public override void Activate()
    {
        PlayerData data = Manager.Data.PlayerData;
        data.GemGain(GemAmount);
    }

}
