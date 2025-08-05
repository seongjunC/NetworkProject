using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectTankSlot : MonoBehaviour
{
    [SerializeField] Image icon;

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
        icon.sprite = data.Icon;
    }
}
