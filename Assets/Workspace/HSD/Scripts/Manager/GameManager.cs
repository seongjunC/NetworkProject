using Game;
using Photon.Pun;

public class GameManager : Singleton<GameManager>
{
    public State State;

    public void GameStart()
    {
        PhotonNetwork.AutomaticallySyncScene = false;

        if (!PhotonNetwork.IsMasterClient)
            PhotonNetwork.LocalPlayer.SetReady(false);

        PhotonNetwork.LocalPlayer.SetGamePlay(true);
    }
}
