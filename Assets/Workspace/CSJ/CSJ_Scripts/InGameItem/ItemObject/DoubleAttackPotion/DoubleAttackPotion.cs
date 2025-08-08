using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/ItemEffect/DoubleAttackPotion")]
public class DoubleAttackPotion : ItemEffectSO
{

    public override void Activate(int actorNumber)
    {
        MSKTurnController turnCon = MSKTurnController.Instance;
        turnCon.GetFire(actorNumber).SetDoubleAttack();
    }
}
