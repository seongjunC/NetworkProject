using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/ItemData")]
public class ItemData : ScriptableObject
{
    [Header("아이템 설명")]
    public string itemName;
    [TextArea(2, 20)]
    public string description;

    [Header("아이템 모습")]
    public Sprite icon;
    public GameObject prefab;

    [Header("드롭율")]
    public int appearRate = 1;

    [Header("아이템 스크립트")]
    public ItemEffectSO Effect;

    public void UseItem(int actorNumber)
    {
        Debug.Log($"{itemName} 사용!");
        Effect.Activate(actorNumber);
    }

}
