using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TankStat", menuName = "Data/TankStat")]
public class TankData : ScriptableObject
{
    [Header("Base Stats")]
    public float baseDamage;
    public float baseSpeed;
    public float baseMaxHp;
    public float baseMaxMove;

    [Header("Growth Per Level")]
    public float damageGrowth;
    public float speedGrowth;
    public float maxHpGrowth;
    public float maxMoveGrowth;

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

    [Tooltip("1������ ���׷��̵忡 �ʿ��� �⺻ ����")]
    public static int needDefaultUpgradeCount = 5;

    [Tooltip("������ �Ҷ����� �� �ʿ��� ���� (���� : needDefaultUpgradeCount + needUpgradeCount * (level - 1))")]
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
    /// �������� ���� �������� ����
    /// </summary>
    /// <returns></returns>
    public int GetRequiredCountForNextLevel()
    {
        return GetRequiredUpgradeCount(Level);
    }

    /// <summary>
    /// �̹� �������� �󸶳� ��ȭ�� �Ǿ�����
    /// </summary>
    /// <returns></returns>
    public int GetProgressTowardsNextLevel()
    {
        return count - GetTotalRequiredCount(Level);
    }

    public void InitStat()
    {
        damage = GetDamage();
        speed = GetSpeed();
        maxHp = GetMaxHp();
        maxMove = GetMaxMove();
    }

    public float GetDamage() => baseDamage + damageGrowth * (Level - 1) * GetRankMultiplier();
    public float GetSpeed() => baseSpeed + speedGrowth * (Level - 1) * GetRankMultiplier();
    public float GetMaxHp() => baseMaxHp + maxHpGrowth * (Level - 1) * GetRankMultiplier();
    public float GetMaxMove() => baseMaxMove + maxMoveGrowth * (Level - 1) * GetRankMultiplier();

    private float GetRankMultiplier()
    {
        return rank switch
        {
            Rank.C => 1f,
            Rank.B => 1.15f,
            Rank.A => 1.35f,
            Rank.S=> 1.6f,
            _ => 1f
        };
    }
}
