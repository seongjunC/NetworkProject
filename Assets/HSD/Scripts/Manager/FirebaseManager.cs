using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
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
    public event System.Action<bool> OnAuthSettingComplated;

    #region LifeCycle

    private void Start()
    {
        StartCoroutine(StartRoutine());
    }
    #endregion

    public void LogOut()
    {
        Auth.SignOut();
        SceneManager.LoadSceneAsync("Login");
    }

    private IEnumerator StartRoutine()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(t =>
        {
            DependencyStatus status = t.Result;

            if (status == DependencyStatus.Available)
            {
                Debug.Log("�ĺ� ���� ����");
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;

                if (auth.CurrentUser != null)
                {
                    Debug.Log("�ڵ� �α��� ��ȿ��: " + auth.CurrentUser.Email);
                    FirebaseUser user = Auth.CurrentUser;

                    OnAuthSettingComplated?.Invoke(user == null);

                    if (user != null)
                    {                        
                        string firebaseUID = user.UserId;

                        PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues();
                        PhotonNetwork.AuthValues.UserId = firebaseUID;      // ���� UID ����

                        PhotonNetwork.ConnectUsingSettings();
                    }                    
                }
                else
                {
                    Debug.Log("�ڵ� �α��� ����");
                    return;
                }

                database = FirebaseDatabase.DefaultInstance;
                database.GoOnline();
                database.SetPersistenceEnabled(false);
                Manager.Database.Init();
            }
            else
            {
                Debug.LogError("�ĺ� ������ �������� ����");
            }
        });
        yield return new WaitForSeconds(1);
        StartCoroutine(LoginRoutine());
    }

    public IEnumerator LoginRoutine()
    {
        var task = Manager.Database.userRef.GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.LogError("Firebase ������ �������� ����");
            yield break;
        }

        var snapshot = task.Result;
        string json = snapshot?.GetRawJsonValue();
        Debug.Log("Firebase ���� ������: " + json);

        if (string.IsNullOrEmpty(json))
        {
            var newData = new PlayerData();
            Manager.Data.PlayerData = newData;

            string saveJson = JsonUtility.ToJson(newData);
            Manager.Database.userRef.SetRawJsonValueAsync(saveJson);
        }
        else
        {
            Manager.Data.PlayerData = JsonUtility.FromJson<PlayerData>(json);
        }
        PhotonNetwork.JoinLobby();
        SceneManager.LoadSceneAsync("Lobby");
    }
}
