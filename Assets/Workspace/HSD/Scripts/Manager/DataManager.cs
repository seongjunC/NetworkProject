using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public PlayerData PlayerData;
    public TankInventoryData TankInventoryData;    
    
    public TankDataController TankDataController = new();
    public GachaManager GachaManager = new();
    public SaveManager saveManager = new();

    private void Start()
    {
        Manager.Firebase.OnLogOut += () => TankDataController.TankDatas.Clear();
    }

    public void Init()
    {
        TankInventoryData = new();
        TankDataController.Init();
        TankInventoryData.OnTankCountUpdated += TankDataController.UpdateCount;
        TankInventoryData.Init();
        GachaManager.Init();
    }
}
