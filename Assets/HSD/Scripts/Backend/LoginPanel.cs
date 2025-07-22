using Firebase.Extensions;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [Header("InputFields")]
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField pw;

    [Header("Buttons")]
    [SerializeField] Button loginButton;
    [SerializeField] Button signupButton;
    [SerializeField] Button googleLoginButton;

    [Header("Panels")]
    [SerializeField] GameObject signupPanel;

    #region LifeCycle
    private void OnEnable()
    {
        Subscribe();
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
        googleLoginButton.onClick.AddListener(GoogleLogin);
    }
    private void UnSubscribe()
    {
        loginButton.onClick.RemoveListener(Login);
        signupButton.onClick.RemoveListener(SignUp);
        googleLoginButton.onClick.RemoveListener(GoogleLogin);
    }
    #endregion

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
            StartCoroutine(LoginRoutine());
        });
    }
    private IEnumerator LoginRoutine()
    {
        yield return null;
    }
    private void SignUp()
    {
        gameObject.SetActive(false);
        signupPanel.SetActive(true);
    }

    private void GoogleLogin()
    {

    }
}
