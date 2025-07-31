using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Gacha : MonoBehaviour
{
    [SerializeField] Image image;

    [Header("Setting")]
    public int needGem;
    public bool isTen;
    [SerializeField] float[] chance;
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

    [ContextMenu("Gacha")]
    public void TryGacha()
    {
        gameObject.SetActive(true);
        StartCoroutine(GachaRoutine());
    }

    private IEnumerator GachaRoutine()
    {
        SetUpCardTransformList();
        ClearCards();
        gachaList.Clear();
        
        float progress = 0;
        float time = .5f;

        while(progress < time)
        {
            progress += Time.deltaTime;
            Color color = image.color;
            color.a = Mathf.Lerp(0, 1, progress / time);
            image.color = color;
            yield return null;
        }        

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

        yield return new WaitForSeconds(.5f);

        StartCoroutine(SetUpCardRoutine());
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
