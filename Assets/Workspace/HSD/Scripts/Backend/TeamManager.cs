using Game;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] Color redColor;
    [SerializeField] Color blueColor;
    private int team;

    public void UpdateSlot(Player player, PlayerSlot slot)
    {
        if (player.CustomProperties.TryGetValue("Team", out object value))
        {
            Color teamColor = (Team)value == 0 ? redColor : blueColor;
            slot.UpdateTeam(teamColor);
        }
    }

    public void ChangeTeam()
    {
        int red = 0;
        int blue = 0;

        GetPlayerTeamCount(out red, out blue);

        if (team + 1 >= (int)Team.Length)
            team = 0;
        else
            team++;

        Team tempTeam = (Team)team;

        if (tempTeam == Team.Red)
        {
            if (red >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
            {
                Manager.UI.PopUpUI.Show("레드팀에 반이상의 인원이 참여할 수는 없습니다.");
                return;
            }
        }
        else if (tempTeam == Team.Blue)
        {
            if (blue >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
            {
                Manager.UI.PopUpUI.Show("블루팀에 반이상의 인원이 참여할 수는 없습니다.");
                return;
            }
        }

        PhotonNetwork.LocalPlayer.SetTeam(tempTeam);
    }

    public Team GetRemainingTeam()
    {
        int red = 0;
        int blue = 0;
        Team team = Team.Length;

        GetPlayerTeamCount(out red, out blue);

        if (red >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
            team = Team.Blue;
        else if (blue >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
            team = Team.Red;

        this.team = (int)team;

        return team;
    }

    public Color GetTeamColor(Player player)
    {
        return player.GetTeam() == Team.Red ? redColor : blueColor;
    }

    private void GetPlayerTeamCount(out int red, out int blue)
    {
        red = 0;
        blue = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("TEAM", out object teamObj) && teamObj is Team team)
            {
                if (team == Team.Red) red++;
                else if (team == Team.Blue) blue++;
            }
        }
    }

}
