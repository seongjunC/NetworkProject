using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TestBattleManager : MonoBehaviour
{
    [SerializeField] Button _turnEndButton;
    private PlayerController _playerController;

    private void Start()
    {
        _turnEndButton.onClick.AddListener(TestTurnEnd);
    }
    private void TestTurnEnd()
    {
        if (_playerController != null)
            _playerController.ResetTurn();
    }
    public void RegisterPlayer(PlayerController playerController)
    {
        _playerController = playerController;
    }
}
