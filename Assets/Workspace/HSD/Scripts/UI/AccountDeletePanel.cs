using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountDeletePanel : MonoBehaviour
{
    [SerializeField] Button deleteButton;

    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;
    [SerializeField] TMP_InputField passwordCheck;

    private void Start()
    {
        deleteButton.onClick.AddListener(CheckDelete);
    }

    private void CheckDelete()
    {
        if (string.IsNullOrWhiteSpace(email.text) || string.IsNullOrWhiteSpace(password.text) || string.IsNullOrWhiteSpace(passwordCheck.text))
        {
            Manager.UI.PopUpUI.Show("필드를 입력해주세요.");
            return;
        }

        if (password.text != passwordCheck.text)
        {
            Manager.UI.PopUpUI.Show("비밀번호가 일치하지 않습니다.");            
            return;
        }

        StartCoroutine(DeleteAccountRoutine(email.text, password.text));
    }

    private IEnumerator DeleteAccountRoutine(string email, string password)
    {
        var loginTask = FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Manager.UI.PopUpUI.Show($"실패 : {loginTask.Exception.GetBaseException().Message}");            
            yield break;
        }

        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        var deleteTask = user.DeleteAsync();

        yield return new WaitUntil(() => deleteTask.IsCompleted);

        if (deleteTask.Exception != null)
        {
            Manager.UI.PopUpUI.Show($"실패 : {deleteTask.Exception.GetBaseException().Message}");
        }
    }
}
