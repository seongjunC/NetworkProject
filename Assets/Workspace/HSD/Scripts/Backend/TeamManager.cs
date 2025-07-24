using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] Color redColor;
    [SerializeField] Color blueColor;
    private int team;

    public void Init()
    {
        team = 0;
    }

    public void UpdateSlot(Player player, PlayerSlot slot)
    {
        if(player.CustomProperties.TryGetValue("TEAM", out object value))
        {
            Color teamColor = team == 0 ? redColor : blueColor;
            slot.UpdateTeam(teamColor);
        }
    }

    public void ChangeTeam()
    {
        if (team + 1 >= (int)Team.Length)
            team = 0;

        team++;

        PhotonNetwork.LocalPlayer.SetTeam((Team)team);
    }

    private int GetPlayerBlueCount()
    {

    }

    private int GetPlayerRedCount()
    {

    }
}
