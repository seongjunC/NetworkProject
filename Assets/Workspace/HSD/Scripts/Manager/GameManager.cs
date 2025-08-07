using Game;
using Photon.Pun;
using System.Collections;
using UnityEngine;

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

    public IEnumerator GameStartRoutine(string gameSceneName)
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        Manager.UI.FadeScreen.FadeIn();

        yield return new WaitForSeconds(1);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(gameSceneName);

        yield return new WaitForSeconds(1);

        Manager.UI.FadeScreen.FadeOut();
    }
}
