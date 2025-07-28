using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MSK_UIManager : MonoBehaviour
{
    [SerializeField] private Slider _hp_Slider;
    [SerializeField] private Slider _power_Slider;
    [SerializeField] private Slider _move_Slider;

    private PlayerController _player;
    private Fire _fire;
    public void RegisterPlayer(PlayerController playerController)
    {
        _player = playerController;
        _fire = _player.GetComponentInChildren<Fire>();

        if (_fire != null)
            _power_Slider.maxValue = _fire.maxPower;

        _hp_Slider.maxValue = _player._hp;
        _move_Slider.maxValue = _player._movable;
    }

    private void Update()
    {
        if (_player == null) return;

        _hp_Slider.value = _player._hp;
        _move_Slider.value = _player._movable;

        if (_fire != null)
            _power_Slider.value = _fire.powerCharge;
    }
}
