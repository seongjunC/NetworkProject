using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CouponController : MonoBehaviour
{
    [SerializeField] TMP_InputField couponKey;
    [SerializeField] Button checkButton;

    public Coupon[] Coupons;
    private Coupon newCoupon;

    private void Start()
    {
        checkButton.onClick.AddListener(Check);
    }

    public void Check()
    {
        StartCoroutine(CheckRoutine());
    }

    private IEnumerator CheckRoutine()
    {
        bool isUse = false;
        bool isSame = false;

        checkButton.interactable = false;

        foreach (var coupon in Coupons)
        {
            if (coupon.CouponKey == couponKey.text)
            {
                isSame = true;

                Manager.Database.userRef.Child("Coupons").Child(couponKey.text).GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    if (task.Result.Exists)
                    {
                        DataSnapshot snapshot = task.Result;

                        string json = snapshot.GetRawJsonValue();

                        newCoupon = JsonUtility.FromJson<Coupon>(json);

                        isUse = newCoupon.IsUse;
                    }
                    else
                    {
                        Manager.Database.userRef.Child("Coupons").Child(couponKey.text).SetRawJsonValueAsync(JsonUtility.ToJson(coupon)).ContinueWithOnMainThread(task =>
                        {
                            if (task.IsCanceled || task.IsFaulted)
                            {
                                Debug.Log("���� ���� ����");
                                checkButton.interactable = true;
                                return;
                            }

                            Debug.Log("���� ���� �Ϸ�");
                            newCoupon = coupon;
                        });
                    }
                });

                break;
            }
        }

        if (!isSame)
        {
            checkButton.interactable = true;
            yield break;
        }

        yield return new WaitForSeconds(.5f);

        if (isUse)
        {
            Debug.Log("�̹� ���� �����Դϴ�.");
            checkButton.interactable = true;
            yield break;
        }

        Debug.Log($"������ �Է��Ͽ� {newCoupon.Amount} ��ŭ�� ��ȭ�� ȹ���Ͽ����ϴ�!");
        newCoupon.IsUse = true;

        Manager.Database.userRef.Child("Coupons").Child(newCoupon.CouponKey).SetRawJsonValueAsync(JsonUtility.ToJson(newCoupon)).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.Log("���� ����ߴٴ� ���� ����");
                checkButton.interactable = true;
                return;
            }
            else if (task.IsCompleted)
            {
                checkButton.interactable = true;
            }
        });
    }
}
