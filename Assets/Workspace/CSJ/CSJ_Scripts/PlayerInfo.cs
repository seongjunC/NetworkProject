using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class PlayerInfo
{
    public Player player;
    public string NickName => player.NickName;
    public bool isDead;
    public int ActorNumber;

    public PlayerInfo(Player _player)
    {
        player = _player;
        ActorNumber = _player.ActorNumber;
        isDead = false;
    }
}
