using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(menuName = "Item/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public ItemData[] items;
    private Dictionary<string, ItemData> lookup = new();

    public void OnEnable()
    {
        for (int i = 0; i < items.Length; i++)
        {
            lookup.Add(i.ToString(), items[i]);
        }
    }

    public ItemData Get(string id)
    {
        return lookup.TryGetValue(id, out var d) ? d : null;
    }

    public ItemData[] GetAll()
    {
        return items;
    }

    public int GetIndex(ItemData item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item) return i;
        }
        return -99;
    }
}