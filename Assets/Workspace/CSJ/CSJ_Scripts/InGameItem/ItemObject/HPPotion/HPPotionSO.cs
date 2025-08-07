using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/ItemEffect/HpPotion")]
public class HPPotionSO : ItemEffectSO
{
    [SerializeField]
    private int HealAmount;

    [SerializeField]
    private bool isTurnSkip;

    private MSKTurnController turnController;

    public override void Activate()
    {
        // 추후 플레이어 컨트롤러에서 체력을 변경시키는 메서드 추가
        if (MSKTurnController.Instance != null)
        {
            turnController = MSKTurnController.Instance;
        }
        else
        {
            Debug.LogError("턴 컨트롤러를 찾을 수 없습니다");
            return;
        }
        PlayerController playerCon = turnController.GetPlayerController(PhotonNetwork.LocalPlayer.ActorNumber);
        playerCon.RatioHealToPlayer(HealAmount);
        if (isTurnSkip)
        {
            if (turnController != null)
            {
                turnController.photonView.RPC("RPC_TurnFinished", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
            }
        }

    }
}
