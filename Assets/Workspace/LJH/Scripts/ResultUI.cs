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

        Time.timeScale = 0;

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

        if (PhotonNetwork.LocalPlayer.GetTeam() == winnerTeam)
            Manager.Data.PlayerData.RaiseWinCount();
        else
            Manager.Data.PlayerData.RaiseLoseCount();

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

    /// <summary>
    /// OK ��ư Ŭ�� �� Title ������ �̵�
    /// </summary>
    private void OnClickOK()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        StartCoroutine(GameExitRoutine());
    }

    private IEnumerator GameExitRoutine()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync("Title");
        op.allowSceneActivation = false;

        Manager.UI.FadeScreen.FadeIn();

        yield return new WaitUntil(() => op.progress >= 0.9f);

        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1;

        PhotonNetwork.LocalPlayer.SetGamePlay(false);

        op.allowSceneActivation = true;
    }
}
