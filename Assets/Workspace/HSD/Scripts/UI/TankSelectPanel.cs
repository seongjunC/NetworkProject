using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankSelectPanel : MonoBehaviour
{
    [SerializeField] TankSlot slot;
    [SerializeField] TankToolTip toolTip;

    private void OnEnable()
    {
        UpdateTankSlot(Manager.Data.TankDataController.CurrentTank);
        Manager.Data.TankDataController.OnTankSelected += UpdateTankSlot;
    }

    private void OnDisable()
    {
        Manager.Data.TankDataController.OnTankSelected -= UpdateTankSlot;
    }

    private void UpdateTankSlot(TankData data)
    {
        slot.SetUp(data, toolTip, Utils.GetColor(data.rank));
    }
}
