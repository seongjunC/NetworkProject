using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : Singleton<FirebaseManager>
{
    #region LifeCycle
    private void Awake()
    {
        Init();
    }

    private void Init()
    {

    }
    #endregion
}
