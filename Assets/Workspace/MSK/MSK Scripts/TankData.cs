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
    public int level;
    public int count;
}
