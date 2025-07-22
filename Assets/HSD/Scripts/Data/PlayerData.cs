using Firebase.Database;
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

    public event Action<string> OnNameChanged;
    public event Action<int> OnWinChanged;
    public event Action<int> OnLoseChanged;

    public PlayerData()
    {
        Name = default;
        Win = 0;
        Lose = 0;

        Init();
    }

    private void Init()
    {
        Manager.Database.RegisterUserDataEvent(Database.UserDataType.name, UpdateName);
        Manager.Database.RegisterUserDataEvent(Database.UserDataType.win, UpdateWin);
        Manager.Database.RegisterUserDataEvent(Database.UserDataType.lose, UpdateLose);
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
        if(arg.Snapshot.Exists)
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
        if(arg.Snapshot.Exists)
        {
            object value = arg.Snapshot.Value;

            if(value is long longValue)
            {
                Lose = (int)longValue;
                OnLoseChanged?.Invoke(Lose);
            }
        }
    }
}
