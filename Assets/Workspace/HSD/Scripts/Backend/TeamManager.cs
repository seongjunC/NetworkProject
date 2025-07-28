using Game;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    private int team;

    public void ChangeTeam(Team _team)
    {
        team = (int)PhotonNetwork.LocalPlayer.GetTeam();

        int red = 0;
        int blue = 0;

        GetPlayerTeamCount(out red, out blue);

        Team tempTeam = _team;

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

        Team team = Team.Red;

        GetPlayerTeamCount(out red, out blue);

        if (red >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
            team = Team.Blue;
        else if (blue >= PhotonNetwork.CurrentRoom.MaxPlayers / 2)
            team = Team.Red;        

        this.team = (int)team;

        return team;
    }

    private void GetPlayerTeamCount(out int red, out int blue)
    {
        red = 0;
        blue = 0;

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("Team", out object value))
            {
                if ((Team)value == Team.Red) red++;
                else if ((Team)value == Team.Blue) blue++;
            }
        }
    }

    public int GetWaitPlayerCount()
    {
        int count = 0;
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("Team", out object value))
            {
                if ((Team)((int)value) == Team.Wait) count++;
            }
        }
        return count;
    }
}
