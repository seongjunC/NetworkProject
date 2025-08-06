using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/ItemEffect/DoubleAttackPotion")]
public class DoubleAttackPotion : ItemEffectSO
{

    public override void Activate()
    {
        MSKTurnController turnCon = MSKTurnController.Instance;
        turnCon.GetLocalPlayerFire().SetDoubleAttack();
    }
}
