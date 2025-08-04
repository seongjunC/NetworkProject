using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TankStat", menuName = "Data/TankStat")]
public class TankData : ScriptableObject
{
    [Header("Stat")]
    public float damage;
    public float maxMove;
    public float speed;
    public float maxHp;

    [Header("MetaData")]
    public string tankName;
    public Rank rank;
    public Sprite Icon;
    [TextArea]
    public string description;

    private int level;
    [field: SerializeField] 
    public int Level { get => level; set { level = value; } }

    private int count;
    [field : SerializeField] public int Count
    {
        get => count;
        set
        {
            count = value;

            if (Level <= 0) Level = 1;

            Level = CalculateLevelFromCount();
        }
    }

    [Tooltip("1렙에서 업그레이드에 필요한 기본 갯수")]
    public static int needDefaultUpgradeCount = 5;

    [Tooltip("레벨업 할때마다 더 필요할 갯수 (계산식 : needDefaultUpgradeCount + needUpgradeCount * (level - 1))")]
    public static int needUpgradeCount = 3;

    public int CalculateLevelFromCount()
    {
        int lvl = 1;
        while (count >= GetTotalRequiredCount(lvl + 1))
        {
            lvl++;
        }
        return lvl;
    }

    public int GetTotalRequiredCount(int level)
    {
        int total = 0;
        for (int i = 1; i < level; i++)
        {
            total += GetRequiredUpgradeCount(i);
        }
        return total;
    }

    public int GetRequiredUpgradeCount(int level)
    {
        return needDefaultUpgradeCount + (needUpgradeCount * (level - 1));
    }

    public int CurrentCount()
    {
        return count - GetTotalRequiredCount(Level);
    }

    /// <summary>
    /// 다음레벨 까지 가기위한 갯수
    /// </summary>
    /// <returns></returns>
    public int GetRequiredCountForNextLevel()
    {
        return GetRequiredUpgradeCount(Level);
    }

    /// <summary>
    /// 이번 레벨에서 얼마나 강화가 되었는지
    /// </summary>
    /// <returns></returns>
    public int GetProgressTowardsNextLevel()
    {
        return count - GetTotalRequiredCount(Level);
    }
}
