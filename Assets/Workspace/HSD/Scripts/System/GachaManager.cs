using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[Serializable]
public class GachaManager
{
    private List<GachaResult> gachaResults = new List<GachaResult>();
    private DatabaseReference gachaRef;

    public event Action<GachaResult> OnGachaResultChanged;

    public void Init()
    {
        InitGachaData();
    }

    private void InitGachaData()
    {
        gachaRef = Manager.Database.userRef.Child("GachaResults");

        Manager.Database.userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            var snapShot = task.Result;

            bool exists = false;

            if (snapShot.ChildrenCount > 0)
            {
                foreach (var child in snapShot.Children)
                {
                    if (child.Key == "GachaResults")
                    {
                        exists = true;
                        break;
                    }
                }
            }

            if (!exists)
                gachaRef.SetValueAsync("");
            else
                InitData();

            RegisterGachaListeners();
        });
    }

    private void RegisterGachaListeners()
    {
        gachaRef.ChildAdded += OnGachaResultAdded;
    }

    private void InitData()
    {
        gachaRef.OrderByChild("Timestamp").GetValueAsync()
    .ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted || task.IsCanceled)
        {
            Debug.LogError("가챠 데이터 가져오기 실패");
            return;
        }

        DataSnapshot snapshot = task.Result;

        foreach (var child in snapshot.Children)
        {
            string data = child.GetRawJsonValue();
            GachaResult gachaResult = JsonUtility.FromJson<GachaResult>(data);
            gachaResults.Add(gachaResult);
        }

        // Firebase는 기본적으로 오름차순 정렬
        // 최신순 = Reverse
        gachaResults.Reverse();
    });
    }

    #region Events

    private void OnGachaResultAdded(object sender, ChildChangedEventArgs args)
    {
        string data = args.Snapshot.GetRawJsonValue();
        GachaResult result = JsonUtility.FromJson<GachaResult>(data);    
        
        gachaResults.Add(result);

        OnGachaResultChanged?.Invoke(result);
    }

    #endregion

    public void AddGachaResult(string time, string name)
    {
        GachaResult result = new GachaResult
        {
            Time = time,
            Name = name,
            Timestamp = DateTimeOffset.Parse(time).ToUnixTimeMilliseconds()
        };

        string json = JsonUtility.ToJson(result);
        string key = $"{time}_{name}_{Guid.NewGuid().ToString()}";
        gachaRef.Child(key).SetRawJsonValueAsync(json);
    }

    public void GachaResultsOrderBy()
    {
        gachaResults = gachaResults
        .OrderByDescending(r => DateTime.Parse(r.Time))
        .ToList();
    }
}
