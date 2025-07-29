using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NickNameSelectPanel : MonoBehaviour
{
    [SerializeField] TMP_InputField nickName;
    [SerializeField] Button selectButton;
    [SerializeField] Button closeButton;

    #region LifeCycle
    private void Start()
    {
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    private void OnEnable()
    { 
        selectButton.onClick.AddListener(NickNameSelect);
        selectButton.interactable = true;
    }

    private void OnDisable()
    {
        selectButton.onClick.RemoveListener(NickNameSelect);
    }
    #endregion

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void NickNameSelect()
    {
        if (string.IsNullOrWhiteSpace(nickName.text))
        {
            Manager.UI.PopUpUI.Show("IsNullOrWhiteSpace", Color.red);
            return;
        }

        selectButton.interactable = false;

        PhotonNetwork.NickName = nickName.text;
        Manager.Data.PlayerData.Name = nickName.text;
        
        FirebaseUser user = FirebaseManager.Auth.CurrentUser;
        UserProfile profile = new UserProfile { DisplayName = nickName.text };

        Manager.Database.userDataRef.Child("Name").SetValueAsync(nickName.text).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
                return;

            Debug.Log($"닉네임 설정완료 : {nickName.text}");
        });

        user.UpdateUserProfileAsync(profile).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsCanceled && !task.IsFaulted)
            {
                Debug.Log("닉네임 설정 완료");
                PhotonNetwork.NickName = nickName.text;
                Manager.UI.PopUpUI.Show("닉네임 설정 완료!", Color.green);
            }
            else
            {
                Manager.UI.PopUpUI.Show("닉네임 설정 실패", Color.red);
                Debug.LogError(task.Exception);
            }
        });        

        gameObject.SetActive(false);
    }    
}
