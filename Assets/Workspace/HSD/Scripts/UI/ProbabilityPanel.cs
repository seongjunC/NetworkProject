using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] GameObject rankPercentPrefab;
    [SerializeField] GameObject probabilitySlotPrefab;
    [SerializeField] Transform probabilityContent;
    [SerializeField] ScrollRect scrollRect;

    [Header("Color")]
    [SerializeField] FourGradient sRank;
    [SerializeField] FourGradient aRank;
    [SerializeField] FourGradient bRank;
    [SerializeField] FourGradient cRank;

    private void OnEnable()
    {
        scrollRect.normalizedPosition = Vector3.one;
    }

    private void Start()
    {
        InitSlots();
    }

    private void InitSlots()
    {
        for (int i = 0; i < (int)Rank.C + 1; i++)
        {
            CreateSlot((Rank)i);
        }
    }

    private void CreateSlot(Rank rank)
    {
        TankData[] tanks = gacha.GachaData.GetRankTankData(rank);

        float totalChance = gacha.GachaData.GetGachaPercent(rank);

        Instantiate(rankPercentPrefab, probabilityContent).GetComponent<RankPercentPanel>().
            SetUp(totalChance, rank, GetRankGradient(rank));

        float pickupChance = totalChance;

        if (rank == Rank.S && gacha.GachaData.pickUp != null)
        {
            pickupChance = totalChance / 2f;

            int normalCount = tanks.Length - 1;
            float normalChance = pickupChance / normalCount;

            foreach (var tank in tanks)
            {
                float chance = (tank == gacha.GachaData.pickUp) ? pickupChance : normalChance;

                Instantiate(probabilitySlotPrefab, probabilityContent)
                    .GetComponent<ProbabilitySlot>()
                    .SetUp(tank, chance);
            }
        }
        else
        {
            float chancePerTank = totalChance / tanks.Length;

            foreach (var tank in tanks)
            {
                Instantiate(probabilitySlotPrefab, probabilityContent)
                    .GetComponent<ProbabilitySlot>()
                    .SetUp(tank, chancePerTank);
            }
        }

    }

    private VertexGradient GetRankGradient(Rank rank)
    {
        FourGradient fourGradient = (rank) switch
        {
            Rank.S => sRank,
            Rank.A => aRank,
            Rank.B => bRank,
            Rank.C => cRank,
            _ => cRank
        };

        return new VertexGradient
        {
            bottomLeft = fourGradient.bottimLeft,
            bottomRight = fourGradient.bottomRight,
            topLeft = fourGradient.topLeft,
            topRight = fourGradient.topRight,
        };
    }
}
