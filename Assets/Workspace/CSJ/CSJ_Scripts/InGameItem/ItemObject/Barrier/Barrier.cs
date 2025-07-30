using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/ItemEffect/Barrier")]
public class Barrier : ItemEffectSO
{


    public override void Activate()
    {
        TurnController turnCon = TurnController.Instance;
        turnCon.GetLocalPlayerController().ApplyBarrier();
    }
}
