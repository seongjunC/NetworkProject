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
                Debug.Log("파베 설정 충족");
                app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;

                if (auth.CurrentUser != null)
                {
                    Debug.Log("자동 로그인 유효함: " + auth.CurrentUser.Email);
                    FirebaseUser user = Auth.CurrentUser;

                    OnAuthSettingComplated?.Invoke(user == null);

                    if (user != null)
                    {                        
                        string firebaseUID = user.UserId;

                        PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues();
                        PhotonNetwork.AuthValues.UserId = firebaseUID;      // 포톤 UID 설정

                        PhotonNetwork.ConnectUsingSettings();
                    }                    
                }
                else
                {
                    Debug.Log("자동 로그인 없음");
                    return;
                }

                database = FirebaseDatabase.DefaultInstance;
                database.GoOnline();
                database.SetPersistenceEnabled(false);
                Manager.Database.Init();
            }
            else
            {
                Debug.LogError("파베 설정이 충족되지 않음");
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
            Debug.LogError("Firebase 데이터 가져오기 실패");
            yield break;
        }

        var snapshot = task.Result;
        string json = snapshot?.GetRawJsonValue();
        Debug.Log("Firebase 유저 데이터: " + json);

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
