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

    private Sprite loading;

    void Start()
    {
        okButton.onClick.AddListener(OnClickOK);
    }
    public void UpdateResult(Team winnerTeam, int mvpActor)
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.CurrentRoom.SetGameStart(false);

        Debug.Log("ResultActive");
        gameObject.SetActive(true);

        Time.timeScale = 0;

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

        if (PhotonNetwork.LocalPlayer.GetTeam() == winnerTeam)
            Manager.Data.PlayerData.RaiseWinCount();
        else
            Manager.Data.PlayerData.RaiseLoseCount();

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

    /// <summary>
    /// OK 버튼 클릭 시 Title 씬으로 이동
    /// </summary>
    private void OnClickOK()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        okButton.interactable = false;
        loading = DataManager.loadingImage;
        Manager.Game.GameExit(loading);
    }
}
