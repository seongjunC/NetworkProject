using Firebase.Auth;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AccountDeletePanel : MonoBehaviour
{
    [SerializeField] Button deleteButton;
    [SerializeField] Button closeButton;

    [SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField password;
    [SerializeField] TMP_InputField passwordCheck;

    private void Start()
    {
        closeButton.onClick.AddListener(Close);
        deleteButton.onClick.AddListener(CheckDelete);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Close() => gameObject.SetActive(false);

    private void CheckDelete()
    {
        if (string.IsNullOrWhiteSpace(email.text) || string.IsNullOrWhiteSpace(password.text) || string.IsNullOrWhiteSpace(passwordCheck.text))
        {
            Manager.UI.PopUpUI.Show("�ʵ带 �Է����ּ���.");
            return;
        }

        if (password.text != passwordCheck.text)
        {
            Manager.UI.PopUpUI.Show("��й�ȣ�� ��ġ���� �ʽ��ϴ�.");            
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
            Manager.UI.PopUpUI.Show($"���� : {loginTask.Exception.GetBaseException().Message}");            
            yield break;
        }

        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        var deleteTask = user.DeleteAsync();

        yield return new WaitUntil(() => deleteTask.IsCompleted);

        if (deleteTask.Exception != null)
        {
            Manager.UI.PopUpUI.Show($"���� : {deleteTask.Exception.GetBaseException().Message}");
        }
        else
        {
            Manager.UI.PopUpUI_Action.Show("������ ������ �����Ͻðڽ��ϱ�?", DeleteAccount);
        }
    }

    private void DeleteAccount()
    {
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;

        if (user != null)
        {
            user.DeleteAsync().ContinueWith(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.LogError($"���� ���� ����: {task.Exception?.GetBaseException().Message}");
                    return;
                }
                
                Manager.Firebase.LogOut();
            });
        }
        else
        {
            Debug.LogWarning("�α��ε� ����ڰ� �����ϴ�.");
        }
    }
}
