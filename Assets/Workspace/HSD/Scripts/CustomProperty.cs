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
    private const string FULL = "Full";
    private const string DAMAGETYPE = "DamageType";
    private const string GAMESTART = "GameStart";
    #endregion

    #region PlayerProperty
    private const string UID = "UID";
    private const string READY = "Ready";
    private const string TEAM = "Team";
    #endregion

    private static ExitGames.Client.Photon.Hashtable playerHash = new ExitGames.Client.Photon.Hashtable();
    private static ExitGames.Client.Photon.Hashtable roomHash = new ExitGames.Client.Photon.Hashtable();

    #region Player
    public static bool GetReady(this Player player)
    {
        if (player.CustomProperties.TryGetValue(READY, out object value))
        {
            return (bool)value;
        }
        return false;
    }

    public static void SetReady(this Player player, bool isReady)
    {
        playerHash[READY] = isReady;
        player.SetCustomProperties(playerHash);
    }

    public static void SetUID(this Player player, string _uid)
    {
        playerHash[UID] = _uid;
        player.SetCustomProperties(playerHash);
    }

    public static string GetUID(this Player player)
    {
        if (player.CustomProperties.TryGetValue(UID, out object value))
        {
            return (string)value;
        }
        return "";
    }

    public static void SetTeam(this Player player, Team team)
    {
        playerHash[TEAM] = team;
        player.SetCustomProperties(playerHash);
    }

    public static Team GetTeam(this Player player)
    {
        if (player.CustomProperties.TryGetValue(TEAM, out object value))
        {
            return (Team)value;
        }
        return Team.Wait;
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
        roomHash[MAP] = (MapType)(mapType);
        room.SetCustomProperties(roomHash);
    }


    public static void SetPassword(this Room room, string value)
    {
        roomHash[PASSWORD] = value;
        room.SetCustomProperties(roomHash);
    }
    public static string GetPassword(this Room room)
    {
        if (room.CustomProperties.TryGetValue(PASSWORD, out object value))
        {
            return (string)value;
        }
        return "";
    }

    public static void SetTurnRandom(this Room room, bool isRandom)
    {
        roomHash[TURNRANDOM] = isRandom;
        room.SetCustomProperties(roomHash);
    }

    public static bool GetTurnRandom(this Room room)
    {
        if (room.CustomProperties.TryGetValue(TURNRANDOM, out object value))
        {
            return (bool)value;
        }
        return false;
    }

    public static void SetFull(this Room room, bool isFull)
    {
        roomHash[FULL] = isFull;
        room.SetCustomProperties(roomHash);
    }

    public static bool GetFull(this Room room)
    {
        if (room.CustomProperties.TryGetValue(FULL, out object value))
        {
            return (bool)value;
        }
        return false;
    }

    public static void SetDamageType(this Room room, bool isTeamDamageApply)
    {
        roomHash[DAMAGETYPE] = isTeamDamageApply;
        room.SetCustomProperties(roomHash);
    }

    public static bool GetDamageType(this Room room)
    {
        if(room.CustomProperties.TryGetValue(DAMAGETYPE, out object value))
        {
            return (bool)value;
        }
        return false;
    }

    public static void SetGameStart(this Room room, bool isGameStart)
    {
        roomHash[GAMESTART] = isGameStart;
        room.SetCustomProperties(roomHash);
    }
    public static void SetGameStart(this RoomInfo room, bool isGameStart)
    {        
        room.CustomProperties[GAMESTART] = isGameStart;
    }

    public static bool GetGameStart(this Room room)
    {
        if(room.CustomProperties.TryGetValue(GAMESTART, out object value))
        {
            Debug.Log(value);
            return (bool)value;
        }
        return false;
    }

    public static bool GetGameStart(this RoomInfo room)
    {
        if (room.CustomProperties.TryGetValue(GAMESTART, out object value))
        {            
            Debug.Log((bool)value);
            return (bool)value;
        }
        return false;
    }
    #endregion
}
