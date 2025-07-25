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
using System.Runtime.CompilerServices;

public class TestLoginManager : MonoBehaviourPunCallbacks
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
    [SerializeField] GameObject lobbyPanel;

    private FirebaseUser user;
    private bool isLogin = false;

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
        pw.onEndEdit.AddListener(EnterLogin);
        email.onEndEdit.AddListener(EnterLogin);
        loginButton.onClick.AddListener(Login);
        signupButton.onClick.AddListener(SignUp);
        Manager.Firebase.OnAuthSettingComplated += StartRoutine;
    }
    private void UnSubscribe()
    {
        pw.onEndEdit.RemoveListener(EnterLogin);
        email.onEndEdit.RemoveListener(EnterLogin);
        loginButton.onClick.RemoveListener(Login);
        signupButton.onClick.RemoveListener(SignUp);
        Manager.Firebase.OnAuthSettingComplated -= StartRoutine;
    }
    #endregion

    private void Init()
    {
        email.text = "";
        pw.text = "";
        isLogin = false;
    }

    private void EnterLogin(string s)
    {
        if (Input.GetKeyDown(KeyCode.Return))
            Login();
    }

    public void StartRoutine()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(t =>
        {
            DependencyStatus status = t.Result;

            if (status == DependencyStatus.Available)
            {
 
            }
            else
            {
                Debug.LogError("파베 설정이 충족되지 않음");
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
        if (isLogin) return;

        isLogin = true;
        loginButton.interactable = false;
        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email.text, pw.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"로그인 실패: {task.Exception}");
                Manager.UI.PopUpUI.Show("Login Failed");
                isLogin = false;
                loginButton.interactable = true;
                return;
            }

            user = task.Result.User;

            if (!user.IsEmailVerified)
            {
                Manager.UI.PopUpUI.Show("이메일 인증이 필요합니다. 메일을 확인해주세요.", Color.yellow);
                FirebaseManager.Auth.SignOut();
                isLogin = false;
                loginButton.interactable = true;
                return;
            }

            Debug.Log($"로그인 성공: {task.Result.User.Email}");

            PhotonNetwork.ConnectUsingSettings();
        });
    }

    public IEnumerator LoginRoutine()
    {
        Manager.Database.Init();

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

        PhotonNetwork.LocalPlayer.SetUID(user.UserId);
        PhotonNetwork.LocalPlayer.NickName = Manager.Data.PlayerData.Name;
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
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
