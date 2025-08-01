using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
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
            Debug.Log($"{tankData.tankName} 추가");
            tankData.Level = 1;
            tankData.Count = 0;
            TankDatas.Add(tankData.tankName, tankData);
        }
    }

    public void UpdateCount(string tankName, int count)
    {
        Debug.Log($"{tankName}_{count}로 설정");
        TankDatas[tankName].Count = count;
        OnTankDataChanged?.Invoke(TankDatas[tankName]);
    }    
}
