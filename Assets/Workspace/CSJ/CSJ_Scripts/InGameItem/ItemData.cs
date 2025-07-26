using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Battle/ItemData")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public GameObject prefab;
    public string description;
    public int appearRate = 1;
    public IActivatable Effect;

    public void UseItem()
    {
        Effect.Activate();
    }

}
