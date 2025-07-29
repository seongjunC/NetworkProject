using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Gacha : MonoBehaviour
{
    [Header("Setting")]
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(SetUpCardRoutine());
    }

    [ContextMenu("Gacha")]
    public void TryGacha()
    {
        cardTransform.Clear();
        gachaList.Clear();
        cards.Clear();

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
        float rand = Random.Range(0, 100);

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

        TankData[] randomData = model.
            Where(t => (int)t.rank == select).ToArray();

        TankData selectTank = randomData[Random.Range(0, randomData.Length)];

        GameObject cardPosObj = new GameObject("Card");
        cardPosObj.transform.SetParent(cradContent, false);

        cardTransform.Add(cardPosObj.transform);

        Card card = Instantiate(cardPrefab, cardSpawnTransform.position, Quaternion.identity).GetComponent<Card>();        
        cards.Add(card);

        Manager.Data.InventoryData.AddTank(selectTank.tankName, selectTank.level, selectTank.count, selectTank.rank);

        return selectTank;
    }

    private IEnumerator SetUpCardRoutine()
    {
        for (int i = 0;i < cards.Count; i++)
        {
            cards[i].SetUp(gachaList[i], cardTransform[i], moveTime);
            yield return delay;
        }
    }
}
