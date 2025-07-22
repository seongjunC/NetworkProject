using Firebase.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [Header("InputFields")]
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField pw;

    [Header("Buttons")]
    [SerializeField] Button loginButton;
    [SerializeField] Button signupButton;

    [Header("Panels")]
    [SerializeField] GameObject signupPanel;

    #region LifeCycle
    private void OnEnable()
    {
        Init();
        Subscribe();
        loginButton.interactable = true;
    }
    private void OnDisable()
    {
        UnSubscribe();
    }
    #endregion

    #region EventSubscribe
    private void Subscribe()
    {
        loginButton.onClick.AddListener(Login);
        signupButton.onClick.AddListener(SignUp);
    }
    private void UnSubscribe()
    {
        loginButton.onClick.RemoveListener(Login);
        signupButton.onClick.RemoveListener(SignUp);
    }
    #endregion

    private void Init()
    {
        email.text = "";
        pw.text = "";
    }

    #region Login
    private void Login()
    {
        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email.text, pw.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError($"로그인 실패: {task.Exception}");
                Manager.UI.PopUpUI.Show("Login Failed");
                return;
            }

            Debug.Log($"로그인 성공: {task.Result.User.Email}");
            loginButton.interactable = false;
            StartCoroutine(Manager.Firebase.LoginRoutine());
        });
    }
       
    #endregion
    private void SignUp()
    {
        gameObject.SetActive(false);
        signupPanel.SetActive(true);
    }
}
