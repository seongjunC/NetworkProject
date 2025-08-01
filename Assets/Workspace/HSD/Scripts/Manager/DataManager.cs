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

    private void Start()
    {
        Manager.Firebase.OnLogOut += () => TankDataController.TankDatas.Clear();
    }

    public void Init()
    {
        TankInventoryData = new();
        Debug.Log("1");
        TankDataController.Init();
        Debug.Log("2");
        TankInventoryData.OnTankCountUpdated += TankDataController.UpdateCount;
        Debug.Log("3");
        TankInventoryData.Init();
        Debug.Log("4");
    }
}
