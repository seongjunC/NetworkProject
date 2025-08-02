using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "GachaData", menuName = "Data/Gacha")]
public class GachaData : ScriptableObject
{
    public GachaChance[] GachaDatas;
    public int NeedGem;
    public TankData[] GachaList;

    public TankData[] GetRankTankData(Rank rank)
    {
        return (rank) switch
        {
            Rank.S => GachaList.Where(t => t.rank == Rank.S).ToArray(),
            Rank.A => GachaList.Where(t => t.rank == Rank.A).ToArray(),
            Rank.B => GachaList.Where(t => t.rank == Rank.B).ToArray(),
            Rank.C => GachaList.Where(t => t.rank == Rank.C).ToArray(),
            _ => GachaList
        };
    }
}
