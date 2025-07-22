using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirebaseManager : Singleton<FirebaseManager>
{
    #region Firebase
    private static FirebaseAuth     auth;
    public static FirebaseAuth      Auth { get { return auth; } }

    private static FirebaseApp      app;
    public static FirebaseApp       App { get { return app; } }

    private static FirebaseDatabase database;
    public static FirebaseDatabase  Database { get { return database; } }
    #endregion

    #region LifeCycle

    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(t =>
        {
            DependencyStatus status = t.Result;

            if(status == DependencyStatus.Available)
            {
                Debug.Log("파베 설정 충족");
                app         = FirebaseApp.DefaultInstance;
                auth        = FirebaseAuth.DefaultInstance;
                database    = FirebaseDatabase.DefaultInstance;
                database.GoOnline();
                database.SetPersistenceEnabled(false);
                Manager.Database.Init();
            }
            else
            {
                Debug.LogError("파베 설정이 충족되지 않음");
            }
        });
    }
    #endregion
}
