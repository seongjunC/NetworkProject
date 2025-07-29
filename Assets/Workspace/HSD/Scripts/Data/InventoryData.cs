using Firebase.Database;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Firebase.Extensions;
public enum TestTankRank
{
    S, A, B, C
}


// 레벨에 따른 탱크 갯수를 관리하는 클래스
[Serializable]
public class TankGroupData
{
    public string TankName;
    public TestTankRank Rank;
    public Dictionary<int, int> Levels = new();
}

[Serializable]
public class InventoryData
{
    public Dictionary<string, TankGroupData> tankGroups = new();
    private DatabaseReference tankRef;

    private const int needUpgradeCount = 3;

    public event Action<string, int, int> OnTankCountUpdated; // (TankName, Level, NewCount)
    public event Action<string, int> OnTankLevelRemoved;

    public InventoryData()
    {
        InitTanks();        
    }

    private void InitInventory()
    {
        tankRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                RegisterTankListeners();
            }
        });
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

        InitData().ContinueWithOnMainThread(task =>
        {
            Manager.Database.userRef.Child("Tanks").GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    tankRef = Manager.Database.userRef.Child("Tanks");
                    InitInventory();
                }
            });
            InitInventory();
        });      
    }

    private Task InitData()
    {
        return tankRef.GetValueAsync().ContinueWithOnMainThread(task =>
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
        if (!string.IsNullOrEmpty(rankStr) && Enum.TryParse(rankStr, out TestTankRank rank))
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
        var rankParsed = Enum.TryParse(rankStr, out TestTankRank rank) ? rank : TestTankRank.C; 

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
                int count = int.TryParse(levelSnap.Child("Count")?.Value?.ToString(), out int cnt) ? cnt : 0;
                groupData.Levels[level] = count;

                OnTankCountUpdated?.Invoke(tankName, level, count);
            }
        }

        tankGroups[tankName] = groupData;
    }

    private Task CreateTankGroupData(string tankName, int level)
    {
        // 랭크는 추후 프리팹에서 key값으로 가져옴
        TestTankRank rank = TestTankRank.C;

        tankRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            var snapShot = task.Result;

            if(snapShot == null || (string)snapShot.Value != tankName)
            {
                tankRef.SetValueAsync(tankName).ContinueWithOnMainThread(task =>
                {

                });
            }
        });

        DatabaseReference groupRef = tankRef.Child(tankName);

        TankGroupData groupData = new TankGroupData
        {
            TankName = tankName,
            Rank = rank,
            Levels = new Dictionary<int, int>()
        };

        return groupRef.SetRawJsonValueAsync(JsonUtility.ToJson(groupData)).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
                return;
        });           
    }

    public Task AddTank(string tankName, int level, int count, TestTankRank rank = TestTankRank.C)
    {
        if(!tankGroups.ContainsKey(tankName))
        {
            CreateTankGroupData(tankName, level).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("데이터 저장 완료");
            });
        }

        var countRef = tankRef.Child(tankName).Child("Levels").Child(level.ToString()).Child("Count");

        return countRef.RunTransaction(mutableData =>
        {
            int current = 0;

            if (mutableData.Value != null)
                int.TryParse(mutableData.Value.ToString(), out current);

            mutableData.Value = current + count;

            return TransactionResult.Success(mutableData);
        });
    }

    public Task RemoveTank(string tankName, int level, int count)
    {
        var countRef = tankRef.Child(tankName).Child("Levels").Child(level.ToString()).Child("Count");

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

    public Task UpgradeTank(string tankName, int currentLevel)
    {
        var currentLevelRef = tankRef.Child(tankName).Child("Levels").Child(currentLevel.ToString()).Child("Count");

        return currentLevelRef.RunTransaction(mutableData =>
        {
            int current = 0;
            if (mutableData.Value != null)
                int.TryParse(mutableData.Value.ToString(), out current);

            if (current < needUpgradeCount)
                return TransactionResult.Abort();

            mutableData.Value = current - needUpgradeCount;
            return TransactionResult.Success(mutableData);

        }).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                var nextLevelRef = tankRef.Child(tankName).Child("Levels").Child((currentLevel + 1).ToString()).Child("Count");
                nextLevelRef.RunTransaction(mutableData =>
                {
                    int nextLevelCount = 0;
                    if (mutableData.Value != null)
                        int.TryParse(mutableData.Value.ToString(), out nextLevelCount);

                    mutableData.Value = nextLevelCount + 1;
                    return TransactionResult.Success(mutableData);
                });
            }
        });
    }
}
