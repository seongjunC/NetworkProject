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
    private const string MAP = "Map";
    private const string PASSWORD = "Password";
    private const string TURNRANDOM = "TurnRandom";
    #endregion

    #region PlayerProperty
    private const string UID = "UID";
    private const string READY = "Ready";
    #endregion

    private static ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();

    #region Player
    public static bool GetReady(this Player player)
    {
        if(player.CustomProperties.TryGetValue(READY, out object value))
        {
            return (bool)value;
        }
        return false;
    }

    public static void SetReady(this Player player, bool isReady)
    {;
        hash[READY] = isReady;
        player.SetCustomProperties(hash);
    }

    public static void SetUID(this Player player, string _uid)
    {        
        hash[UID] = _uid;
        player.SetCustomProperties(hash);
    }

    public static string GetUID(this Player player)
    {
        if (player.CustomProperties.TryGetValue(UID, out object value))
        {
            return (string)value;
        }
        return "";
    }
    #endregion

    #region Room
    public static int GetMap(this Room room)
    {
        if (room.CustomProperties.TryGetValue(MAP, out object value))
        {
            return (int)value;
        }
        return -1;
    }

    public static void SetMap(this Room room, int mapType)
    {        
        hash[MAP] = (MapType)(mapType);
        room.SetCustomProperties(hash);
    }


    public static void SetPassword(this Room room, string value)
    {
        hash[PASSWORD] = value;
        room.SetCustomProperties(hash);
    }
    public static string GetPassword(this Room room)
    {
        if(room.CustomProperties.TryGetValue(PASSWORD, out object value))
        {
            return (string)value;
        }
        return "";
    }

    public static void SetTurnRandom(this Room room, bool isRandom)
    {
        hash[TURNRANDOM] = isRandom;
        room.SetCustomProperties(hash);
    }

    public static bool GetTurnRandom(this Room room)
    {
        if(room.CustomProperties.TryGetValue(TURNRANDOM, out object value))
        {
            return (bool)value;
        }
        return false;
    }
    #endregion
}
