using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CustomProperty
{
    private const string map = "Map";
    private const string ready = "Ready";
    private const string uid = "UID";

    private static ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
    
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
}
