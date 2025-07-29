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

[Serializable]
public class TankLevelData
{
    public int Count;
}

[Serializable]
public class TankGroupData
{
    public string TankName;
    public TestTankRank Rank;
    public Dictionary<int, TankLevelData> Levels = new();
}

[Serializable]
public class InventoryData
{
    public Dictionary<string, TankGroupData> tankGroups = new();
    private DatabaseReference tankRef => Manager.Database.userRef.Child("Tanks");

    private const int needUpgradeCount = 3;

    public event Action<string, int, int> OnTankCountUpdated; // (TankName, Level, NewCount)
    public event Action<string, int> OnTankLevelRemoved;

    public InventoryData()
    {
        InitInventory();
    }

    private void InitInventory()
    {
        tankRef.GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && task.Result.Exists)
            {
                RegisterTankListeners();
            }
            else
            {
                CreateDefaultTanks().ContinueWithOnMainThread(_ => RegisterTankListeners());
            }
        });
    }

    private async Task CreateDefaultTanks()
    {
        var defaultTanks = new List<TankGroupData>
        {
            new TankGroupData
            {
                TankName = "HeavyTank",
                Rank = TestTankRank.A,
                Levels = new Dictionary<int, TankLevelData> { { 1, new TankLevelData { Count = 1 } } }
            },
            new TankGroupData
            {
                TankName = "SniperTank",
                Rank = TestTankRank.B,
                Levels = new Dictionary<int, TankLevelData> { { 1, new TankLevelData { Count = 1 } } }
            }
        };

        foreach (var tank in defaultTanks)
        {
            var groupRef = tankRef.Child(tank.TankName);
            await groupRef.Child("Rank").SetValueAsync(tank.Rank.ToString());

            foreach (var level in tank.Levels)
            {
                await groupRef.Child("Levels").Child(level.Key.ToString())
                    .Child("Count").SetValueAsync(level.Value.Count);
            }
        }
    }

    private void RegisterTankListeners()
    {
        tankRef.ChildAdded += OnTankGroupAdded;
        tankRef.ChildChanged += OnTankGroupChanged;
        tankRef.ChildRemoved += OnTankGroupRemoved;
    }

    private void OnTankGroupAdded(object sender, ChildChangedEventArgs args) => UpdateTankGroup(args.Snapshot);
    private void OnTankGroupChanged(object sender, ChildChangedEventArgs args) => UpdateTankGroup(args.Snapshot);
    private void OnTankGroupRemoved(object sender, ChildChangedEventArgs args)
    {
        string tankName = args.Snapshot.Key;
        tankGroups.Remove(tankName);
    }

    private void UpdateTankGroup(DataSnapshot snapshot)
    {
        string tankName = snapshot.Key;
        if (!snapshot.Exists) return;

        var rankStr = snapshot.Child("Rank")?.Value?.ToString();
        var rankParsed = Enum.TryParse(rankStr, out TestTankRank rank) ? rank : TestTankRank.C;

        var groupData = new TankGroupData
        {
            TankName = tankName,
            Rank = rank,
            Levels = new Dictionary<int, TankLevelData>()
        };

        var levelsSnapshot = snapshot.Child("Levels");
        foreach (var levelSnap in levelsSnapshot.Children)
        {
            if (int.TryParse(levelSnap.Key, out int level))
            {
                int count = int.TryParse(levelSnap.Child("Count")?.Value?.ToString(), out int cnt) ? cnt : 0;
                groupData.Levels[level] = new TankLevelData { Count = count };

                OnTankCountUpdated?.Invoke(tankName, level, count);
            }
        }

        tankGroups[tankName] = groupData;
    }

    public Task AddTank(string tankName, int level, int count)
    {
        int current = 0;
        if (tankGroups.TryGetValue(tankName, out var group) &&
            group.Levels.TryGetValue(level, out var levelData))
        {
            current = levelData.Count;
        }

        int newCount = current + count;
        return tankRef.Child(tankName).Child("Levels").Child(level.ToString())
            .Child("Count").SetValueAsync(newCount);
    }

    public Task RemoveTank(string tankName, int level, int count)
    {
        int current = 0;
        if (tankGroups.TryGetValue(tankName, out var group) &&
            group.Levels.TryGetValue(level, out var levelData))
        {
            current = levelData.Count;
        }

        int newCount = Mathf.Max(0, current - count);
        return tankRef.Child(tankName).Child("Levels").Child(level.ToString())
            .Child("Count").SetValueAsync(newCount);
    }

    public Task UpgradeTank(string tankName, int currentLevel)
    {
        return RemoveTank(tankName, currentLevel, needUpgradeCount).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                int nextLevel = currentLevel + 1;
                AddTank(tankName, nextLevel, 1);
            }
        });
    }
}
