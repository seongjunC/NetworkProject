using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private void Start()
    {
        Grid grid = GridManager.Get<Grid>(gameObject);
    }
}

public static class GridManager 
{
    private static Dictionary<string , Component> componentDic = new Dictionary<string , Component>(10000);

    public static void RegisterGrid<T>(T component) where T : Component
    {
        if(!componentDic.ContainsKey(component.gameObject.name))
        {
            componentDic.Add($"{component.gameObject.name}_{typeof(T).Name}", component);
        }
    }

    public static T Get<T>(GameObject obj) where T : Component
    {
        return componentDic[$"{obj.name}_{typeof(T).Name}"] as T;
    }

    public static void UnRegisterGrid<T>(T component) where T : Component
    {
        componentDic.Remove($"{component.gameObject.name}_{typeof(T).Name}");
    }

    public static void Reset()
    {
        componentDic.Clear();
    }
}
