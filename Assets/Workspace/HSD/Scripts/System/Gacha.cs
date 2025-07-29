using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gacha : MonoBehaviour
{
    [SerializeField] int[] ints;
    [SerializeField] float[] chance;
    [SerializeField] bool isTen;
    [SerializeField] private TankData[] model;
    [SerializeField] private List<TankData> gachaList = new();

    [ContextMenu("Gacha")]
    public void TryGacha()
    {
        gachaList.Clear();

        if (isTen)
        {
            for (int i = 0; i < 10; i++)
            {
                gachaList.Add(GetRandomTank());
            }
        }
        else
            gachaList.Add(GetRandomTank());
    }

    private TankData GetRandomTank()
    {
        float rand = Random.Range(0, ints.Length);

        TankData[] randomData = model.
            Where(t => (int)t.rank == (int)rand).ToArray();

        rand = Random.Range(0, 100);

        int select = 0;

        float cumulative = 0;

        for (int i = 0; i < chance.Length; i++)
        {
            cumulative += chance[i];
            if (rand < cumulative)
            {
                select = i;
                break;
            }
        }

        TankData selectTank = randomData[select];

        Manager.Data.InventoryData.AddTank(selectTank.tankName, selectTank.level, selectTank.count, selectTank.rank);

        return selectTank;
    }
}
