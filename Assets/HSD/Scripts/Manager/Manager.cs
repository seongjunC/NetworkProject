using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Manager
{
    public static FirebaseManager Firebase => FirebaseManager.Instance;
    public static DatabaseManager Database => DatabaseManager.Instance;
    public static UIManager UI => UIManager.Instance;
    public static DataManager Data => DataManager.Instance;
    public static ResourcesManager Resources => ResourcesManager.Instance;
    public static PoolManager Pool => PoolManager.Instance;
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        FirebaseManager.CreateInstance();
        DatabaseManager.CreateInstance();        
        UIManager.CreateInstance();
        DataManager.CreateInstance();
        ResourcesManager.CreateInstance();
        PoolManager.CreateInstance();
    }
}
