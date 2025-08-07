using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/ItemEffect/Barrier")]
public class Barrier : ItemEffectSO
{


    public override void Activate()
    {
        MSKTurnController turnCon = MSKTurnController.Instance;
        turnCon.GetLocalPlayerController().ApplyBarrier();
    }
}
