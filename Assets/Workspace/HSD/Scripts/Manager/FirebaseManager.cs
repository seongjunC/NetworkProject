using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System;
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

    public event Action OnAuthSettingComplated;
    public event Action OnLogOut;

    private void Awake()
    {
        Application.runInBackground = true;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(t =>
        {
            DependencyStatus status = t.Result;

            if (status == DependencyStatus.Available)
            {
                Debug.Log("�ĺ� ���� ����");
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                database = FirebaseDatabase.DefaultInstance;
                database.GoOnline();
                database.SetPersistenceEnabled(false);
            }
            else
            {
                Debug.LogError("�ĺ� ������ �������� ����");
            }
            OnAuthSettingComplated?.Invoke();
        });
    }

    public void LogOut()
    {
        StartCoroutine(LogOutRoutine());
    }

    private IEnumerator LogOutRoutine()
    {
        Manager.UI.FadeScreen.FadeIn(1);

        yield return new WaitForSeconds(1);

        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            Debug.Log("Photon �κ� ������ ��...");

            yield return new WaitUntil(() => !PhotonNetwork.InLobby);
        }

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
            Debug.Log("Photon ���� ���� ��...");

            yield return new WaitUntil(() => !PhotonNetwork.IsConnected);
        }
        yield return new WaitForSeconds(1);

        Manager.UI.FadeScreen.FadeOut(1);
        Auth.SignOut();
        OnLogOut?.Invoke();

        Manager.UI.PopUpUI.Show("���������� �α׾ƿ� �Ͽ����ϴ�.");
    }
}
