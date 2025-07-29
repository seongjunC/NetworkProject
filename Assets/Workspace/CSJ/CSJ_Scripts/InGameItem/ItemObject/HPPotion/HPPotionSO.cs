using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class HPPotionSO : ItemEffectSO
{
    [SerializeField]
    private int HealAmount;

    [SerializeField]
    private bool isTurnSkip;

    public override void Activate()
    {
        // 추후 플레이어 컨트롤러에서 체력을 변경시키는 메서드 추가

        if (isTurnSkip)
        {

            if (TurnController.Instance != null)
            {
                TurnController.Instance.photonView.RPC("RPC_TurnFinished", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }

    }
}
