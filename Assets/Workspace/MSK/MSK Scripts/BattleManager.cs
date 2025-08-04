using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBattleManager : MonoBehaviourPun
{
    [SerializeField] Button _turnEndButton;
    [SerializeField] private MSKTurnController _turnController;

    private PlayerController _playerController;


    #region Unity LifeCycle
    private void Start()
    {
        _turnEndButton.onClick.AddListener(TestTurnEnd);
    }

    #endregion

    private void TestTurnEnd()
    {
        if (_playerController != null)
            _playerController.EndPlayerTurn();

        if (_turnController.IsMyTurn())
        {
            if (PhotonNetwork.IsMasterClient)
                _turnController.TurnFinished();
            else
                _turnController.photonView.RPC("RPC_TurnFinished", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber);
        }
    }
    private void PlayerAttacked()
    {
        if (_playerController._hp <= 0)
        {
            // 사망 처리
            _turnController.photonView.RPC("RPC_PlayerDead", RpcTarget.All);
            // 죽은 유저가 자신의 턴이었다면, 턴도 종료
            if (_turnController.IsMyTurn())
            {
                _turnController.TurnFinished();
            }
            return;
        }
        // 살아있고 내 턴이면 턴 종료
        if (_turnController.IsMyTurn())
        {
            _turnController.TurnFinished();
        }
    }

    public void RegisterPlayer(PlayerController playerController)
    {
        _playerController = playerController;
        _playerController.OnPlayerAttacked += PlayerAttacked;
    }
}
