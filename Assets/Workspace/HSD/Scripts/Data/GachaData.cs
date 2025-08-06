using Firebase.Extensions;
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
    public TankData pickUp;

    private void OnEnable()
    {
        GachaList = Resources.LoadAll<TankData>("Data/Tank");
    }

    public void InitPickUp()
    {
        FirebaseManager.Database.RootReference.Child("PickUp").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log("픽업 설정 실패");
                return;
            }

            Debug.Log((string)task.Result.Value);

            pickUp = Manager.Data.TankDataController.TankDatas[(string)task.Result.Value];
        });
    }

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

    public float GetGachaPercent(Rank rank)
    {
        float totalChance = 0;

        foreach (var data in GachaDatas)
        {
            totalChance += data.chance;
        }

        float selectChance = GachaDatas.FirstOrDefault(g => g.rank == rank).chance;

        float chance = (selectChance / totalChance) * 100;      

        return chance;
    }
}
