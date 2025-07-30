using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
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
    public Dictionary<int, int> Levels = new();
}

[Serializable]
public class TankInventoryData
{
    public Dictionary<string, TankGroupData> tankGroups = new();
    private DatabaseReference tankRef;

    private const int needUpgradeCount = 3;

    public event Action<string, int, int> OnTankCountUpdated; // (TankName, Level, NewCount)
    public event Action<string, int> OnTankLevelRemoved;

    public TankInventoryData()
    {
        InitTanks();
    }

    private void InitInventory()
    {
        RegisterTankListeners();
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

        Debug.Log("Run");
        Manager.Database.userRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            Debug.Log("Run1");

            var snapShot = task.Result;

            bool exists = false;

            foreach (var child in snapShot.Children)
            {
                if(child.Key == "Tanks")
                {
                    exists = true;
                    break;
                }
            }
            Debug.Log(exists);
            if (!exists)
                tankRef.SetValueAsync("");

            InitData();
        });
    }

    private void InitData()
    {
        tankRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            foreach (var tank in task.Result.Children)
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.Log($"{tank.Value}");
                    return;
                }

                tankRef.Child((string)tank.Value).GetValueAsync().ContinueWithOnMainThread(task =>
                {
                    DataSnapshot snapshot = task.Result;

                    string json = snapshot.GetRawJsonValue();
                    TankGroupData group = JsonUtility.FromJson<TankGroupData>(json);
                    tankGroups.Add(group.TankName, group);
                });
            }
            InitInventory();
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

        foreach (var levelSnap in args.Snapshot.Child("Levels").Children) // DB안에 있는 모든 레벨을 순회
        {
            if (int.TryParse(levelSnap.Key, out int level)) // key를 int로 변환
            {
                int count = int.TryParse(levelSnap.Child("Count")?.Value?.ToString(), out int cnt) ? cnt : 0; // db의 count를 파싱
                groupData.Levels[level] = count;  // dictionary에 실제 값 반영

                OnTankCountUpdated?.Invoke(tankName, level, count);
            }
        }
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

        // 가져온 데이터를 기반으로 그룹을 만들어줌
        var groupData = new TankGroupData
        {
            TankName = tankName,
            Rank = rank,
            Levels = new Dictionary<int, int>()
        };

        var levelsSnapshot = snapshot.Child(tankName).Child("Levels");

        foreach (var levelSnap in levelsSnapshot.Children)
        {
            if (int.TryParse(levelSnap.Key, out int level))
            {
                int count = int.TryParse(levelSnap.Value?.ToString(), out int cnt) ? cnt : 0;
                groupData.Levels[level] = count;

                OnTankCountUpdated?.Invoke(tankName, level, count);
            }
        }
        tankGroups[tankName] = groupData;
    }

    public void AddTankEvent(string tankName, int level, int count, TankRank rank = TankRank.C)
    {
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
                CreateTankGroupData(tankName, level, rank, () =>
                {
                    AddCountAndUpgrade(tankName, level, count);
                });
            }
            else
            {
                AddCountAndUpgrade(tankName, level, count);
            }
        });
    }

    private void CreateTankGroupData(string name, int level, TankRank rank, Action onComplete)
    {
        var data = new TankGroupData 
        {   
            TankName = name,
            Rank = rank,
            Levels = new Dictionary<int, int> { { level, 0 } } 
        };

        tankRef.Child(name).SetRawJsonValueAsync(JsonUtility.ToJson(data)).ContinueWithOnMainThread(_ => onComplete?.Invoke());
    }

    private void AddCountAndUpgrade(string name, int level, int count)
    {
        var countRef = tankRef.Child(name).Child("Levels").Child(level.ToString());

        countRef.RunTransaction(mutableData =>
        {
            int current = mutableData.Value != null ? Convert.ToInt32(mutableData.Value) : 0;
            mutableData.Value = current + count;
            return TransactionResult.Success(mutableData);
        }).ContinueWithOnMainThread(_ =>
        {
            UpgradeTank(name);
        });
    }

    public Task RemoveTank(string tankName, int level, int count)
    {
        var countRef = tankRef.Child(tankName).Child("Levels").Child(level.ToString());

        return countRef.RunTransaction(mutableData =>
        {
            int current = 0;
            if (mutableData.Value != null)
                int.TryParse(mutableData.Value.ToString(), out current);

            int newCount = Mathf.Max(0, current - count);
            mutableData.Value = newCount;

            OnTankLevelRemoved?.Invoke(tankName, newCount);

            return TransactionResult.Success(mutableData);
        });
    }

    public void UpgradeTank(string tankName)
    {
        tankRef.Child(tankName).Child("Levels").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsCompleted || task.IsFaulted || task.IsCanceled)
                return;

            var snapshot = task.Result;
            if (!snapshot.Exists) return;

            foreach (var levelNode in snapshot.Children)
            {
                if (!int.TryParse(levelNode.Key, out int level))
                    continue;

                int currentLevel = level;

                var currentRef = tankRef.Child(tankName).Child("Levels").Child(currentLevel.ToString());

                currentRef.RunTransaction(mutableData =>
                {
                    int count = 0;
                    if (mutableData.Value != null)
                        int.TryParse(mutableData.Value.ToString(), out count);

                    if (count < needUpgradeCount)
                        return TransactionResult.Abort();

                    mutableData.Value = count - needUpgradeCount;
                    return TransactionResult.Success(mutableData);

                }).ContinueWithOnMainThread(txTask =>
                {
                    if (txTask.IsCompleted && !txTask.IsFaulted && !txTask.IsCanceled)
                    {
                        // 성공적으로 감소했으면 다음 레벨 증가
                        int nextLevel = currentLevel + 1;
                        var nextRef = tankRef.Child(tankName).Child("Levels").Child(nextLevel.ToString());

                        nextRef.RunTransaction(mutableData =>
                        {
                            int nextCount = 0;
                            if (mutableData.Value != null)
                                int.TryParse(mutableData.Value.ToString(), out nextCount);

                            mutableData.Value = nextCount + 1;
                            return TransactionResult.Success(mutableData);
                        });
                    }
                });
            }
            UpgradeTank(tankName);
        });
    }

}
