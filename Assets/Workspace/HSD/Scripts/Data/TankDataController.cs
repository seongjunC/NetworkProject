using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class TankDataController
{ 
    public Dictionary<string, TankData> TankDatas = new Dictionary<string, TankData>();    

    public event Action<TankData> OnTankDataChanged;
    public Action<TankData> OnTankSelected;

    private TankData currentTank;
    public TankData CurrentTank
    {
        get
        {
            if (currentTank == null)
            {
                foreach (var tank in TankDatas.Values)
                {
                    if (tank.Count > 0)
                    {
                        SetSelectTank(tank);
                        break;
                    }
                }
            }

            return currentTank;
        }

        set => currentTank = value;
    }

    public void Init()
    {
        InitTankDatas();
    }

    private void InitTankDatas()
    {
        TankData[] tankDatas = Resources.LoadAll<TankData>("Data/Tank");
        foreach (var tankData in tankDatas)
        {
            tankData.Level = 1;
            tankData.Count = 0;
            TankDatas.Add(tankData.tankName, tankData);
        }
    }

    public void UpdateCount(string tankName, int count)
    {
        TankDatas[tankName].Count = count;
        OnTankDataChanged?.Invoke(TankDatas[tankName]);
    }
    
    public void SetSelectTank(TankData data)
    {
        if (data == null) return;

        currentTank = data;
        OnTankSelected?.Invoke(data);
        PhotonNetwork.LocalPlayer.SetTank(data.tankName);
    }
}
