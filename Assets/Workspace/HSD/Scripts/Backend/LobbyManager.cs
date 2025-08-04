using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{    
    [SerializeField] RoomManager roomManager;

    [Header("Room")]
    [SerializeField] GameObject roomPrefab;
    [SerializeField] Transform roomContent;
    private static readonly Dictionary<string, RoomSlot> roomListDic = new Dictionary<string, RoomSlot>();

    [Header("RoomCreate")]
    [SerializeField] TMP_InputField roomNameField;
    [SerializeField] TMP_InputField maxPlayerField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] Toggle isPassword;
    [SerializeField] int maxPlayerCount;

    [Header("Main Panel")]
    [SerializeField] GameObject title;
    [SerializeField] GameObject lobby;    
    [SerializeField] GameObject room;   
    [SerializeField] PasswordPanel passwordPanel;

    [Header("Main Buttons")]
    [SerializeField] Button roomOpenSelectButton;
    [SerializeField] Button optionButton;
    [SerializeField] Button gameOutButton;
    [SerializeField] Button logOutButton;

    [Header("Coupon")]
    [SerializeField] Button couponButton;
    [SerializeField] GameObject couponPanel;

    [Header("Sub Panel")]
    [SerializeField] GameObject roomSelectPanel;
    [SerializeField] GameObject roomCreatePanel;

    [Header("Sub Buttons")]
    [SerializeField] Button roomCloseSelectButton;
    [SerializeField] Button roomCreateOpenButton;
    [SerializeField] Button roomCreateCloseButton;
    [SerializeField] Button roomCreateButton;
    [SerializeField] Button fastJoinButton;
    [SerializeField] Button roomLeftButton;
    [SerializeField] Button roomRightButton;

    [Header("Gacha")]
    [SerializeField] GameObject gachaPanel;
    [SerializeField] Button gachaButton;

    [Header("Tank")]
    [SerializeField] GameObject tankInventory;
    [SerializeField] Button tankInventoryButton;

    [Header("Room Max Idx")]
    [SerializeField] int roomMax = 8;
    private bool isRoomCreate;
    private int _currentRoomSelectIdx = 1;
    public int currentRoomSelectIdx { get => _currentRoomSelectIdx; set { _currentRoomSelectIdx = value; ChangeCurrentRoomIdx(); RoomSelectButtonActiveCheck(); } }

    #region LifeCycle
    public override void OnEnable()
    {
        base.OnEnable();        

        Subscribe();
        isRoomCreate = false;
    }

    public override void OnDisable()
    {
        UnSubscribe();
    }

    #endregion

    #region EventSubscribe
    private void Subscribe()
    {
        Manager.Firebase.OnLogOut += GoTitle;

        tankInventoryButton.onClick.AddListener(OpenTankInventory);

        couponButton.onClick    .AddListener(OpenCouponPanel);
        gachaButton.onClick     .AddListener(GachaOpen);
        roomRightButton.onClick .AddListener(RoomIndexPlus);
        roomLeftButton.onClick  .AddListener(RoomIndexMinus);

        optionButton.onClick    .AddListener(Manager.UI.SettingPanel.Show);
        gameOutButton.onClick   .AddListener(GameOut);

        fastJoinButton.onClick          .AddListener(RandomMatching);
        roomCreateButton.onClick        .AddListener(CreateRoom);
        roomCreateOpenButton.onClick    .AddListener(OpenRoomCreatePanel);
        roomCreateCloseButton.onClick   .AddListener(CloseRoomCreatePanel);

        roomOpenSelectButton.onClick    .AddListener(OpenRoomSelectPanel);
        roomCloseSelectButton.onClick   .AddListener(CloseRoomSelectPanel);

        logOutButton.onClick        .AddListener(LogOut);
        isPassword.onValueChanged   .AddListener(PasswordToggleChanged);

        roomNameField.onEndEdit .AddListener(EnterCreateRoom);
        passwordField.onEndEdit .AddListener(EnterCreateRoom);
        maxPlayerField.onEndEdit.AddListener(EnterCreateRoom);
    }
    private void UnSubscribe()
    {
        Manager.Firebase.OnLogOut -= GoTitle;

        tankInventoryButton.onClick.RemoveListener(OpenTankInventory);

        couponButton.onClick    .RemoveListener(OpenCouponPanel);
        gachaButton.onClick     .RemoveListener(GachaOpen);
        roomRightButton.onClick .RemoveListener(RoomIndexPlus);
        roomLeftButton.onClick  .RemoveListener(RoomIndexMinus);

        optionButton.onClick    .RemoveListener(Manager.UI.SettingPanel.Show);
        gameOutButton.onClick   .RemoveListener(GameOut);

        fastJoinButton.onClick          .RemoveListener(RandomMatching);
        roomCreateButton.onClick        .RemoveListener(CreateRoom);
        roomCreateOpenButton.onClick    .RemoveListener(OpenRoomCreatePanel);
        roomCreateCloseButton.onClick   .RemoveListener(CloseRoomCreatePanel);

        roomOpenSelectButton.onClick    .RemoveListener(OpenRoomSelectPanel);
        roomCloseSelectButton.onClick   .RemoveListener(CloseRoomSelectPanel);

        logOutButton.onClick        .RemoveListener(LogOut);
        isPassword.onValueChanged   .RemoveListener(PasswordToggleChanged);

        roomNameField.onEndEdit .RemoveListener(EnterCreateRoom);
        passwordField.onEndEdit .RemoveListener(EnterCreateRoom);
        maxPlayerField.onEndEdit.RemoveListener(EnterCreateRoom);
    }
    #endregion

    public void CreateRoom()
    {
        if (isRoomCreate) return;        

        if (string.IsNullOrEmpty(roomNameField.text))
        {
            Manager.UI.PopUpUI.Show("방 이름을 입력해 주세요.");
            return;
        }

        if(string.IsNullOrWhiteSpace(maxPlayerField.text))
        {
            Manager.UI.PopUpUI.Show("인원을 입력해주세요.\n(뛰어쓴 공간이 있어서는 안됩니다.)", Color.red);
            return;
        }

        if (int.TryParse(maxPlayerField.text, out int maxPlayer))
        {            
            if (maxPlayer <= 0)
            {
                Manager.UI.PopUpUI.Show("0보다 큰 값을 입력해 주세요.");
                return;
            }
            if (maxPlayer % 2 != 0)
            {
                Manager.UI.PopUpUI.Show("짝수를 입력해 주세요.");
                return;
            }
            if (maxPlayer > maxPlayerCount)
            {
                Manager.UI.PopUpUI.Show($"{maxPlayerCount} 보다 낮은 값을 입력해 주세요.");
                return;
            }
        }

        if (isPassword.isOn)
        {
            if (string.IsNullOrWhiteSpace(passwordField.text))
            {
                Manager.UI.PopUpUI.Show("비밀번호는 뛰어쓰거나 빈칸일 수 없습니다.");
                return;
            }
        }

        isRoomCreate = true;

        StartCoroutine(RoomCreateRoutine(maxPlayer));
    }

    private void EnterCreateRoom(string s)
    {
        if (Input.GetKeyDown(KeyCode.Return))
            CreateRoom();
    }

    private void PasswordToggleChanged(bool isPassword)
    {
        passwordField.interactable = isPassword;        
    }

    private void OpenPasswordPanel(RoomInfo room)
    {
        passwordPanel.SetUp(room);
        passwordPanel.gameObject.SetActive(true);
    }

    private void GoTitle()
    {
        lobby.SetActive(false);
        room.SetActive(false);
        roomSelectPanel.SetActive(false);
        roomCreatePanel.SetActive(false);
        title.SetActive(true);
    }

    private IEnumerator RoomCreateRoutine(int maxPlayer)
    {
        Manager.UI.FadeScreen.FadeIn(.5f);
        yield return new WaitForSeconds(.5f);
        RoomOptions option = new RoomOptions();
        option.MaxPlayers = maxPlayer;
        option.CustomRoomPropertiesForLobby = new string[] { "Map", "Password", "Full", "GameStart" };
        PhotonNetwork.CreateRoom(roomNameField.text, option);
        roomNameField.text = "";
        maxPlayerField.text = "";       
    }

    private IEnumerator RandomRoomJoinRoutine()
    {
        Manager.UI.FadeScreen.FadeIn(.5f);
        yield return new WaitForSeconds(.5f);
        
        PhotonNetwork.JoinRandomRoom();

        yield return new WaitForSeconds(1f);
        Manager.UI.FadeScreen.FadeOut(.5f);
    }

    #region ButtonEvent
    private void OpenRoomCreatePanel()
    {
        roomCreatePanel.SetActive(true);
        isPassword.isOn = false;
        passwordField.interactable = isPassword.isOn;
    }
    private void CloseRoomCreatePanel()
    {
        roomCreatePanel.SetActive(false);

        roomNameField.text  = "";
        passwordField.text  = "";
        maxPlayerField.text = "";
    }
    private void RandomMatching()
    {
        StartCoroutine(RandomRoomJoinRoutine());
    }
    private void LogOut()
    {
        Manager.Firebase.LogOut();       
    }
    private void GameOut() => Application.Quit();
    private void ActiveRoomSelectPanel(bool isActive)
    {
        roomSelectPanel.SetActive(isActive);
        lobby.SetActive(!isActive);
    }
    private void OpenRoomSelectPanel() => ActiveRoomSelectPanel(true);
    private void CloseRoomSelectPanel() => ActiveRoomSelectPanel(false);
    private void ChangeCurrentRoomIdx()
    {
        int start = (currentRoomSelectIdx - 1) * roomMax;
        int end = currentRoomSelectIdx * roomMax;

        int count = 0;

        foreach (RoomSlot slot in roomListDic.Values)
        {
            if (count >= start && count < end)
            {
                slot.gameObject.SetActive(true);
            }
            else
            {
                slot.gameObject.SetActive(false);
            }

            count++;
        }
    }
    private void RoomIndexPlus()
    {
        int maxPage = Mathf.CeilToInt((float)roomListDic.Count / roomMax);

        if (currentRoomSelectIdx < maxPage)
        {
            currentRoomSelectIdx++;
        }
    }
    private void RoomIndexMinus()
    {
        if (currentRoomSelectIdx > 1)
        {
            currentRoomSelectIdx--;
        }
    }
    private void GachaOpen()
    {
        lobby.SetActive(false);
        gachaPanel.SetActive(true);
    }
    private void OpenTankInventory() => tankInventory.SetActive(true);
    #endregion
    private void RoomSelectButtonActiveCheck()
    {
        int maxPage = Mathf.CeilToInt((float)roomListDic.Count / roomMax);

        roomRightButton.interactable = maxPage > currentRoomSelectIdx;
        roomLeftButton.interactable = 1 < currentRoomSelectIdx;        
    }

    #region PhotonCallbacks
    public override void OnCreatedRoom()
    {
        PhotonNetwork.CurrentRoom.SetMap(0);
        PhotonNetwork.CurrentRoom.SetTurnRandom(true);
        PhotonNetwork.CurrentRoom.SetFull(false);
        PhotonNetwork.CurrentRoom.SetDamageType(false);
        PhotonNetwork.CurrentRoom.SetGameStart(false);        

        if (isPassword.isOn)
        {
            PhotonNetwork.CurrentRoom.SetPassword(passwordField.text);            
        }        

        passwordField.text = "";
        Debug.Log("방 생성 완료");
        Manager.UI.FadeScreen.FadeOut(.5f);
    }

    private void OpenCouponPanel()
    {
        if(Manager.Game.State != Game.State.Lobby)
            return;

        couponPanel.SetActive(true);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        isRoomCreate = false;
    }

    public override void OnLeftLobby()
    {
        lobby.SetActive(false);

        Debug.Log("로비에 접속함");
    }

    public override void OnJoinedLobby()
    {
        currentRoomSelectIdx = 1;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료");

        room.SetActive(true);
        lobby.SetActive(false);
        roomSelectPanel.SetActive(false);
        roomCreatePanel.SetActive(false);

        if (Manager.Data.PlayerData.Name == "")
            Manager.UI.NickNameSelectPanel.Show();

        roomManager.OnJoinedRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Manager.UI.PopUpUI.Show("접속 가능한 방이 없습니다.", Color.red);
    }

    public override void OnLeftRoom()    
    {
        lobby.SetActive(true);
        room.SetActive(false);
        roomManager.OnLeftRoom();
        isRoomCreate = false;
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if(newPlayer != PhotonNetwork.LocalPlayer)
            roomManager.OnPlayerEnteredRoom(newPlayer);
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer != PhotonNetwork.LocalPlayer)            
            roomManager.OnPlayerLeftRoom(otherPlayer);
    }
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        roomManager.OnMasterClientSwitched(newMasterClient);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        CreateRoomSlots(roomList);
    }

    private void CreateRoomSlots(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                if (roomListDic.TryGetValue(room.Name, out RoomSlot roomSlot))
                {
                    roomListDic[room.Name].OnPasswordRoomSelected -= OpenPasswordPanel;
                    Destroy(roomSlot.gameObject);
                    roomListDic.Remove(room.Name);
                }

                continue;
            }

            if (!roomListDic.ContainsKey(room.Name))
            {
                RoomSlot slot = Instantiate(roomPrefab, roomContent).GetComponent<RoomSlot>();
                slot.SetUp(room);
                roomListDic.Add(room.Name, slot);
                roomListDic[room.Name].OnPasswordRoomSelected += OpenPasswordPanel;                
            }
            else
            {
                roomListDic[room.Name].SetUp(room);
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        roomManager.OnRoomPropertiesUpdate();

        foreach (var slot in roomListDic.Values)
        {
            slot.Refresh();
        }
    }
    public override void OnPlayerPropertiesUpdate(Player target,
        ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        roomManager.OnPlayerPropertiesUpdate(target);       
    }
    #endregion
}
