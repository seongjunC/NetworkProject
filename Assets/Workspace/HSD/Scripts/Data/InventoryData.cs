using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
public enum TankRank
{
    S, A, B, C
}

// ������ ���� ��ũ ������ �����ϴ� Ŭ���� (������ ���̽� ����� ������)
[Serializable]
public class TankGroupData
{
    public string TankName;
    public TankRank Rank;
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
        Debug.Log("1");
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

        InitData().ContinueWithOnMainThread(task =>
        {
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
        if (!string.IsNullOrEmpty(rankStr) && Enum.TryParse(rankStr, out TankRank rank))
        {
            groupData.Rank = rank;
        }

        foreach (var levelSnap in args.Snapshot.Child("Levels").Children) // DB�ȿ� �ִ� ��� ������ ��ȸ
        {
            if (int.TryParse(levelSnap.Key, out int level)) // key�� int�� ��ȯ
            {
                int count = int.TryParse(levelSnap.Child("Count")?.Value?.ToString(), out int cnt) ? cnt : 0; // db�� count�� �Ľ�
                groupData.Levels[level] = count;  // dictionary�� ���� �� �ݿ�

                OnTankCountUpdated?.Invoke(tankName, level, count);
            }
        }
    }
    private void OnTankGroupRemoved(object sender, ChildChangedEventArgs args)
    {
        string tankName = args.Snapshot.Key;
        tankGroups.Remove(tankName);
    }

    // ������ ���̽��� �߰��� �Ǹ� ��Ÿ�� �� Dictionary�� �߰��ϴ� ����
    private void UpdateTankGroup(DataSnapshot snapshot)
    {
        Debug.Log("UpdateTankGroup");
        if (!snapshot.Exists) return;

        string tankName = snapshot.Key; // ��ũ �̸� ������

        var rankStr = snapshot.Child("Rank")?.Value?.ToString();    // ��ũ ������
        var rankParsed = Enum.TryParse(rankStr, out TankRank rank) ? rank : TankRank.C;

        // ������ �����͸� ������� �׷��� �������
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
        Debug.Log("UpdateTankGroup End");
        tankGroups[tankName] = groupData;
    }

    private Task CreateTankGroupData(string tankName, int level)
    {
        // ��ũ�� ���� �����տ��� key������ ������
        TankRank rank = TankRank.C;

        return tankRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            var snapShot = task.Result;

            if (snapShot == null || (string)snapShot.Value != tankName)
            {
                DatabaseReference groupRef = tankRef.Child(tankName);

                TankGroupData groupData = new TankGroupData
                {
                    TankName = tankName,
                    Rank = rank,
                    Levels = new Dictionary<int, int> { {level, 1 } }
                };

                if (task.IsCompleted)
                {
                    Debug.Log("Complated");

                    groupRef.SetRawJsonValueAsync(JsonUtility.ToJson(groupData)).ContinueWithOnMainThread(task =>
                    {
                        if (task.IsCanceled || task.IsFaulted)
                            return;

                        if (task.IsCompleted)
                            Debug.Log("Complated2");
                    });
                }
            }
        });
    }

    public Task AddTank(string tankName, int level, int count, TankRank rank = TankRank.C)
    {
        var countRef = tankRef.Child(tankName).Child("Levels").Child(level.ToString()).Child("Count");

        if (!tankGroups.ContainsKey(tankName))
        {
            return CreateTankGroupData(tankName, level).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log("������ ���� �Ϸ�");
                    countRef.RunTransaction(mutableData =>
                    {
                        int current = 0;
                        if (mutableData.Value != null)
                            int.TryParse(mutableData.Value.ToString(), out current);
                        Debug.Log(mutableData.Value);
                        mutableData.Value = current + count;
                        return TransactionResult.Success(mutableData);
                    });
                }
            });            
        }        

        return countRef.RunTransaction(mutableData =>
        {
            int current = 0;
            if (mutableData.Value != null)
                int.TryParse(mutableData.Value.ToString(), out current);

            mutableData.Value = current + count;
            return TransactionResult.Success(mutableData);
        }).ContinueWithOnMainThread(task =>
        {
            return UpgradeTank(tankName);
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

    public Task UpgradeTank(string tankName)
    {
        return tankRef.Child(tankName).Child("Levels").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsCompleted || task.IsFaulted || task.IsCanceled)
                return;

            var snapshot = task.Result;
            if (!snapshot.Exists) return;

            foreach (var levelNode in snapshot.Children)
            {
                if (!int.TryParse(levelNode.Key, out int level))
                    continue;

                int currentLevel = level; // ������

                var currentRef = tankRef.Child(tankName).Child("Levels").Child(currentLevel.ToString()).Child("Count");

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
                        // ���������� ���������� ���� ���� ����
                        int nextLevel = currentLevel + 1;
                        var nextRef = tankRef.Child(tankName).Child("Levels").Child(nextLevel.ToString()).Child("Count");

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
