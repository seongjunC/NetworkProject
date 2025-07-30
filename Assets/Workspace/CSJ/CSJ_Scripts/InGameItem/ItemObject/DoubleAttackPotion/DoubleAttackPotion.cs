using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/ItemEffect/DoubleAttackPotion")]
public class DoubleAttackPotion : ItemEffectSO
{

    public override void Activate()
    {
        TurnController turnCon = TurnController.Instance;
        turnCon.GetLocalPlayerFire().SetDoubleAttack();
    }
}
