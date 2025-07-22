using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Manager
{
    public static FirebaseManager Firebase => FirebaseManager.Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        FirebaseManager.CreateInstance();
    }
}
