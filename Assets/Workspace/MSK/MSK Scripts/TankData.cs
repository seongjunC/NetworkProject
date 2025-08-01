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
    public TankRank rank;
    public Sprite icon;
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

            if (level <= 0) level = 1;

            level = CalculateLevelFromCount();
            Debug.Log($"현재 레벨 {level}로 설정 됨");
        }
    }

    [Tooltip("1렙에서 업그레이드에 필요한 기본 갯수")]
    public static int needDefaultUpgradeCount = 5;

    [Tooltip("레벨업 할때마다 더 필요할 갯수 (계산식 : needDefaultUpgradeCount + needUpgradeCount * (level - 1))")]
    public static int needUpgradeCount = 3;

    public int CalculateLevelFromCount()
    {
        if (count == 0)
            return 1;

        int c = count;
        int lvl = 1;
        while (c >= GetRequiredUpgradeCount(lvl))
        {
            c -= GetRequiredUpgradeCount(lvl);
            lvl++;
        }
        return lvl;
    }

    public int GetRequiredUpgradeCount(int level)
    {
        return needDefaultUpgradeCount + needUpgradeCount * (level - 1);
    }

    public int CurrentCount()
    {
        return Count + needDefaultUpgradeCount - GetRequiredUpgradeCount(Level);
    }
}
