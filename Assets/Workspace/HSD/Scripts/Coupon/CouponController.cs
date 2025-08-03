using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CouponController : MonoBehaviour
{
    [SerializeField] TMP_InputField couponKey;
    [SerializeField] Button checkButton;

    private void Start()
    {
        checkButton.onClick.AddListener(Check);
    }

    public void Check()
    {
        checkButton.interactable = false;

        string inputKey = couponKey.text.Trim();
        if (string.IsNullOrEmpty(inputKey))
        {
            ShowError("���� �ڵ带 �Է��ϼ���.");
            return;
        }

        ValidateCoupon(inputKey);
    }

    private void ValidateCoupon(string key)
    {
        Manager.Database.root.Child("Coupon").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                ShowError("���� ����: ���� ����� ������ �� �����ϴ�.");
                return;
            }

            DataSnapshot allCoupons = task.Result;

            if (!allCoupons.HasChild(key))
            {
                ShowError("��ȿ���� ���� ���� ��ȣ�Դϴ�.");
                return;
            }

            // ���� ��ȿ �� ������ �̹� ����ߴ��� Ȯ��
            CheckCouponUsage(key);
        });
    }

    private void CheckCouponUsage(string key)
    {
        Manager.Database.userRef.Child("Coupons").Child(key).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                ShowError("���� ����: ���� ��� ���� Ȯ�� ����");
                return;
            }

            DataSnapshot snapshot = task.Result;
            Coupon couponData;

            if (snapshot.Exists)
            {
                // �̹� ����� ������ ���� ����
                couponData = JsonUtility.FromJson<Coupon>(snapshot.GetRawJsonValue());

                if (couponData.IsUse)
                {
                    ShowError("�̹� ���� �����Դϴ�.");
                    return;
                }
            }
            else
            {
                // ���ο� ���� ���
                couponData = new Coupon(key);
                string json = JsonUtility.ToJson(couponData);

                Manager.Database.userRef.Child("Coupons").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(setTask =>
                {
                    if (setTask.IsFaulted || setTask.IsCanceled)
                    {
                        ShowError("���� ��� ����");
                        return;
                    }

                    Debug.Log("���� ��� ����");
                    GrantCouponReward(key, couponData);
                });

                return; // ������ Set�� �Ϸ�Ǹ� �����
            }

            // ���� ����: ��� �� �����Ƿ� ���� ����
            GrantCouponReward(key, couponData);
        });
    }

    private void GrantCouponReward(string key, Coupon couponData)
    {
        Manager.Database.root.Child("Coupon").Child(key).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                ShowError("���� ������ �ҷ����� ���߽��ϴ�.");
                return;
            }

            DataSnapshot rewardSnapshot = task.Result;
            Dictionary<string, object> rewardData = rewardSnapshot.Value as Dictionary<string, object>;

            if (rewardData == null)
            {
                ShowError("���� ���� ������ ����");
                return;
            }

            foreach (var pair in rewardData)
            {
                if (int.TryParse(pair.Value.ToString(), out int value))
                {
                    if (pair.Key == "Gem")
                    {
                        Manager.Data.PlayerData.GemGain(value);
                        Debug.Log($"����: Gem {value} ����");
                    }
                    else
                    {
                        Manager.Data.TankInventoryData.AddTankEvent(pair.Key, value);
                        Debug.Log($"����: {pair.Key} ���� {value}�� ����");
                    }
                }
            }

            // ��� ǥ�� ����
            couponData.IsUse = true;
            string updatedJson = JsonUtility.ToJson(couponData);

            Manager.Database.userRef.Child("Coupons").Child(key).SetRawJsonValueAsync(updatedJson).ContinueWithOnMainThread(setTask =>
            {
                if (setTask.IsFaulted || setTask.IsCanceled)
                {
                    Debug.LogWarning("���� ��� ǥ�� ����");
                }
                else
                {
                    Debug.Log("���� ��� �Ϸ�");
                    Manager.UI.PopUpUI.Show("���� ������ ���޵Ǿ����ϴ�!", Color.green);
                }

                checkButton.interactable = true;
            });
        });
    }

    private void ShowError(string message)
    {
        Manager.UI.PopUpUI.Show(message, Color.red);
        checkButton.interactable = true;
    }
}
