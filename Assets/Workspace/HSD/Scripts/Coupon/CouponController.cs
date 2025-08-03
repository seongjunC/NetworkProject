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
    [SerializeField] Button closeButton;
    [SerializeField] CouponRewordPanel couponRewordPanel;

    private void Start()
    {
        checkButton.onClick.AddListener(Check);
        closeButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void Check()
    {
        checkButton.interactable = false;

        string inputKey = couponKey.text.Trim();
        if (string.IsNullOrEmpty(inputKey))
        {
            ShowError("쿠폰 코드를 입력하세요.");
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
                ShowError("서버 오류: 쿠폰 목록을 가져올 수 없습니다.");
                return;
            }

            DataSnapshot allCoupons = task.Result;

            if (!allCoupons.HasChild(key))
            {
                ShowError("유효하지 않은 쿠폰 번호입니다.");
                return;
            }

            // 쿠폰 유효 → 유저가 이미 사용했는지 확인
            CheckCouponUsage(key);
        });
    }

    private void CheckCouponUsage(string key)
    {
        Manager.Database.userRef.Child("Coupons").Child(key).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                ShowError("서버 오류: 쿠폰 사용 여부 확인 실패");
                return;
            }

            DataSnapshot snapshot = task.Result;
            Coupon couponData;

            if (snapshot.Exists)
            {
                // 이미 사용한 쿠폰일 수도 있음
                couponData = JsonUtility.FromJson<Coupon>(snapshot.GetRawJsonValue());

                if (couponData.IsUse)
                {
                    ShowError("이미 사용된 쿠폰입니다.");
                    return;
                }
            }
            else
            {
                // 새로운 쿠폰 등록
                couponData = new Coupon(key);
                string json = JsonUtility.ToJson(couponData);

                Manager.Database.userRef.Child("Coupons").Child(key).SetRawJsonValueAsync(json).ContinueWithOnMainThread(setTask =>
                {
                    if (setTask.IsFaulted || setTask.IsCanceled)
                    {
                        ShowError("쿠폰 등록 실패");
                        return;
                    }

                    Debug.Log("쿠폰 등록 성공");
                    GrantCouponReward(key, couponData);
                });

                return; // 보상은 Set이 완료되면 실행됨
            }

            // 기존 쿠폰: 사용 안 했으므로 보상 지급
            GrantCouponReward(key, couponData);
        });
    }

    private void GrantCouponReward(string key, Coupon couponData)
    {
        Dictionary<string, int> dic = new Dictionary<string, int>();

        Manager.Database.root.Child("Coupon").Child(key).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                ShowError("보상 정보를 불러오지 못했습니다.");
                return;
            }

            DataSnapshot rewardSnapshot = task.Result;
            Dictionary<string, object> rewardData = rewardSnapshot.Value as Dictionary<string, object>;

            if (rewardData == null)
            {
                ShowError("쿠폰 보상 데이터 오류");
                return;
            }

            foreach (var kvp in rewardData)
            {
                if (int.TryParse(kvp.Value.ToString(), out int value))
                {
                    if (kvp.Key == "Gem")
                    {
                        Manager.Data.PlayerData.GemGain(value);
                        Debug.Log($"보상: Gem {value} 지급");
                    }
                    else
                    {
                        Manager.Data.TankInventoryData.AddTankEvent(kvp.Key, value);
                        Debug.Log($"보상: {kvp.Key} 유닛 {value}개 지급");
                    }

                    dic[kvp.Key] = value;
                }
            }

            // 사용 표시 저장
            couponData.IsUse = true;
            string updatedJson = JsonUtility.ToJson(couponData);

            Manager.Database.userRef.Child("Coupons").Child(key).SetRawJsonValueAsync(updatedJson).ContinueWithOnMainThread(setTask =>
            {
                if (setTask.IsFaulted || setTask.IsCanceled)
                {
                    Debug.LogWarning("쿠폰 사용 표시 실패");
                }
                else
                {
                    Debug.Log("쿠폰 사용 완료");
                    Manager.UI.PopUpUI.Show("쿠폰 보상이 지급되었습니다!", Color.green);
                }

                checkButton.interactable = true;
            });
        });

        couponRewordPanel.CreateSlot(dic);
    }

    private void ShowError(string message)
    {
        Manager.UI.PopUpUI.Show(message, Color.red);
        checkButton.interactable = true;
    }
}
