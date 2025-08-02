using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public struct FourGradient
{
    public Color topLeft;
    public Color topRight;
    public Color bottimLeft;
    public Color bottomRight;
}

public class ProbabilityPanel : MonoBehaviour
{
    [SerializeField] Gacha gacha;
    [SerializeField] GameObject rankPersentPrefab;
    [SerializeField] GameObject probabilitySlotPrefab;
    [SerializeField] Transform probabilityContent;

    [Header("Color")]
    [SerializeField] FourGradient sRank;
    [SerializeField] FourGradient aRank;
    [SerializeField] FourGradient bRank;
    [SerializeField] FourGradient cRank;

    private void Awake()
    {
        
    }

    private void InitSlots()
    {
        TankData[] tankDatas = Manager.Data.TankDataController.TankDatas.Values.ToArray();

        TankData[] sTanks = tankDatas.Where(t => t.rank == Rank.S).ToArray();
        TankData[] aTanks = tankDatas.Where(t => t.rank == Rank.A).ToArray();
        TankData[] bTanks = tankDatas.Where(t => t.rank == Rank.B).ToArray();
        TankData[] cTanks = tankDatas.Where(t => t.rank == Rank.C).ToArray();

        foreach (var item in sTanks)
        {
            
        }
    }
}
