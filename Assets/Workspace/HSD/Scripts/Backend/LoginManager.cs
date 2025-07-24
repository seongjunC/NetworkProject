using Firebase.Auth;
using Firebase.Database;
using Firebase;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;

public class LoginManager : MonoBehaviourPunCallbacks
{
    [Header("InputFields")]
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField pw;

    [Header("Buttons")]
    [SerializeField] Button loginButton;
    [SerializeField] Button signupButton;

    [Header("Panels")]
    [SerializeField] GameObject signupPanel;
    [SerializeField] GameObject loginPanel;

    #region LifeCycle

    public override void OnEnable()
    {
        base.OnEnable();

        Init();
        Subscribe();
        loginButton.interactable = true;
    }
    public override void OnDisable()
    {
        base.OnDisable();

        UnSubscribe();
    }
    #endregion

    #region EventSubscribe
    private void Subscribe()
    {
        loginButton.onClick.AddListener(Login);
        signupButton.onClick.AddListener(SignUp);
        Manager.Firebase.OnAuthSettingComplated += StartRoutine;
    }
    private void UnSubscribe()
    {
        loginButton.onClick.RemoveListener(Login);
        signupButton.onClick.RemoveListener(SignUp);
        Manager.Firebase.OnAuthSettingComplated -= StartRoutine;
    }
    #endregion

    private void Init()
    {
        email.text = "";
        pw.text = "";
    }

    public void StartRoutine()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(t =>
        {
            DependencyStatus status = t.Result;

            if (status == DependencyStatus.Available)
            {
                FirebaseAuth auth = FirebaseManager.Auth;
                FirebaseDatabase database = FirebaseManager.Database;

                if (auth.CurrentUser != null)
                {
                    Debug.Log("�ڵ� �α��� ��ȿ��: " + auth.CurrentUser.Email);
                    FirebaseUser user = auth.CurrentUser;

                    LoginSetActive(user == null);

                    if (user != null)
                    {
                        PhotonNetwork.ConnectUsingSettings();
                    }
                }
                else
                {
                    Debug.Log("�ڵ� �α��� ����");
                    return;
                }
            }
            else
            {
                Debug.LogError("�ĺ� ������ �������� ����");
            }
        });
    }

    private void SignUp()
    {
        loginPanel.SetActive(false);
        signupPanel.SetActive(true);
    }

    #region Login
    private void Login()
    {
        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email.text, pw.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"�α��� ����: {task.Exception}");
                Manager.UI.PopUpUI.Show("Login Failed");
                return;
            }        

            Debug.Log($"�α��� ����: {task.Result.User.Email}");
            loginButton.interactable = false;

            PhotonNetwork.ConnectUsingSettings();
        });
    }
    
    public IEnumerator LoginRoutine()
    {
        if (Manager.Database == null || Manager.Database.userRef == null)
        {
            Debug.LogError("Database or userRef is null!");
            Manager.Database.Init();
        }

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

        PhotonNetwork.LocalPlayer.SetUID(FirebaseManager.Auth.CurrentUser.UserId);

        PhotonNetwork.LocalPlayer.NickName = Manager.Data.PlayerData.Name;
        yield return new WaitForSeconds(1);
        SceneManager.LoadSceneAsync("Lobby");
    }
    #endregion

    #region PhotonCallbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Master Connected");

        StartCoroutine(LoginRoutine());
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Connected");
        PhotonNetwork.ConnectUsingSettings();
    }
    #endregion

    private void LoginSetActive(bool active) => loginPanel.SetActive(active);
}
