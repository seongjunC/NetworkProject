using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public PlayerData PlayerData;
    public TankInventoryData TankInventoryData;

    [Header("Cached")]
    public TankDataController TankDataController = new(); 

    public void Init()
    {
        TankInventoryData = new();
        TankDataController.Init();
        TankInventoryData.OnTankCountUpdated += TankDataController.UpdateCount;
        TankInventoryData.Init();
    }    
}
