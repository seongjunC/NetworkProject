using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;
public enum TankRank
{
    S, A, B, C
}

// 레벨에 따른 탱크 갯수를 관리하는 클래스 (데이터 베이스 저장용 데이터)
[Serializable]
public class TankGroupData
{
    public string TankName;
    public TankRank Rank;
    public int Level;
    public int Count;
}

[Serializable]
public class TankInventoryData
{
    public Dictionary<string, TankGroupData> tankGroups = new();
    private DatabaseReference tankRef;

    public event Action<string, int> OnTankCountUpdated; // (TankName, NewCount)
    public event Action<string, int> OnTankLevelRemoved;

    public void Init()
    {
        InitTanks();
    }

    private void RegisterTankListeners()
    {
        tankRef.ChildAdded += OnTankGroupAdded;
        tankRef.ChildChanged += OnTankGroupChanged;
        tankRef.ChildRemoved += OnTankGroupRemoved;
    }

    private void InitTanks()
    {
        tankRef = Manager.Database.userRef.Child("Tanks");
        RegisterTankListeners();

        Manager.Database.userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            var snapShot = task.Result;

            bool exists = false;

            if (snapShot.ChildrenCount > 0)
            {
                foreach (var child in snapShot.Children)
                {
                    if (child.Key == "Tanks")
                    {
                        exists = true;
                        break;
                    }
                }
            }
            
            if (!exists)
                tankRef.SetValueAsync("");

            InitData();
        });
    }

    private void InitData()
    {
        tankRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Firebase 실패");
                return;
            }

            int count = 0;
            foreach (var tank in task.Result.Children)
            {
                Debug.Log("추가");

                string json = tank.GetRawJsonValue();
                Debug.Log(json);
                TankGroupData group = JsonUtility.FromJson<TankGroupData>(json);
                tankGroups.Add(group.TankName, group);
                Debug.Log(group.TankName);
                count++;
            }
            Debug.Log($"{count}");
            Debug.Log("끝");
        });
    }

    private void OnTankGroupAdded(object sender, ChildChangedEventArgs args) => UpdateTankGroup(args.Snapshot);
    private void OnTankGroupChanged(object sender, ChildChangedEventArgs args)
    {
        string tankName = args.Snapshot.Key;
        if (!tankGroups.TryGetValue(tankName, out var groupData))
        {
            UpdateTankGroup(args.Snapshot);
            return;
        }

        var rankStr = args.Snapshot.Child("Rank")?.Value?.ToString();
        if (!string.IsNullOrEmpty(rankStr) && Enum.TryParse(rankStr, out TankRank rank))
        {
            groupData.Rank = rank;
        }

        var countStr = args.Snapshot.Child("Count")?.Value?.ToString();
        if (!string.IsNullOrEmpty(countStr) && int.TryParse(rankStr, out int count))
        {
            groupData.Count = count;
        }

        OnTankCountUpdated?.Invoke(tankName, groupData.Count);
    }
    private void OnTankGroupRemoved(object sender, ChildChangedEventArgs args)
    {
        string tankName = args.Snapshot.Key;
        tankGroups.Remove(tankName);
    }

    // 데이터 베이스에 추가가 되면 런타임 중 Dictionary에 추가하는 로직
    private void UpdateTankGroup(DataSnapshot snapshot)
    {
        if (!snapshot.Exists) return;

        string tankName = snapshot.Key; // 탱크 이름 가져옴

        var rankStr = snapshot.Child("Rank")?.Value?.ToString();    // 랭크 가져옴
        var rankParsed = Enum.TryParse(rankStr, out TankRank rank) ? rank : TankRank.C;

        var countStr = snapshot.Child("Count")?.Value?.ToString();        

        if(int.TryParse(countStr, out int count))
        {
            Debug.Log(count);
        }
        else
            count = 0;

        // 가져온 데이터를 기반으로 그룹을 만들어줌
        var groupData = new TankGroupData
        {
            TankName = tankName,
            Rank = rank,
            Count = count
        };

        OnTankCountUpdated?.Invoke(tankName, count);

        tankGroups[tankName] = groupData;
    }

    public void AddTankEvent(string tankName, int count, TankRank rank = TankRank.C)
    {
        Debug.Log("AddTank");
        tankRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Firebase 요청 실패");
                return;
            }

            var snapshot = task.Result;

            bool exists = false;

            if (snapshot.Exists)
            {
                foreach (var child in snapshot.Children)
                {
                    if (child.Key == tankName)
                    {
                        exists = true;
                        break;
                    }
                }
            }

            if (!exists)
            {
                Debug.Log($"새 탱크 생성 : {tankName}");
                CreateTankGroupData(tankName, rank, () =>
                {
                    AddCountAndUpgrade(tankName, count);
                });
            }
            else
            {
                AddCountAndUpgrade(tankName, count);
            }
        });
    }

    private void CreateTankGroupData(string name, TankRank rank, Action onComplete)
    {
        var data = new TankGroupData
        {
            TankName = name,
            Rank = rank,
            Count = 0
        };

        tankRef.Child(name).SetRawJsonValueAsync(JsonUtility.ToJson(data)).ContinueWithOnMainThread(_ => onComplete?.Invoke());
    }

    private void AddCountAndUpgrade(string name, int count)
    {
        var Ref = tankRef.Child(name).Child("Count");

        Ref.RunTransaction(mutableData =>
        {
            int current = mutableData.Value != null ? Convert.ToInt32(mutableData.Value) : 0;
            mutableData.Value = current + count;
            return TransactionResult.Success(mutableData);
        });
    }

    //public void UpgradeTank(string tankName)
    //{
    //    tankRef.Child(tankName).Child("Levels").GetValueAsync().ContinueWithOnMainThread(task =>
    //    {
    //        if (!task.IsCompleted || task.IsFaulted || task.IsCanceled)
    //            return;

    //        var snapshot = task.Result;
    //        if (!snapshot.Exists) return;

    //        foreach (var levelNode in snapshot.Children)
    //        {
    //            if (!int.TryParse(levelNode.Key, out int level))
    //                continue;

    //            int currentLevel = level;

    //            if (currentLevel >= maxLevel)
    //                return;

    //            var currentRef = tankRef.Child(tankName).Child("Levels").Child(currentLevel.ToString());

    //            currentRef.RunTransaction(mutableData =>
    //            {
    //                int count = 0;
    //                if (mutableData.Value != null)
    //                    int.TryParse(mutableData.Value.ToString(), out count);

    //                if (count < needUpgradeCount)
    //                    return TransactionResult.Abort();

    //                mutableData.Value = count - needUpgradeCount;
    //                return TransactionResult.Success(mutableData);

    //            }).ContinueWithOnMainThread(txTask =>
    //            {
    //                if (txTask.IsCompleted && !txTask.IsFaulted && !txTask.IsCanceled)
    //                {
    //                    // 성공적으로 감소했으면 다음 레벨 증가
    //                    int nextLevel = currentLevel + 1;
    //                    var nextRef = tankRef.Child(tankName).Child("Levels").Child(nextLevel.ToString());

    //                    nextRef.RunTransaction(mutableData =>
    //                    {
    //                        int nextCount = 0;
    //                        if (mutableData.Value != null)
    //                            int.TryParse(mutableData.Value.ToString(), out nextCount);

    //                        mutableData.Value = nextCount + 1;
    //                        return TransactionResult.Success(mutableData);
    //                    });
    //                }
    //            });
    //        }
    //        UpgradeTank(tankName);
    //    });
    //}
}
