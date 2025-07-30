using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public string Name;
    public int Win;
    public int Lose;
    public int Gem { get; private set; }

    public event Action<string> OnNameChanged;
    public event Action<int> OnWinChanged;
    public event Action<int> OnLoseChanged;
    public event Action<int> OnGemChanged;

    public PlayerData()
    {
        Name = default;
        Win = 0;
        Lose = 0;
        Gem = 0;
    }

    public void Init()
    {
        Manager.Database.RegisterUserDataEvent(Database.UserDataType.Name, UpdateName);
        Manager.Database.RegisterUserDataEvent(Database.UserDataType.Win, UpdateWin);
        Manager.Database.RegisterUserDataEvent(Database.UserDataType.Lose, UpdateLose);
        Manager.Database.RegisterUserDataEvent(Database.UserDataType.Gem, UpdateGem);
    }

    private void UpdateName(object sender, ValueChangedEventArgs arg)
    {
        if (arg.Snapshot.Exists && arg.Snapshot.Value != null)
        {
            Name = arg.Snapshot.Value.ToString();
            OnNameChanged?.Invoke(Name);
        }
    }
    private void UpdateWin(object sender, ValueChangedEventArgs arg)
    {
        if (arg.Snapshot.Exists)
        {
            object value = arg.Snapshot.Value;

            if (value is long longValue)
            {
                Win = (int)longValue;
                OnWinChanged?.Invoke(Win);
            }
        }
    }
    private void UpdateLose(object sender, ValueChangedEventArgs arg)
    {
        if (arg.Snapshot.Exists)
        {
            object value = arg.Snapshot.Value;

            if (value is long longValue)
            {
                Lose = (int)longValue;
                OnLoseChanged?.Invoke(Lose);
            }
        }
    }

    private void UpdateGem(object sender, ValueChangedEventArgs arg)
    {
        if (arg.Snapshot.Exists)
        {
            object value = arg.Snapshot.Value;

            if (value is long longValue)
            {
                Lose = (int)longValue;
                OnGemChanged?.Invoke(Gem);
            }
        }
    }

    // 전역적으로 젬 획득 관련 메서드가 필요해보여서 추가했습니다.
    // 추후 논의 후 이전 가능성이 있습니다.
    public void GemGain(int amount)
    {
        Manager.Database.userDataRef.Child("Gem")
        .SetValueAsync(Gem + amount).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Gem += amount;
                Debug.Log($"{amount}만큼의 Gem을 획득하였습니다.\n 현재 Gem의 개수 {Gem}");
            }
            else
            {
                Debug.LogError("저장 실패");
            }
        });
    }

    public bool IsBuy(int amount) => Gem >= amount;    
}
