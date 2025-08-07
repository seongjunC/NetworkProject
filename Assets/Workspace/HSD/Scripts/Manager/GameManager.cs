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

    public void GameStart(string gameSceneName)
    {
        StartCoroutine(GameStartRoutine(gameSceneName));
    }

    private IEnumerator GameStartRoutine(string gameSceneName)
    {       
        Manager.UI.FadeScreen.FadeIn();
        Debug.Log("FadeIn");
        yield return new WaitForSeconds(1);

        PhotonNetwork.AutomaticallySyncScene = true;

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel(gameSceneName);


        yield return new WaitForSeconds(1);

        Manager.Audio.PlayBGMFade("Game", .8f, 1, 1);

        Manager.UI.FadeScreen.FadeOut();
        Debug.Log("FadeOut");
    }
}
