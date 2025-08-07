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

    [Header("�θ� �г�")]
    [SerializeField] private Transform playersParent;

    [Header("������")]
    [SerializeField] private GameObject playerSlotPrefab;

    [Header("�� ����")]
    [SerializeField] private Color redTeamColor = new Color(1f, 0.3f, 0.3f);
    [SerializeField] private Color blueTeamColor = new Color(0.3f, 0.3f, 1f);

    [Header("������ �г�")]
    [SerializeField] private GameObject lobbyPanel;


    void Start()
    {
        okButton.onClick.AddListener(OnClickOK);
    }
    public void UpdateResult(Team winnerTeam, int mvpActor)
    {
        Debug.Log("ResultActive");
        gameObject.SetActive(true);

        // ��� �г� ǥ��
        winnerText.text = $"�¸�: {(winnerTeam == Team.Red ? "RED ��" : "BLUE ��")}";
        // �÷��̾� ���� �ʱ�ȭ
        foreach (Transform child in playersParent)
        {
            Destroy(child.gameObject);
        }
        PhotonNetwork.CurrentRoom.Players.TryGetValue(mvpActor, out Player mvpplayer);
        Debug.Log($"Mvp user == {mvpplayer.NickName}");
        // �¸� �� �÷��̾� �߰�
        foreach (var player in PhotonNetwork.PlayerList)
        {
            Debug.Log($"{player.NickName}, {player.ActorNumber}, {mvpplayer.ActorNumber}");
            if (player.ActorNumber == mvpplayer.ActorNumber)
            {
                Debug.Log($"{player} mvp panel ����");
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
        // ������ ����
        GameObject slot = Instantiate(playerSlotPrefab, playersParent);
        // �г� ��� ����
        Image bgImage = slot.GetComponent<Image>();
        if (bgImage != null)
            bgImage.color = (CustomProperty.GetTeam(player) == Team.Red) ? redTeamColor : blueTeamColor;
        // �÷��̾� �г���
        TextMeshProUGUI nameText = slot.transform.Find("NickNameText (TMP)")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
            nameText.text = player.NickName;
        else Debug.LogError("nameText == null");
        // ���� ���� ���
        int reward = isWinner ? 100 : 50;
        // ���� ���� Text
        TextMeshProUGUI rewardText = slot.transform.Find("RewardText")?.GetComponent<TextMeshProUGUI>();
        if (rewardText != null)
            rewardText.text = $"+{reward}";
    }

    public void AddMVPPlayerSlot(Player player)
    {
        // ������ ����
        GameObject slot = Instantiate(playerSlotPrefab, playersParent);
        // �г� ��� ����
        Image bgImage = slot.GetComponent<Image>();
        if (bgImage != null)
            bgImage.color = (CustomProperty.GetTeam(player) == Team.Red) ? redTeamColor : blueTeamColor;
        // �÷��̾� �г���
        TextMeshProUGUI nameText = slot.transform.Find("NickNameText (TMP)")?.GetComponent<TextMeshProUGUI>();
        if (nameText != null)
            nameText.text = player.NickName;
        // ���� ���� ���
        int reward = 150;
        // ���� ���� Text
        TextMeshProUGUI rewardText = slot.transform.Find("RewardText")?.GetComponent<TextMeshProUGUI>();
        if (rewardText != null)
            rewardText.text = $"+{reward}";
        Image mvpImage = slot.transform.Find("MVPImage")?.GetComponent<Image>();
        if (mvpImage != null)
            mvpImage.gameObject.SetActive(true);
    }

    ///// <summary>
    ///// ��� UI ǥ��
    ///// </summary>
    //public void ShowResult(Team winnerTeam, Dictionary<int, int> playerScores)
    //{
    //    gameObject.SetActive(true);
    //    resultPanel.SetActive(true);
    //    winnerText.text = $"�¸�: {(winnerTeam == Team.Red ? "RED ��" : "BLUE ��")}";

    //    // ���� ���� ����
    //    foreach (Transform child in playersParent)
    //    {
    //        Destroy(child.gameObject);
    //    }

    //    // MVP ã��
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

    //    // ���� �з�
    //    List<Player> redPlayers = new();
    //    List<Player> bluePlayers = new();
    //    foreach (var player in PhotonNetwork.PlayerList)
    //    {
    //        if (CustomProperty.GetTeam(player) == Team.Red) redPlayers.Add(player);
    //        else bluePlayers.Add(player);
    //    }

    //    int redIndex = 0;
    //    int blueIndex = 0;

    //    // �ִ� 8�� ���� ����
    //    for (int i = 0; i < 8; i++)
    //    {
    //        Player playerToAdd = null;

    //        // Ȧ��(0,2,4..) �� ������
    //        if (i % 2 == 0 && redIndex < redPlayers.Count)
    //        {
    //            playerToAdd = redPlayers[redIndex++];
    //        }
    //        // ¦��(1,3,5..) �� �����
    //        else if (i % 2 == 1 && blueIndex < bluePlayers.Count)
    //        {
    //            playerToAdd = bluePlayers[blueIndex++];
    //        }
    //        // ���� ���� �����ϸ� �ٸ� ������ ä��
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

    //        // ������ ����
    //        GameObject slot = Instantiate(playerSlotPrefab, playersParent);

    //        // �г� ��� ����
    //        Image bgImage = slot.GetComponent<Image>();
    //        if (bgImage != null)
    //            bgImage.color = (playerTeam == Team.Red) ? redTeamColor : blueTeamColor;

    //        // MVP �̹���
    //        Image mvpImage = slot.transform.Find("MVPImage")?.GetComponent<Image>();
    //        if (mvpImage != null)
    //            mvpImage.gameObject.SetActive(actorNumber == mvpActorNumber);

    //        // �÷��̾� �г���
    //        TextMeshProUGUI nameText = slot.transform.Find("PlayerNickname")?.GetComponent<TextMeshProUGUI>();
    //        if (nameText != null)
    //            nameText.text = playerToAdd.NickName;

    //        // ���� ���� ���
    //        int reward = (playerTeam == winnerTeam) ? 100 : 50;
    //        if (actorNumber == mvpActorNumber) reward = 150;

    //        // ���� ���� Text
    //        TextMeshProUGUI rewardText = slot.transform.Find("RewardText")?.GetComponent<TextMeshProUGUI>();
    //        if (rewardText != null)
    //            rewardText.text = $"+{reward}";

    //        // Gem �̹����� �����տ� ���縸 �ϸ� �ڵ� ǥ�õ� (���� ���� �ʿ� ����)
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
    /// OK ��ư Ŭ�� �� Title ������ �̵�
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
