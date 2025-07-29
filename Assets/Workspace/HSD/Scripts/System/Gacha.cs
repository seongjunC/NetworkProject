using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gacha : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] int[] ints;
    [SerializeField] float[] chance;
    [SerializeField] bool isTen;
    [SerializeField] TankData[] model;

    [Header("List")]
    [SerializeField] List<Transform> cardTransform = new();
    [SerializeField] List<TankData> gachaList = new();

    [Header("Cards")]
    [SerializeField] List<Card> cards = new();
    [SerializeField] Transform cradContent;
    [SerializeField] Transform cardSpawnTransform;
    [SerializeField] GameObject cardPrefab;

    [Header("Move")]
    [SerializeField] float moveTime;
    [SerializeField] float cardDelay;

    private YieldInstruction delay;

    private void Start()
    {
        delay = new WaitForSeconds(cardDelay);
    }

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

        GameObject cardPosObj = new GameObject("Card");
        cardPosObj.transform.SetParent(cradContent, false);

        cardTransform.Add(cardPosObj.transform);

        Manager.Data.InventoryData.AddTank(selectTank.tankName, selectTank.level, selectTank.count, selectTank.rank);

        return selectTank;
    }

    private void SetUpCard()
    {

    }
}
