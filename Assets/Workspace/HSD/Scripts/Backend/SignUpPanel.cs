using Firebase;
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

    [Header("Check")]
    [SerializeField] Sprite checkSprite;
    [SerializeField] Sprite xSprite;
    [SerializeField] Image emailCheckImage;
    [SerializeField] Image pwCheckImage;
    [SerializeField] Image pwCCheckImage;
    [Space]

    private bool isEmailChecking;

    #region LifeCycle
    private void OnEnable()
    {
        Init();
        InitChecking();
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
        pw.onValueChanged.AddListener(PasswordCheck);
        pwCheck.onValueChanged.AddListener(PasswordConfirm);

        loginPanelButton.onClick.AddListener(SwitchLoginPanel);
        createButton    .onClick.AddListener(CreateAccount);
        emailCheckButton.onClick.AddListener(CheckEmail);
    }
    private void UnSubscribe()
    {
        pw.onValueChanged.RemoveListener(PasswordCheck);
        pwCheck.onValueChanged.RemoveListener(PasswordConfirm);

        loginPanelButton.onClick.RemoveListener(SwitchLoginPanel);
        createButton    .onClick.RemoveListener(CreateAccount);
        emailCheckButton.onClick.RemoveListener(CheckEmail);
    }
    #endregion

    private void Init()
    {
        email.text      = "";
        pw.text         = "";
        pwCheck.text    = "";        
    }

    private void InitChecking()
    {
        isEmailChecking = false;        
        emailCheckImage.sprite  = null;
        pwCCheckImage.sprite    = null;
        pwCheckImage.sprite     = null;
        
        emailCheckImage.color   = Color.clear;
        pwCCheckImage.color     = Color.clear;
        pwCheckImage.color      = Color.clear;
    }
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
                var exception = task.Exception.Flatten().InnerExceptions[0] as FirebaseException;
                AuthError error = (AuthError)exception.ErrorCode;

                if (error == AuthError.EmailAlreadyInUse)
                {
                    Manager.UI.PopUpUI.Show("이미 존재하는 계정입니다.");
                }
                else if (error == AuthError.InvalidEmail)
                {
                    Manager.UI.PopUpUI.Show("이메일 형식이 잘못되었습니다.");
                }
                else if (error == AuthError.WeakPassword)
                {
                    Manager.UI.PopUpUI.Show("비밀번호는 6자 이상이어야 합니다.");
                }

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
                emailCheckImage.sprite = xSprite;
                emailCheckImage.color = Color.white;                
                Manager.UI.PopUpUI.Show("Error : An Account Already Exists");
            }
            else
            {
                emailCheckImage.sprite = checkSprite;
                emailCheckImage.color = Color.white;
                isEmailChecking = true;
                Manager.UI.PopUpUI.Show("This is An Available Email");
            }
        });
    }

    private void PasswordCheck(string s)
    {
        if(s.Length >= 6)
        {
            pwCheckImage.sprite = checkSprite;
            pwCheckImage.color = Color.white;
        }
        else
        {
            pwCheckImage.sprite = xSprite;
            pwCheckImage.color = Color.white;
        }
    }

    private void PasswordConfirm(string s)
    {
        if(pw.text.Length >= 6 && (s == pw.text))
        {
            pwCCheckImage.sprite = checkSprite;
            pwCheckImage.color = Color.white;
        }
        else
        {
            pwCCheckImage.sprite = xSprite;
            pwCheckImage.color = Color.white;
        }
    }

    private void SwitchLoginPanel()
    {
        loginPanel.SetActive(true);
        gameObject.SetActive(false);
    }
}
