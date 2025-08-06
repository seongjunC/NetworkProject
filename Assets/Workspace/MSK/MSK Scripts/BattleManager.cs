using Photon.Pun;
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
        _turnEndButton.interactable = false;
    }

    #endregion

    public void SetTurnEndButton(bool result)
    {
            _turnEndButton.interactable = result;
    }

    public void TestTurnEnd()
    {
        if (_playerController != null)
            _playerController.EndPlayerTurn();

        if (_turnController.IsMyTurn())
            _turnController.TurnFinished();
    }
    public void TestTurnEnd(int actnum)
    {
        if (_playerController != null)
            _playerController.EndPlayerTurn();

        if (_turnController.IsMyTurn())
            _turnController.TurnFinished(actnum);
    }

    private void PlayerAttacked()
    {
        _turnController.photonView.RPC("RPC_TimeStop", RpcTarget.All);
        //  시간 정지
        /*
        if (_playerController._hp <= 0)
        {
            // 사망 처리
            _turnController.photonView.RPC("RPC_PlayerDead", RpcTarget.MasterClient);
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
        }*/
    }

    public void RegisterPlayer(PlayerController playerController)
    {
        if (_playerController != null)
            _playerController.OnPlayerAttacked = null;
        _playerController = playerController;
        _playerController.OnPlayerAttacked += PlayerAttacked;
    }
}
