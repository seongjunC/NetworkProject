using Firebase.Auth;
using Firebase.Extensions;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUpPanel : MonoBehaviour
{
    [Header("InputFields")]
    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField pw;
    [SerializeField] TMP_InputField pwCheck;

    [Header("Panels")]
    [SerializeField] GameObject loginPanel;

    [Header("Buttons")]
    [SerializeField] Button loginPanelButton;
    [SerializeField] Button createButton;
    [SerializeField] Button emailCheckButton;

    [Header("Text")]
    [SerializeField] TMP_Text checkText;
    [Space]

    private bool isEmailChecking;

    #region LifeCycle
    private void OnEnable()
    {
        isEmailChecking = false;
        Subscribe();
    }
    #endregion

    private void OnDisable()
    {
        UnSubscribe();
    }
    #region EventSubscribe
    private void Subscribe()
    {

    }
    private void UnSubscribe()
    {

    }
    #endregion

    private void CreateAccount()
    {
        if(pw.text != pwCheck.text)
        {
            Manager.UI.PopUpUI.Show("Password Not Equals",Color.red);
            return;
        }

        if (!isEmailChecking)
        {
            Manager.UI.PopUpUI.Show("Please Try Email Check", Color.yellow);
            return;
        }

        FirebaseManager.Auth.CreateUserWithEmailAndPasswordAsync(email.text, pw.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("회원가입 실패");
                Manager.UI.PopUpUI.Show("Failed!", Color.red);
                return;
            }
            else
            {
                Debug.Log("회원가입 성공");
                Manager.UI.PopUpUI.Show("Succes!", Color.green);
            }
        });
    }
    private void CheckEmail()
    {
        FirebaseManager.Auth.FetchProvidersForEmailAsync(email.text).ContinueWithOnMainThread(task =>
        {
            if(task.IsCanceled || task.IsFaulted)
            {
                Manager.UI.PopUpUI.Show($"Error : {task.Exception?.Flatten().InnerException.Message}", Color.red);
                return;
            }

            var providers = task.Result;

            if (providers != null && providers.Count() > 0)
            {
                checkText.color = Color.red;
                checkText.text = "X";
                Manager.UI.PopUpUI.Show("Error : An Account Already Exists");
            }
            else
            {
                checkText.color = Color.green;
                checkText.text = "O";
                Manager.UI.PopUpUI.Show("This is An Available Email");
            }
        });
    }
    private void SwitchLoginPanel()
    {
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
