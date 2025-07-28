using Database;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject loginMessage;

    [SerializeField] bool isTest;
    private FirebaseUser user;
    private bool isLogin = false;

    #region LifeCycle
    public override void OnEnable()
    {
        base.OnEnable();
        Manager.UI.FadeScreen.FadeOut(1);
        Manager.Game.State = Game.State.Login;

        loginMessage.SetActive(false);
        loginPanel.SetActive(true);
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
                if (!isTest)
                {
                    FirebaseAuth auth = FirebaseManager.Auth;
                    FirebaseDatabase database = FirebaseManager.Database;

                    if (auth.CurrentUser != null && auth.CurrentUser.IsEmailVerified)
                    {
                        Debug.Log("�ڵ� �α��� ��ȿ��: " + auth.CurrentUser.Email);
                        user = auth.CurrentUser;

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
        if (isLogin) return;
        isLogin = true;
        loginButton.interactable = false;
        loginMessage.SetActive(true);
        loginPanel.SetActive(false);
        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email.text, pw.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"�α��� ����: {task.Exception}");
                Manager.UI.PopUpUI.Show("Login Failed");
                isLogin = false;
                loginButton.interactable = true;
                loginMessage.SetActive(false);
                loginPanel.SetActive(true);
                return;
            }

            user = task.Result.User;

            user.ReloadAsync().ContinueWithOnMainThread(reloadTask =>
            {
                if (reloadTask.IsFaulted || reloadTask.IsCanceled)
                    return;

                if (!user.IsEmailVerified)
                {
                    user.SendEmailVerificationAsync().ContinueWithOnMainThread(sendTaskEmail =>
                    {
                        if (sendTaskEmail.IsCanceled || sendTaskEmail.IsFaulted)
                            return;

                        Debug.Log("�̸��� ������");
                    });
                    loginMessage.SetActive(false);
                    loginPanel.SetActive(true);

                    Manager.UI.PopUpUI.Show("�̸��� ������ �ʿ��մϴ�. ������ Ȯ�����ּ���.", Color.yellow);
                    FirebaseManager.Auth.SignOut();
                    isLogin = false;
                    loginButton.interactable = true;
                    return;
                }

                Debug.Log($"�α��� ����: {task.Result.User.Email}");
                PhotonNetwork.ConnectUsingSettings();
                Manager.UI.FadeScreen.FadeIn(1);
            });
        });
    }

    public IEnumerator LoginRoutine()
    {
        Manager.Database.Init();

        var task = Manager.Database.userRef.GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);

        bool connected = false;

        Manager.Database.userRef.Child(UserDataType.Connected.ToString()).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Firebase ����: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Debug.Log("Exists");
                    connected = (bool)snapshot.Value;
                }
                else
                {
                    Debug.Log("Not Exists");
                    Manager.Database.userRef.Child(UserDataType.Connected.ToString()).SetValueAsync(false);                    
                }
            }
        });

        yield return new WaitForSeconds(.5f);

        if(connected)
        {
            Manager.UI.PopUpUI.Show("�̹� �������� �����Դϴ�.",Color.red);
            isLogin = false;
            loginButton.interactable = true;
            loginMessage.SetActive(false);
            loginPanel.SetActive(true);
            Manager.UI.FadeScreen.FadeOut(1);
            yield break;
        }

        
        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.LogError("Firebase ������ �������� ����");
            isLogin = false;
            loginButton.interactable = true;
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

        PhotonNetwork.LocalPlayer.SetUID(user.UserId);

        PhotonNetwork.LocalPlayer.NickName = Manager.Data.PlayerData.Name;
        Manager.Data.PlayerData.Init();
        yield return new WaitForSeconds(1);

        gameObject.SetActive(false);
        lobbyPanel.SetActive(true);
        PhotonNetwork.JoinLobby();
        Manager.UI.FadeScreen.FadeOut(1);
        Manager.Game.State = Game.State.Lobby;
        Manager.Database.userRef.Child(UserDataType.Connected.ToString()).SetValueAsync(true);
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
