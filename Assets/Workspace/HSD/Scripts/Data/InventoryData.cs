using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TestTankRank
{
    S,A,B,C
}

[Serializable]
public class Test_TankData
{
    public string Key => $"{TankName}_{Level}";
    public string TankName;
    public TestTankRank Rank;
    public int Level;
    public int count;
    
}

[Serializable]
public class InventoryData
{
    public Dictionary<string, Test_TankData> tankDataDic = new();
}
