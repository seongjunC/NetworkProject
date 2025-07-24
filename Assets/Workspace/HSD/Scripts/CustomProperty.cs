using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public static class CustomProperty
{
    #region RoomProperty
    private const string map = "Map";
    private const string password = "Password";
    #endregion

    #region PlayerProperty
    private const string uid = "UID";
    private const string ready = "Ready";
    #endregion

    private static ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();

    #region Player
    public static bool GetReady(this Player player)
    {
        if(player.CustomProperties.TryGetValue(ready, out object value))
        {
            return (bool)value;
        }
        return false;
    }

    public static void SetReady(this Player player, bool isReady)
    {;
        hash[ready] = isReady;
        player.SetCustomProperties(hash);
    }

    public static void SetUID(this Player player, string _uid)
    {        
        hash[uid] = _uid;
        player.SetCustomProperties(hash);
    }

    public static string GetUID(this Player player)
    {
        if (player.CustomProperties.TryGetValue(uid, out object value))
        {
            return (string)value;
        }
        return "";
    }
    #endregion

    #region Room
    public static int GetMap(this Room room)
    {
        if (room.CustomProperties.TryGetValue(map, out object value))
        {
            return (int)value;
        }
        return -1;
    }

    public static void SetMap(this Room room, int mapType)
    {        
        hash[map] = (MapType)(mapType);
        room.SetCustomProperties(hash);
    }


    public static void SetPassword(this Room room, long value)
    {
        hash[password] = value;
        room.SetCustomProperties(hash);
    }
    public static long GetPassword(this Room room)
    {
        if(room.CustomProperties.TryGetValue(password, out object value))
        {
            return (long)value;
        }
        return -1;
    }
    #endregion
}
