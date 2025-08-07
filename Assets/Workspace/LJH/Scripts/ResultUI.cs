using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private Button okButton;

    [Header("Î∂?Î™? ?å®?Ñê")]
    [SerializeField] private Transform playersParent;

    [Header("?îÑÎ¶¨Ìåπ")]
    [SerializeField] private GameObject playerSlotPrefab;

    [Header("??? ?Éâ?ÉÅ")]
    [SerializeField] private Color redTeamColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private Color blueTeamColor = new Color(0.3f, 0.3f, 1f);

    [Header("?ó∞Í≤∞Ìï† ?å®?Ñê")]
    [SerializeField] private GameObject lobbyPanel;


    void Start()
    {
        okButton.onClick.AddListener(OnClickOK);
    }
    public void UpdateResult(Team winnerTeam, int mvpActor)
    {
        Debug.Log("ResultActive");
        gameObject.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("∞‘¿” ≥°");
            PhotonNetwork.CurrentRoom.SetGameStart(false);
        }
        // ∞·∞˙ ∆–≥Œ «•Ω√
        winnerText.text = $"Ω¬∏Æ: {(winnerTeam == Team.Red ? "RED ∆¿" : "BLUE ∆¿")}";
        // «√∑π¿ÃæÓ ΩΩ∑‘ √ ±‚»≠
        foreach (Transform child in playersParent)
        {
            Destroy(child.gameObject);
        }
        PhotonNetwork.CurrentRoom.Players.TryGetValue(mvpActor, out Player mvpplayer);
        Debug.Log($"Mvp user == {mvpplayer.NickName}");
        // ?äπÎ¶? ??? ?îå?†à?ù¥?ñ¥ Ï∂îÍ??
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"{player.NickName}, {player.ActorNumber}, {mvpplayer.ActorNumber}");
            if (player.ActorNumber == mvpplayer.ActorNumber)
            {
                Debug.Log($"{player} mvp panel ?Éù?Ñ±");
                AddMVPPlayerSlot(player);
            }
            else if (CustomProperty.GetTeam(player) == winnerTeam)
            {
                AddPlayerSlot(player, true);
            }
            else
            {
                AddPlayerSlot(player, false);
            }
        }
        resultPanel.SetActive(true);


    }
    public void AddPlayerSlot(Player player, bool isWinner)
    {
        // ?îÑÎ¶¨Ìåπ ?Éù?Ñ±
        GameObject slot = Instantiate(playerSlotPrefab, playersParent);
        // ?å®?Ñê Î∞∞Í≤Ω ?Éâ?ÉÅ
        Image bgImage = slot.GetComponent<Image>();
        if (bgImage != null)
            bgImage.color = (CustomProperty.GetTeam(player) == Team.Red) ? redTeamColor : blueTeamColor;
        // ?îå?†à?ù¥?ñ¥ ?ãâ?Ñ§?ûÑ
        TextMeshProUGUI nameText = slot.transform.Find("NickNameText (TMP)")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
            nameText.text = player.NickName;
        else Debug.LogError("nameText == null");
        // Î≥¥ÏÉÅ ?†ê?àò Í≥ÑÏÇ∞

        if (isWinner)
            Manager.Data.PlayerData.RaiseWinCount();
        else
            Manager.Data.PlayerData.RaiseLoseCount();
        
        int reward = isWinner ? 100 : 50;

        // Î≥¥ÏÉÅ ?à´?ûê Text
        TextMeshProUGUI rewardText = slot.transform.Find("RewardText")?.GetComponent<TextMeshProUGUI>();
        if (rewardText != null)
            rewardText.text = $"+{reward}";
    }

    public void AddMVPPlayerSlot(Player player)
    {
        // ?îÑÎ¶¨Ìåπ ?Éù?Ñ±
        GameObject slot = Instantiate(playerSlotPrefab, playersParent);
        // ?å®?Ñê Î∞∞Í≤Ω ?Éâ?ÉÅ
        Image bgImage = slot.GetComponent<Image>();
        if (bgImage != null)
            bgImage.color = (CustomProperty.GetTeam(player) == Team.Red) ? redTeamColor : blueTeamColor;
        // ?îå?†à?ù¥?ñ¥ ?ãâ?Ñ§?ûÑ
        TextMeshProUGUI nameText = slot.transform.Find("NickNameText (TMP)")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
            nameText.text = player.NickName;
        // Î≥¥ÏÉÅ ?†ê?àò Í≥ÑÏÇ∞
        int reward = 150;
        // Î≥¥ÏÉÅ ?à´?ûê Text
        TextMeshProUGUI rewardText = slot.transform.Find("RewardText")?.GetComponent<TextMeshProUGUI>();
        if (rewardText != null)
            rewardText.text = $"+{reward}";
        Image mvpImage = slot.transform.Find("MVPImage")?.GetComponent<Image>();
        if (mvpImage != null)
            mvpImage.gameObject.SetActive(true);
    }

    ///// <summary>
    ///// Í≤∞Í≥º UI ?ëú?ãú
    ///// </summary>
    //public void ShowResult(Team winnerTeam, Dictionary<int, int> playerScores)
    //{
    //    gameObject.SetActive(true);
    //    resultPanel.SetActive(true);
    //    winnerText.text = $"?äπÎ¶?: {(winnerTeam == Team.Red ? "RED ???" : "BLUE ???")}";

    //    // Í∏∞Ï°¥ ?ä¨Î°? ?†úÍ±?
    //    foreach (Transform child in playersParent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    // MVP Ï∞æÍ∏∞
    //    int mvpActorNumber = -1;
    //    int highestScore = -1;
    //    foreach (var kv in playerScores)
    //    {
    //        Player p = GetPlayer(kv.Key);
    //        if (p != null && CustomProperty.GetTeam(p) == winnerTeam && kv.Value > highestScore)
    //        {
    //            highestScore = kv.Value;
    //            mvpActorNumber = kv.Key;
    //        }
    //    }

    //    // ???Î≥? Î∂ÑÎ•ò
    //    List<Player> redPlayers = new();
    //    List<Player> bluePlayers = new();
    //    foreach (var player in PhotonNetwork.PlayerList)
    //    {
    //        if (CustomProperty.GetTeam(player) == Team.Red) redPlayers.Add(player);
    //        else bluePlayers.Add(player);
    //    }

    //    int redIndex = 0;
    //    int blueIndex = 0;

    //    // ÏµúÎ?? 8Î™? ?ä¨Î°? ?Éù?Ñ±
    //    for (int i = 0; i < 8; i++)
    //    {
    //        Player playerToAdd = null;

    //        // ????àò(0,2,4..) ?Üí ?†à?ìú???
    //        if (i % 2 == 0 && redIndex < redPlayers.Count)
    //        {
    //            playerToAdd = redPlayers[redIndex++];
    //        }
    //        // ÏßùÏàò(1,3,5..) ?Üí Î∏îÎ£®???
    //        else if (i % 2 == 1 && blueIndex < bluePlayers.Count)
    //        {
    //            playerToAdd = bluePlayers[blueIndex++];
    //        }
    //        // ?ïúÏ™? ????ù¥ Î∂?Ï°±ÌïòÎ©? ?ã§Î•? ????óê?Ñú Ï±ÑÏ??
    //        else if (redIndex < redPlayers.Count)
    //        {
    //            playerToAdd = redPlayers[redIndex++];
    //        }
    //        else if (blueIndex < bluePlayers.Count)
    //        {
    //            playerToAdd = bluePlayers[blueIndex++];
    //        }

    //        if (playerToAdd == null) continue;

    //        int actorNumber = playerToAdd.ActorNumber;
    //        int score = playerScores.ContainsKey(actorNumber) ? playerScores[actorNumber] : 0;
    //        Team playerTeam = CustomProperty.GetTeam(playerToAdd);

    //        // ?îÑÎ¶¨Ìåπ ?Éù?Ñ±
    //        GameObject slot = Instantiate(playerSlotPrefab, playersParent);

    //        // ?å®?Ñê Î∞∞Í≤Ω ?Éâ?ÉÅ
    //        Image bgImage = slot.GetComponent<Image>();
    //        if (bgImage != null)
    //            bgImage.color = (playerTeam == Team.Red) ? redTeamColor : blueTeamColor;

    //        // MVP ?ù¥ÎØ∏Ï??
    //        Image mvpImage = slot.transform.Find("MVPImage")?.GetComponent<Image>();
    //        if (mvpImage != null)
    //            mvpImage.gameObject.SetActive(actorNumber == mvpActorNumber);

    //        // ?îå?†à?ù¥?ñ¥ ?ãâ?Ñ§?ûÑ
    //        TextMeshProUGUI nameText = slot.transform.Find("PlayerNickname")?.GetComponent<TextMeshProUGUI>();
    //        if (nameText != null)
    //            nameText.text = playerToAdd.NickName;

    //        // Î≥¥ÏÉÅ ?†ê?àò Í≥ÑÏÇ∞
    //        int reward = (playerTeam == winnerTeam) ? 100 : 50;
    //        if (actorNumber == mvpActorNumber) reward = 150;

    //        // Î≥¥ÏÉÅ ?à´?ûê Text
    //        TextMeshProUGUI rewardText = slot.transform.Find("RewardText")?.GetComponent<TextMeshProUGUI>();
    //        if (rewardText != null)
    //            rewardText.text = $"+{reward}";

    //        // Gem ?ù¥ÎØ∏Ï???äî ?îÑÎ¶¨Ìåπ?óê Ï°¥Ïû¨Îß? ?ïòÎ©? ?ûê?èô ?ëú?ãú?ê® (Î≥ÑÎèÑ Î°úÏßÅ ?ïÑ?öî ?óÜ?ùå)
    //    }
    //}

    //private Player GetPlayer(int actorNumber)
    //{
    //    foreach (var p in PhotonNetwork.PlayerList)
    //    {
    //        if (p.ActorNumber == actorNumber) return p;
    //    }
    //    return null;
    //}

    /// <summary>
    /// OK Î≤ÑÌäº ?Å¥Î¶? ?ãú Title ?î¨?úºÎ°? ?ù¥?èô
    /// </summary>
    private void OnClickOK()
    {
        okButton.interactable = false;

        Debug.Log($"?î¨ ?èôÍ∏∞Ìôî ?ó¨Î∂? : {PhotonNetwork.AutomaticallySyncScene}");

        StartCoroutine(GameExitRoutine());
    }

    private IEnumerator GameExitRoutine()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Title");
        op.allowSceneActivation = false;

        Manager.UI.FadeScreen.FadeIn();

        yield return new WaitUntil(() => op.progress >= 0.9f);

        yield return new WaitForSeconds(1f);

        PhotonNetwork.LocalPlayer.SetGamePlay(false);

        op.allowSceneActivation = true;
    }
}
