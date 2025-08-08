using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class GachaManager
{
    public GachaData GachaData;
    public List<GachaResult> gachaResults = new List<GachaResult>();
    private DatabaseReference gachaRef;

    public event Action<GachaResult> OnGachaResultChanged;

    public void Init()
    {
        InitGachaData();
        gachaResults.Clear();
        GachaData = Resources.Load<GachaData>("Data/Gacha/GachaData");
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
            {
                gachaRef.SetValueAsync("");
            }

            RegisterGachaListeners();          
        });
    }

    private void RegisterGachaListeners()
    {
        gachaRef.ChildAdded += OnGachaResultAdded;
    }

    #region Events

    private void OnGachaResultAdded(object sender, ChildChangedEventArgs args)
    {
        string data = args.Snapshot.GetRawJsonValue();
        GachaResult result = JsonUtility.FromJson<GachaResult>(data);
        Debug.Log("Add Event Invoke");        
        gachaResults.Add(result);

        OnGachaResultChanged?.Invoke(result);
    }

    #endregion

    public void AddGachaResult(string time, string name)
    {
        long _time = long.Parse(time);

        GachaResult result = new GachaResult
        {
            Time = _time,
            Name = name
        };

        string json = JsonUtility.ToJson(result);
        string key = $"{time}_{name}_{Guid.NewGuid().ToString()}";
        gachaRef.Child(key).SetRawJsonValueAsync(json)
    .ContinueWithOnMainThread(task =>
    {
        if (task.IsFaulted)
            Debug.LogError($"GachaResult 저장 실패: {task.Exception}");
        else if (task.IsCanceled)
            Debug.LogWarning(" GachaResult 저장 취소됨");
        else
            Debug.Log($"GachaResult 저장 성공: {key}");
    });
    }

    public void GachaResultsOrderBy()
    {
        gachaResults = gachaResults
            .OrderByDescending(r => r.Time)
            .ToList();
    }
}
