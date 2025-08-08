using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
            items[i].AddID(i.ToString());
        }
    }

    public ItemData Get(string id)
    {
        return lookup.TryGetValue(id, out var d) ? d : null;
    }
    public ItemData Get(ItemData item)
    {
        string id = GetIndex(item).ToString();
        if (id == "-99") return null;
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