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
    [SerializeField] List<Transform> cardTransforms = new();
    [SerializeField] List<TankData> gachaList = new();

    [Header("Cards")]
    [SerializeField] List<Card> cards = new();
    [SerializeField] Transform cradContent;
    [SerializeField] Transform cardSpawnTransform;
    [SerializeField] GameObject cardPrefab;

    [Header("Move")]
    [SerializeField] float moveTime;
    [SerializeField] float cardDelay;

    [Header("Delay")]
    [SerializeField] float gachaAddDelay;
    private YieldInstruction delay;
    private YieldInstruction addDelay;

    private void Start()
    {
        delay = new WaitForSeconds(cardDelay);
        addDelay = new WaitForSeconds(gachaAddDelay);
        model = Manager.Data.TankDataController.TankDatas.Values.ToArray();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(SetUpCardRoutine());
    }

    [ContextMenu("Gacha")]
    public void TryGacha()
    {
        StartCoroutine(GachaRoutine());
    }

    private IEnumerator GachaRoutine()
    {
        SetUpCardTransformList();
        ClearCards();
        gachaList.Clear();

        if (isTen)
        {
            for (int i = 0; i < 10; i++)
            {
                gachaList.Add(GetRandomTank());
                yield return addDelay;
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

        Card card = Instantiate(cardPrefab, cardSpawnTransform.position, Quaternion.identity, cardSpawnTransform).GetComponent<Card>();
        cards.Add(card);

        Manager.Data.TankInventoryData.AddTankEvent(selectTank.tankName, selectTank.level, 1, selectTank.rank);

        return selectTank;
    }

    private void SetUpCardTransformList()
    {
        if(isTen)
        {
            foreach(Transform t in cardTransforms)
            {
                t.gameObject.SetActive(true);
            }
        }
        else
        {
            for(int i = 0; i < cardTransforms.Count; i++)
            {
                if(i == 0)
                    cardTransforms[i].gameObject.SetActive(true);
                else
                    cardTransforms[i].gameObject.SetActive(false);
            }
        }
    }
    private void ClearCards()
    {
        foreach (Card c in cards)
        {
            Destroy(c.gameObject);
        }
        cards.Clear();
    }

    private IEnumerator SetUpCardRoutine()
    {
        for (int i = 0;i < cards.Count; i++)
        {
            cards[i].SetUp(gachaList[i], cardTransforms[i], moveTime);
            yield return delay;
        }
    }
}
