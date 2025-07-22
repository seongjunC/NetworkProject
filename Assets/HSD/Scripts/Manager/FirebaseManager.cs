using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
                Debug.Log("�ĺ� ���� ����");
                app         = FirebaseApp.DefaultInstance;
                auth        = FirebaseAuth.DefaultInstance;

                if(auth.CurrentUser != null)
                {
                    Debug.Log("�ڵ� �α��� ��ȿ��: " + auth.CurrentUser.Email);
                    SceneManager.LoadSceneAsync("Lobby");
                }
                else
                {
                    Debug.Log("�ڵ� �α��� ����");
                }

                database    = FirebaseDatabase.DefaultInstance;
                database.GoOnline();
                database.SetPersistenceEnabled(false);
                Manager.Database.Init();
            }
            else
            {
                Debug.LogError("�ĺ� ������ �������� ����");
            }
        });
    }
    #endregion

    public void LogOut()
    {
        Auth.SignOut();
        SceneManager.LoadSceneAsync("Login");
    }
}
