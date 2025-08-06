using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/ItemEffect/AttackPotion")]
public class AttackPotion : ItemEffectSO
{
    [Header("버프 수치 ")]
    [SerializeField]
    private float BuffAmount;
    [Header("적용 범위")]
    [SerializeField]
    private bool isFixed;

    public override void Activate()
    {
        MSKTurnController turnCon = MSKTurnController.Instance;
        if (isFixed)
            turnCon.GetLocalPlayerFire().SetFixedDamageBuff(BuffAmount);
        else
        {
            turnCon.GetLocalPlayerFire().SetRatioDamageBuff(BuffAmount);
        }
    }
}
