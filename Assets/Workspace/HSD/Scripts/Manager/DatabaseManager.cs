using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using Database;
using UnityEngine;
using System;
using Firebase.Auth;

public class DatabaseManager : Singleton<DatabaseManager>
{   
    public DatabaseReference root {  get; private set; }
    public DatabaseReference userRef { get; private set; }

    #region LifeCycle

    #endregion

    public void Init()
    {
        root    = FirebaseManager.Database.RootReference;
        userRef = root.Child("UserData").Child(FirebaseManager.Auth.CurrentUser.UserId);
    }

    #region EventHandler
    public void RegisterUserDataEvent(UserDataType dataType, EventHandler<ValueChangedEventArgs> eventHandler)
    {
        DatabaseReference dataRef = userRef.Child(dataType.ToString());

        if (dataRef == null) return;

        dataRef.ValueChanged += eventHandler;
    }

    public void UnRegisterUserDataEvent(UserDataType dataType, EventHandler<ValueChangedEventArgs> eventHandler)
    {
        DatabaseReference dataRef = userRef.Child(dataType.ToString());

        if (dataRef == null) return;

        dataRef.ValueChanged -= eventHandler;
    }
    #endregion
}
