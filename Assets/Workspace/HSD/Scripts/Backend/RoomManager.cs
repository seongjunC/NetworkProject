using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject playerSlotPrefab;
    [SerializeField] Transform redPlayerContent;
    [SerializeField] Transform bluePlayerContent;
    private Dictionary<int, PlayerSlot> playerSlotDic = new Dictionary<int, PlayerSlot>();

    [Header("Map")]
    [SerializeField] GameObject mapPrefab;
    [SerializeField] Transform mapContent;
    [SerializeField] GameObject mapSelectPanel;
    [SerializeField] RawImage mapImage;
    [SerializeField] Button mapChangeButton;
    [SerializeField] Button mapCloseButton;
    private int mapIdx;

    [Header("Game")]
    [SerializeField] Button readyButton;
    [SerializeField] Button startButton;
    [SerializeField] Button turnSwitchButton;
    [SerializeField] Button exitButton;
    [SerializeField] TMP_Text turnType;
    [SerializeField] TMP_Text readyCount;

    [Header("Team")]
    public TeamManager teamManager;
    [SerializeField] Button teamSwitchButton;

    [Header("Panel")]
    [SerializeField] GameObject lobby;
    [SerializeField] GameObject room;

    [Header("Chat")]
    [SerializeField] Chat chat;

    [Header("Room")]
    [SerializeField] TMP_Text roomName;

    private bool isReady;
    private bool isRandom;
    private int currentReadyCount;

    #region LifeCycle
    private void Start()
    {
        CreateMapSlot();
    }
    private void OnEnable()
    {
        Subscribe();
    }
    private void OnDisable()
    {
        UnSubscribe();
    }
    #endregion

    #region EventSubscribe
    private void Subscribe()
    {
        exitButton.onClick.AddListener(LeaveRoom);
        readyButton.onClick.AddListener(Ready);
        startButton.onClick.AddListener(GameStart);
        mapCloseButton.onClick.AddListener(CloseMapPanel);
        mapChangeButton.onClick.AddListener(OpenMapPanel);
        turnSwitchButton.onClick.AddListener(TurnTypeSwitch);
        teamSwitchButton.onClick.AddListener(teamManager.ChangeTeam);
    }

    private void UnSubscribe()
    {
        exitButton.onClick.RemoveListener(LeaveRoom);
        readyButton.onClick.RemoveListener(Ready);
        startButton.onClick.RemoveListener(GameStart);
        mapCloseButton.onClick.RemoveListener(CloseMapPanel);
        mapChangeButton.onClick.RemoveListener(OpenMapPanel);
        turnSwitchButton.onClick.RemoveListener(TurnTypeSwitch);
        teamSwitchButton.onClick.RemoveListener(teamManager.ChangeTeam);
    }
    #endregion

    private void Init()
    {
        isReady = PhotonNetwork.IsMasterClient ? true : false;
        isRandom = true;
    }

    private void LeaveRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Destroy(playerSlotDic[player.ActorNumber].gameObject);
        }

        playerSlotDic.Clear();

        PhotonNetwork.LeaveRoom();
    }

    #region PlayerSlot
    private void CreatePlayerSlot(Player player)
    {
        if (!playerSlotDic.ContainsKey(player.ActorNumber))
        {
            GameObject obj = Instantiate(playerSlotPrefab, GetPlayerTeamContent(player));
            PlayerSlot playerSlot = obj.GetComponent<PlayerSlot>();
            playerSlot.SetUp(player);
            playerSlotDic.Add(player.ActorNumber, playerSlot);
        }
        else
        {
            playerSlotDic[player.ActorNumber].SetUp(player);
        }

        SetButtonInteractable();
    }

    private void SetButtonInteractable()
    {
        mapChangeButton.interactable = PhotonNetwork.IsMasterClient;

        if (PhotonNetwork.IsMasterClient)
        {
            startButton.gameObject.SetActive(true);
            readyButton.gameObject.SetActive(false);
        }
        else
        {
            startButton.gameObject.SetActive(false);
            readyButton.gameObject.SetActive(true);
        }

        turnSwitchButton.interactable = PhotonNetwork.IsMasterClient;
    }

    private void CreatePlayerSlot()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsMasterClient)
        {
            MapChange();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject obj = Instantiate(playerSlotPrefab, GetPlayerTeamContent(player));
            PlayerSlot playerSlot = obj.GetComponent<PlayerSlot>();
            playerSlot.SetUp(player);
            playerSlotDic.Add(player.ActorNumber, playerSlot);
        }
        SetButtonInteractable();
    }

    private void UpdateAllPlayerSlot()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerSlot slot = playerSlotDic[player.ActorNumber];
            slot.SetUp(player);
        }
    }

    private void DestroyPlayerSlot(Player player)
    {
        if (playerSlotDic.TryGetValue(player.ActorNumber, out PlayerSlot panel))
        {
            Destroy(panel.gameObject);
            playerSlotDic.Remove(player.ActorNumber);
        }
        else
        {
            Debug.LogError("슬롯 없음");
        }
    }

    private Transform GetPlayerTeamContent(Player player)
    {
        return player.GetTeam() == Team.Red ? redPlayerContent : bluePlayerContent;
    }
    private void PlayerSlotSetParent(Player player)
    {
        playerSlotDic[player.ActorNumber].gameObject.transform.SetParent(GetPlayerTeamContent(player), false);
    }
    #endregion

    #region Ready
    private void Ready()
    {
        isReady = !isReady;
        ReadyPropertyUpdate();
        UpdateReadyCountText();
    }

    private void AllReadyCheck()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            ReadyCheck(player.Value);
        }
    }

    private void ReadyCheck(Player player)
    {
        if (player.CustomProperties.TryGetValue("Ready", out object value))
        {
            playerSlotDic[player.ActorNumber].SetUp(player);
            UpdateReadyCountText();
        }
    }

    private void ReadyPropertyUpdate()
    {
        PhotonNetwork.LocalPlayer.SetReady(isReady);
    }
    private void UpdateReadyCountText()
    {
        currentReadyCount = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("Ready", out object isReady) && (bool)isReady)
            {
                currentReadyCount++;
            }
        }

        readyCount.text = $"{currentReadyCount} / {PhotonNetwork.CurrentRoom.MaxPlayers}";
    }
    #endregion

    #region Map
    private void MapChange()
    {
        mapIdx = PhotonNetwork.CurrentRoom.GetMap();
        mapImage.texture = Manager.Resources.Load<Texture2D>($"MapIcon/{((MapType)mapIdx).ToString()}");
    }

    private void CreateMapSlot()
    {
        for (int i = 0; i <= (int)MapType.Length - 1; i++)
        {
            GameObject obj = Instantiate(mapPrefab, mapContent);
            MapSlot slot = obj.GetComponent<MapSlot>();
            slot.SetUp((MapType)i);
        }
    }
    private void OpenMapPanel()
    {
        mapSelectPanel.SetActive(true);
    }
    private void CloseMapPanel()
    {
        mapSelectPanel.SetActive(false);
    }
    #endregion

    #region Turn
    private void TurnTypeSwitch()
    {
        isRandom = !isRandom;
        PhotonNetwork.CurrentRoom.SetTurnRandom(isRandom);
    }

    private void UpdateTurnType()
    {
        turnType.text = PhotonNetwork.CurrentRoom.GetTurnRandom() ? "랜덤" : "입장순서";
    }
    #endregion

    private void GameStart()
    {
        if (currentReadyCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log("방에 인원이 부족하거나 모든 플레이어가 레디하지 않았습니다.");
            return;
        }

        //PhotonNetwork.LoadLevel(""); // 씬이동
    }

    #region PhotonCallbacks
    public void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.SetTeam(teamManager.GetRemainingTeam());
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        Init();
        CreatePlayerSlot();
        UpdateAllPlayerSlot();
        UpdateReadyCountText();
        ReadyPropertyUpdate();
        UpdateTurnType();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreatePlayerSlot(newPlayer);
    }
    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        DestroyPlayerSlot(otherPlayer);
        UpdateReadyCountText();
        UpdateAllPlayerSlot();
    }
    public void OnRoomPropertiesUpdate()
    {
        MapChange();
        UpdateTurnType();
    }

    public void OnPlayerPropertiesUpdate(Player target)
    {
        ReadyCheck(target);
        PlayerSlotSetParent(target);
    }

    public void OnMasterClientSwitched(Player newMasterClient)
    {
        CreatePlayerSlot(newMasterClient);
        SetButtonInteractable();
        newMasterClient.SetReady(true);
    }

    public void OnLeftRoom()
    {
        chat.ResetChat();
    }
    #endregion
}
