using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct GachaChance
{
    public Rank rank;
    public float chance;
}

public class Gacha : MonoBehaviour
{
    [SerializeField] Image image;    

    [Header("Setting")]    
    public bool isTen;
    public GachaData GachaData;

    [Header("List")]
    [SerializeField] List<Transform> cardTransforms = new();
    [SerializeField] List<TankData> gachaResultList = new();

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

    public Dictionary<TankData, int> beforeLevel = new();
    public Dictionary<TankData, int> afterLevel = new();

    public event System.Action OnGachaEnded;

    private string now;

    private void Start()
    {        
        delay = new WaitForSeconds(cardDelay);
        addDelay = new WaitForSeconds(gachaAddDelay);
        GachaData.GachaList = Manager.Data.TankDataController.TankDatas.Values.ToArray();
    }    

    [ContextMenu("Gacha")]
    public void TryGacha()
    {
        ClearCards();
        gameObject.SetActive(true);
        StartCoroutine(GachaRoutine());
    }

    private IEnumerator GachaRoutine()
    {
        yield return null;

        SetUpCardTransformList();
        SaveBeforeLevel();

        gachaResultList.Clear();
        
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
                gachaResultList.Add(GetRandomTank());
                yield return addDelay;
            }
        }
        else
            gachaResultList.Add(GetRandomTank());

        yield return new WaitForSeconds(.5f);

        SaveAfterLevel();

        StartCoroutine(SetUpCardRoutine());
    }

    private TankData GetRandomTank()
    {
        now = GetCurrentTimeKey();

        float max = 0;
        for (int i = 0; i < GachaData.GachaDatas.Length; i++)
        {
            max += GachaData.GachaDatas[i].chance;
        }
        
        float rand = Random.Range(0, max);

        int select = 0;

        float cumulative = 0;

        for (int i = 0; i < GachaData.GachaDatas.Length; i++)
        {
            cumulative += GachaData.GachaDatas[i].chance;
            if (rand < cumulative)
            {
                select = i;
                break;
            }
        }

        TankData[] randomData = GachaData.GetRankTankData((Rank)select);

        TankData selectTank = randomData[Random.Range(0, randomData.Length)];

        Card card = Instantiate(cardPrefab, cardSpawnTransform.position, Quaternion.identity, cardSpawnTransform).GetComponent<Card>();
        cards.Add(card);

        Manager.Data.TankInventoryData.AddTankEvent(selectTank.tankName, 1, selectTank.rank);
        Manager.Data.GachaManager.AddGachaResult(now, selectTank.tankName);

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
            cards[i].SetUp(gachaResultList[i], cardTransforms[i], moveTime);
            Manager.Audio.PlaySFX("Deal", Vector3.zero, 1, Random.Range(.8f, 1.2f));
            yield return delay;
        }

        OnGachaEnded?.Invoke();
        //Manager.Data.GachaManager.GachaResultsOrderBy();
    }

    private void SaveBeforeLevel()
    {
        beforeLevel.Clear();

        foreach (var tank in GachaData.GachaList)
        {
            beforeLevel.Add(tank, tank.Level);
        }
    }

    private void SaveAfterLevel()
    {
        afterLevel.Clear();

        TankData[] tanks = gachaResultList.Distinct().ToArray();

        foreach (var tank in tanks)
        {
            afterLevel.Add(tank, tank.Level);
        }
    }

    private string GetCurrentTimeKey()
    {
        System.DateTime nowKST = System.DateTime.UtcNow.AddHours(9);
        return nowKST.ToString("yyyyMMddHHmmssfff");
    }
}
