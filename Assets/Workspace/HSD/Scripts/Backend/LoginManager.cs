using Database;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
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
    [SerializeField] Button gameOutButton;

    [Header("Panels")]
    [SerializeField] GameObject signupPanel;
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject loginMessage;

    [SerializeField] bool isTest;
    [SerializeField] TankData firstTank;
    private FirebaseUser user;
    private bool isLogin = false;
    private Sprite loading;

    #region LifeCycle
    public override void OnEnable()
    {
        base.OnEnable();
        if (Manager.Game.State == Game.State.Game)
            return;        

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
        gameOutButton.onClick.AddListener(GameOut);
        Manager.Firebase.OnAuthSettingComplated += StartRoutine;
    }
    private void UnSubscribe()
    {
        pw.onEndEdit.RemoveListener(EnterLogin);
        email.onEndEdit.RemoveListener(EnterLogin);
        loginButton.onClick.RemoveListener(Login);
        signupButton.onClick.RemoveListener(SignUp);
        gameOutButton.onClick.RemoveListener(GameOut);
        Manager.Firebase.OnAuthSettingComplated -= StartRoutine;
    }
    #endregion

    private void Init()
    {
        email.text = "";
        pw.text = "";
        isLogin = false;
        user = null;
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
                        Debug.Log(auth.CurrentUser.Email);
                        user = auth.CurrentUser;

                        LoginSetActive(user == null);

                        if (user != null)
                        {
                            PhotonNetwork.ConnectUsingSettings();
                        }
                    }
                    else
                    {
                        return;
                    }
                }
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
        loading = DataManager.loadingImage;
        isLogin = true;
        loginButton.interactable = false;
        loginMessage.SetActive(true);
        loginPanel.SetActive(false);

        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email.text, pw.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Manager.UI.PopUpUI.Show("로그인에 실패하였습니다.");
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
                    });

                    loginMessage.SetActive(false);
                    loginPanel.SetActive(true);

                    Manager.UI.PopUpUI.Show("이메일을 확인해주세요.", Color.yellow);
                    FirebaseManager.Auth.SignOut();
                    isLogin = false;
                    loginButton.interactable = true;
                    return;
                }

                PhotonNetwork.ConnectUsingSettings();
                Manager.UI.FadeScreen.FadeIn(1, loading);
            });
        });
    }

    public IEnumerator LoginRoutine()
    {
        Manager.Database.Init();

        var task = Manager.Database.root.Child("UserData").Child(user.UserId).Child("Data").GetValueAsync();
        yield return new WaitUntil(() => task.IsCompleted);
        bool connected = false;
        Debug.Log(FirebaseManager.Auth.CurrentUser.Email);
        Debug.Log(user.Email);

        if (user.Email != FirebaseManager.Auth.CurrentUser.Email)
            user = FirebaseManager.Auth.CurrentUser;

        Manager.Database.root.Child("UserData").Child(user.UserId).Child(UserDataType.Connected.ToString()).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                PhotonNetwork.Disconnect();
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

        if (connected)
        {
            Manager.UI.PopUpUI.Show("이미 접속중인 계정입니다.", Color.red);
            isLogin = false;
            loginButton.interactable = true;
            loginMessage.SetActive(false);
            loginPanel.SetActive(true);
            PhotonNetwork.Disconnect();
            Manager.UI.FadeScreen.FadeOut(1, loading);
            yield break;
        }
        
        if (task.IsFaulted || task.IsCanceled)
        {
            isLogin = false;
            loginMessage.SetActive(false);
            loginPanel.SetActive(true);
            loginButton.interactable = true;
            PhotonNetwork.Disconnect();
            yield break;
        }

        var snapshot = task.Result;
        string json = snapshot?.GetRawJsonValue();
        PlayerData newData = JsonUtility.FromJson<PlayerData>(json);
        
        if (newData == null || string.IsNullOrEmpty(newData.Name))
        {
            newData = new PlayerData();
            Manager.Data.PlayerData = newData;

            string saveJson = JsonUtility.ToJson(newData);
            Manager.Database.root.Child("UserData").Child(user.UserId).Child("Data").SetRawJsonValueAsync(saveJson);
        }
        else
        {
            Manager.Data.PlayerData = newData;
        }

        PhotonNetwork.LocalPlayer.SetUID(user.UserId);
        PhotonNetwork.LocalPlayer.NickName = Manager.Data.PlayerData.Name;

        Manager.Data.PlayerData.Init();
        

        yield return new WaitForSeconds(1);

        Manager.Game.State = Game.State.Lobby;
        Manager.Database.userRef.Child(UserDataType.Connected.ToString()).SetValueAsync(true);
        Manager.Database.userRef.Child(UserDataType.Connected.ToString()).OnDisconnect().SetValue(false);
        Manager.Data.Init();
        Manager.Data.GachaManager.GachaData.InitPickUp();
        SetUpSelectTank();

        if (string.IsNullOrEmpty(Manager.Data.PlayerData.Name))
        {
            Manager.Data.TankInventoryData.AddTankEvent(firstTank.tankName, 1);
            Manager.UI.NickNameSelectPanel.Show();
        }

        yield return new WaitForSeconds(1);

        PhotonNetwork.JoinLobby();
        Manager.UI.FadeScreen.FadeOut(1, loading);        

        gameObject.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    #endregion

    private void SetUpSelectTank()
    {        
        Manager.Database.userRef.Child("SelectTank").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if(!task.Result.Exists)
            {
                return;
            }

            if (task.IsCompleted)
            {
                Manager.Data.TankDataController.SetSelectTank((string)task.Result.Value);
            }
        });
    }

    #region PhotonCallbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Master Connected");

        StartCoroutine(LoginRoutine());
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("Disconnected");        
    }
    private void GameOut() => Application.Quit();
    #endregion

    private void LoginSetActive(bool active) => loginPanel.SetActive(active);
}
