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

    [Header("부모 패널")]
    [SerializeField] private Transform playersParent;

    [Header("프리팹")]
    [SerializeField] private GameObject playerSlotPrefab;

    [Header("팀 색상")]
    [SerializeField] private Color redTeamColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private Color blueTeamColor = new Color(0.3f, 0.3f, 1f);

    [Header("연결할 패널")]
    [SerializeField] private GameObject lobbyPanel;


    void Start()
    {
        okButton.onClick.AddListener(OnClickOK);
    }
    public void UpdateResult(Team winnerTeam, int mvpActor)
    {
        Debug.Log("ResultActive");
        gameObject.SetActive(true);

        // 결과 패널 표시
        winnerText.text = $"승리: {(winnerTeam == Team.Red ? "RED 팀" : "BLUE 팀")}";
        // 플레이어 슬롯 초기화
        foreach (Transform child in playersParent)
        {
            Destroy(child.gameObject);
        }
        PhotonNetwork.CurrentRoom.Players.TryGetValue(mvpActor, out Player mvpplayer);
        Debug.Log($"Mvp user == {mvpplayer.NickName}");
        // 승리 팀 플레이어 추가
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"{player.NickName}, {player.ActorNumber}, {mvpplayer.ActorNumber}");
            if (player.ActorNumber == mvpplayer.ActorNumber)
            {
                Debug.Log($"{player} mvp panel 생성");
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
        // 프리팹 생성
        GameObject slot = Instantiate(playerSlotPrefab, playersParent);
        // 패널 배경 색상
        Image bgImage = slot.GetComponent<Image>();
        if (bgImage != null)
            bgImage.color = (CustomProperty.GetTeam(player) == Team.Red) ? redTeamColor : blueTeamColor;
        // 플레이어 닉네임
        TextMeshProUGUI nameText = slot.transform.Find("NickNameText (TMP)")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
            nameText.text = player.NickName;
        else Debug.LogError("nameText == null");
        // 보상 점수 계산
        int reward = isWinner ? 100 : 50;
        // 보상 숫자 Text
        TextMeshProUGUI rewardText = slot.transform.Find("RewardText")?.GetComponent<TextMeshProUGUI>();
        if (rewardText != null)
            rewardText.text = $"+{reward}";
    }

    public void AddMVPPlayerSlot(Player player)
    {
        // 프리팹 생성
        GameObject slot = Instantiate(playerSlotPrefab, playersParent);
        // 패널 배경 색상
        Image bgImage = slot.GetComponent<Image>();
        if (bgImage != null)
            bgImage.color = (CustomProperty.GetTeam(player) == Team.Red) ? redTeamColor : blueTeamColor;
        // 플레이어 닉네임
        TextMeshProUGUI nameText = slot.transform.Find("NickNameText (TMP)")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
            nameText.text = player.NickName;
        // 보상 점수 계산
        int reward = 150;
        // 보상 숫자 Text
        TextMeshProUGUI rewardText = slot.transform.Find("RewardText")?.GetComponent<TextMeshProUGUI>();
        if (rewardText != null)
            rewardText.text = $"+{reward}";
        Image mvpImage = slot.transform.Find("MVPImage")?.GetComponent<Image>();
        if (mvpImage != null)
            mvpImage.gameObject.SetActive(true);
    }

    ///// <summary>
    ///// 결과 UI 표시
    ///// </summary>
    //public void ShowResult(Team winnerTeam, Dictionary<int, int> playerScores)
    //{
    //    gameObject.SetActive(true);
    //    resultPanel.SetActive(true);
    //    winnerText.text = $"승리: {(winnerTeam == Team.Red ? "RED 팀" : "BLUE 팀")}";

    //    // 기존 슬롯 제거
    //    foreach (Transform child in playersParent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    // MVP 찾기
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

    //    // 팀별 분류
    //    List<Player> redPlayers = new();
    //    List<Player> bluePlayers = new();
    //    foreach (var player in PhotonNetwork.PlayerList)
    //    {
    //        if (CustomProperty.GetTeam(player) == Team.Red) redPlayers.Add(player);
    //        else bluePlayers.Add(player);
    //    }

    //    int redIndex = 0;
    //    int blueIndex = 0;

    //    // 최대 8명 슬롯 생성
    //    for (int i = 0; i < 8; i++)
    //    {
    //        Player playerToAdd = null;

    //        // 홀수(0,2,4..) → 레드팀
    //        if (i % 2 == 0 && redIndex < redPlayers.Count)
    //        {
    //            playerToAdd = redPlayers[redIndex++];
    //        }
    //        // 짝수(1,3,5..) → 블루팀
    //        else if (i % 2 == 1 && blueIndex < bluePlayers.Count)
    //        {
    //            playerToAdd = bluePlayers[blueIndex++];
    //        }
    //        // 한쪽 팀이 부족하면 다른 팀에서 채움
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

    //        // 프리팹 생성
    //        GameObject slot = Instantiate(playerSlotPrefab, playersParent);

    //        // 패널 배경 색상
    //        Image bgImage = slot.GetComponent<Image>();
    //        if (bgImage != null)
    //            bgImage.color = (playerTeam == Team.Red) ? redTeamColor : blueTeamColor;

    //        // MVP 이미지
    //        Image mvpImage = slot.transform.Find("MVPImage")?.GetComponent<Image>();
    //        if (mvpImage != null)
    //            mvpImage.gameObject.SetActive(actorNumber == mvpActorNumber);

    //        // 플레이어 닉네임
    //        TextMeshProUGUI nameText = slot.transform.Find("PlayerNickname")?.GetComponent<TextMeshProUGUI>();
    //        if (nameText != null)
    //            nameText.text = playerToAdd.NickName;

    //        // 보상 점수 계산
    //        int reward = (playerTeam == winnerTeam) ? 100 : 50;
    //        if (actorNumber == mvpActorNumber) reward = 150;

    //        // 보상 숫자 Text
    //        TextMeshProUGUI rewardText = slot.transform.Find("RewardText")?.GetComponent<TextMeshProUGUI>();
    //        if (rewardText != null)
    //            rewardText.text = $"+{reward}";

    //        // Gem 이미지는 프리팹에 존재만 하면 자동 표시됨 (별도 로직 필요 없음)
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
    /// OK 버튼 클릭 시 Title 씬으로 이동
    /// </summary>
    private void OnClickOK()
    {
        okButton.interactable = false;
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
