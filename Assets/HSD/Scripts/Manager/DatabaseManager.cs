using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using Database;
using UnityEngine;
using System;

public class DatabaseManager : Singleton<DatabaseManager>
{   
    private DatabaseReference root;
    private DatabaseReference userRef;

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
