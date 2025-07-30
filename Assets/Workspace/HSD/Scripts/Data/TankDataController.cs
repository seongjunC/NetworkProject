using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankDataController
{ 
    public Dictionary<string, TankData> TankDatas = new Dictionary<string, TankData>();
    public event Action<TankData> OnTankDataChanged;

    public void Init()
    {
        InitTankDatas();
    }

    private void InitTankDatas()
    {
        TankData[] tankDatas = Resources.LoadAll<TankData>("Data/Tank");
        foreach (var tankData in tankDatas)
        {
            this.TankDatas.Add($"{tankData.tankName}_{tankData.level}", tankData);
        }
    }

    public void UpdateCount(string tankName, int level, int count)
    {
        TankDatas[$"{tankName}_{level}"].count = count;
        OnTankDataChanged?.Invoke(TankDatas[$"{tankName}_{level}"]);
    }
}
