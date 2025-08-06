using Firebase.Auth;
using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(PhotonView))]
public class RoomManager : MonoBehaviourPun
{
    [SerializeField] string gameSceneName;

    [Header("Player")]
    [SerializeField] GameObject playerSlotPrefab;
    [SerializeField] Transform redPlayerContent;
    [SerializeField] Transform bluePlayerContent;
    [SerializeField] Transform waitPlayerContent;
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
    [SerializeField] Button damageTypeButton;
    [SerializeField] Button gameSettingButton;
    [SerializeField] Button gameSettingCloseButton;
    [SerializeField] TMP_Text turnType;
    [SerializeField] TMP_Text damageType;
    [SerializeField] TMP_Text readyCount;

    [Header("Team")]
    public TeamManager teamManager;
    [SerializeField] Button blueTeamChangeButton;
    [SerializeField] Button redTeamChangeButton;
    [SerializeField] Button waitTeamChangeButton;

    [Header("Panel")]
    [SerializeField] GameObject lobby;
    [SerializeField] GameObject room;
    [SerializeField] GameObject gameSettingPanel;
    [SerializeField] GameObject login;

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
        redTeamChangeButton.onClick.AddListener(() => teamManager.ChangeTeam(Team.Red));
        blueTeamChangeButton.onClick.AddListener(() => teamManager.ChangeTeam(Team.Blue));
        waitTeamChangeButton.onClick.AddListener(() => teamManager.ChangeTeam(Team.Wait));
        gameSettingButton.onClick.AddListener(() => GameSettingPanelActive(true));
        gameSettingCloseButton.onClick.AddListener(() => GameSettingPanelActive(false));
    }

    private void OnEnable()
    {
        Subscribe();
    }

    public void RecreateRoom()
    {
        if (Manager.Game.State == Game.State.Game)
        {
            login.SetActive(false);
            room.SetActive(true);
            OnJoinedRoom();

            Manager.Game.State = Game.State.Lobby;
        }
    }

    private void OnDisable()
    {
        UnSubscribe();
    }
    #endregion

    #region EventSubscribe
    private void Subscribe()
    {
        PlayerSlot.OnKick += Kick;

        redTeamChangeButton.onClick.AddListener(OnClickRedTeamChange);
        blueTeamChangeButton.onClick.AddListener(OnClickBlueTeamChange);
        waitTeamChangeButton.onClick.AddListener(OnClickWaitTeamChange);
        gameSettingButton.onClick.AddListener(OnClickGameSettingOpen);
        gameSettingCloseButton.onClick.AddListener(OnClickGameSettingClose);

        exitButton.onClick.AddListener(LeaveRoom);
        readyButton.onClick.AddListener(Ready);
        startButton.onClick.AddListener(GameStart);
        mapCloseButton.onClick.AddListener(CloseMapPanel);
        mapChangeButton.onClick.AddListener(OpenMapPanel);
        damageTypeButton.onClick.AddListener(ChangeDamageType);
        turnSwitchButton.onClick.AddListener(TurnTypeSwitch);
    }

    private void UnSubscribe()
    {
        PlayerSlot.OnKick -= Kick;

        redTeamChangeButton.onClick.RemoveListener(OnClickRedTeamChange);
        blueTeamChangeButton.onClick.RemoveListener(OnClickBlueTeamChange);
        waitTeamChangeButton.onClick.RemoveListener(OnClickWaitTeamChange);
        gameSettingButton.onClick.RemoveListener(OnClickGameSettingOpen);
        gameSettingCloseButton.onClick.RemoveListener(OnClickGameSettingClose);

        exitButton.onClick.RemoveListener(LeaveRoom);
        readyButton.onClick.RemoveListener(Ready);
        startButton.onClick.RemoveListener(GameStart);
        mapCloseButton.onClick.RemoveListener(CloseMapPanel);
        mapChangeButton.onClick.RemoveListener(OpenMapPanel);
        damageTypeButton.onClick.RemoveListener(ChangeDamageType);
        turnSwitchButton.onClick.RemoveListener(TurnTypeSwitch);
    }
    #endregion

    private void Init()
    {
        isReady = PhotonNetwork.IsMasterClient ? true : false;
        isRandom = true;
    }

    private void LeaveRoom()
    {
        StartCoroutine(LeaveRoomRoutine());
    }

    private IEnumerator LeaveRoomRoutine()
    {
        Manager.UI.FadeScreen.FadeIn(.5f);

        yield return new WaitForSeconds(.5f);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Destroy(playerSlotDic[player.ActorNumber].gameObject);
        }

        playerSlotDic.Clear();
        PhotonNetwork.LeaveRoom();

        yield return new WaitForSeconds(.5f);

        Manager.UI.FadeScreen.FadeOut(.5f);
    }

    #region Kick
    private void Kick(Player player)
    {
        photonView.RPC(nameof(Kick_RPC), player);
    }
    [PunRPC]
    private void Kick_RPC()
    {
        StartCoroutine(KickRoutine());
    }
    private IEnumerator KickRoutine()
    {
        Manager.UI.FadeScreen.FadeIn(.5f);

        yield return new WaitForSeconds(.5f);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Destroy(playerSlotDic[player.ActorNumber].gameObject);
        }

        playerSlotDic.Clear();
        PhotonNetwork.LeaveRoom();

        yield return new WaitForSeconds(.5f);
        Manager.UI.PopUpUI.Show("방에서 강퇴 되었습니다.");
        Manager.UI.FadeScreen.FadeOut(.5f);
    }
    #endregion

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
        gameSettingButton.interactable = PhotonNetwork.IsMasterClient;

        if (PhotonNetwork.IsMasterClient)
        {
            startButton
                .gameObject.SetActive(true);
            readyButton.gameObject.SetActive(false);
        }
        else
        {
            startButton.gameObject.SetActive(false);
            readyButton.gameObject.SetActive(true);
        }
    }

    private void CreatePlayerSlot()
    {
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
        Team team = player.GetTeam();
        if (team == Team.Red)
            return redPlayerContent;
        else if (team == Team.Blue)
            return bluePlayerContent;
        else
            return waitPlayerContent;
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
            CheckButtons();
        }
    }

    private void ReadyPropertyUpdate()
    {
        PhotonNetwork.LocalPlayer.SetReady(isReady);
        CheckButtons();
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
    private void CheckButtons()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            redTeamChangeButton.interactable = true;
            blueTeamChangeButton.interactable = true;
            waitTeamChangeButton.interactable = true;
        }
        else
        {
            redTeamChangeButton.interactable = !isReady;
            blueTeamChangeButton.interactable = !isReady;
            waitTeamChangeButton.interactable = !isReady;
        }
        startButton.interactable = currentReadyCount == PhotonNetwork.CurrentRoom.MaxPlayers;
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

    #region TeamDamageType
    private void ChangeDamageType()
    {
        PhotonNetwork.CurrentRoom.SetDamageType(!PhotonNetwork.CurrentRoom.GetDamageType());
    }
    private void ChangeDamageTypeText()
    {
        damageType.text = PhotonNetwork.CurrentRoom.GetDamageType() ? "팀 데미지 허용" : "팀 데미지 금지";
    }
    #endregion

    private void CheckFull()
    {
        bool isFull = PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers;

        PhotonNetwork.CurrentRoom.SetFull(isFull);
    }

    private void GameSettingPanelActive(bool isActive) => gameSettingPanel.SetActive(isActive);

    private void GameStart()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if(player.GetGamePlay())
            {
                Manager.UI.PopUpUI.Show("누군가 아직 게임 내부에 있습니다.", Color.red);
                return;
            }
        }

        if (currentReadyCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log("방에 인원이 부족하거나 모든 플레이어가 레디하지 않았습니다.");
            return;
        }

        if (teamManager.GetWaitPlayerCount() > 0)
        {
            Manager.UI.PopUpUI.Show("누군가가 대기자에 포함되어 있습니다.");
            return;
        }
        
        PhotonNetwork.CurrentRoom.SetGameStart(true);
        photonView.RPC(nameof(LoadScene), RpcTarget.All);
    }

    [PunRPC]
    private void LoadScene()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    #region Events
    private void OnClickRedTeamChange()
    {
        teamManager.ChangeTeam(Team.Red);
    }

    private void OnClickBlueTeamChange()
    {
        teamManager.ChangeTeam(Team.Blue);
    }

    private void OnClickWaitTeamChange()
    {
        teamManager.ChangeTeam(Team.Wait);
    }

    private void OnClickGameSettingOpen()
    {
        GameSettingPanelActive(true);
    }

    private void OnClickGameSettingClose()
    {
        GameSettingPanelActive(false);
    }
    #endregion

    #region PhotonCallbacks
    public void OnJoinedRoom()
    {
        PhotonNetwork.LocalPlayer.SetTeam(teamManager.GetRemainingTeam());
        roomName.text = PhotonNetwork.CurrentRoom.Name;
        Manager.UI.FadeScreen.FadeOut(.5f);

        Init();
        MapChange();
        CreateMapSlot();
        CreatePlayerSlot();
        UpdateAllPlayerSlot();
        UpdateReadyCountText();
        ReadyPropertyUpdate();
        UpdateTurnType();
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        CreatePlayerSlot(newPlayer);
        CheckFull();
    }
    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        DestroyPlayerSlot(otherPlayer);
        UpdateReadyCountText();
        UpdateAllPlayerSlot();
        CheckFull();
    }
    public void OnRoomPropertiesUpdate()
    {
        MapChange();
        UpdateTurnType();
        ChangeDamageTypeText();
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
