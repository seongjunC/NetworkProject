using Photon.Pun;
using Photon.Realtime;
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

    [Header("Sub Panel")]
    [SerializeField] GameObject roomSelectPanel;
    [SerializeField] GameObject roomCreatePanel;

    [Header("Sub Buttons")]
    [SerializeField] Button roomCloseSelectButton;
    [SerializeField] Button roomCreateOpenButton;
    [SerializeField] Button roomCreateCloseButton;
    [SerializeField] Button roomCreateButton;
    [SerializeField] Button fastJoinButton;

    private bool isRoomCreate;

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

        optionButton.onClick    .AddListener(Manager.UI.settingPanel.Show);
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
        optionButton.onClick    .RemoveListener(Manager.UI.settingPanel.Show);
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
            Manager.UI.PopUpUI.Show("�� �̸��� �Է��� �ּ���.");
            return;
        }

        if (int.TryParse(maxPlayerField.text, out int maxPlayer))
        {
            if (maxPlayer % 2 != 0)
            {
                Manager.UI.PopUpUI.Show("¦���� �Է��� �ּ���.");
                return;
            }
            if (maxPlayer > maxPlayerCount)
            {
                Manager.UI.PopUpUI.Show($"{maxPlayerCount} ���� ���� ���� �Է��� �ּ���.");
                return;
            }
        }

        if (isPassword.isOn)
        {
            if (string.IsNullOrWhiteSpace(passwordField.text))
            {
                Manager.UI.PopUpUI.Show("��й�ȣ�� �پ�ų� ��ĭ�� �� �����ϴ�.");
                return;
            }
        }

        isRoomCreate = true;

        RoomOptions option = new RoomOptions();
        option.MaxPlayers = maxPlayer;
        option.CustomRoomPropertiesForLobby = new string[] { "Map", "Password", "Full"};               
        PhotonNetwork.CreateRoom(roomNameField.text, option);        
        roomNameField.text = "";
        maxPlayerField.text = "";
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
        PhotonNetwork.JoinRandomOrCreateRoom();
    }
    private void LogOut()
    {
        title.SetActive(true);
        lobby.SetActive(false);        
    }
    private void GameOut() => Application.Quit();
    private void ActiveRoomSelectPanel(bool isActive)
    {
        roomSelectPanel.SetActive(isActive);
        lobby.SetActive(!isActive);
    }
    private void OpenRoomSelectPanel() => ActiveRoomSelectPanel(true);
    private void CloseRoomSelectPanel() => ActiveRoomSelectPanel(false);    
    #endregion

    #region PhotonCallbacks
    public override void OnCreatedRoom()
    {
        PhotonNetwork.CurrentRoom.SetMap(0);
        PhotonNetwork.CurrentRoom.SetTurnRandom(true);
        PhotonNetwork.CurrentRoom.SetFull(false);

        if (isPassword.isOn)
        {
            PhotonNetwork.CurrentRoom.SetPassword(passwordField.text);            
        }        

        passwordField.text = "";
        Debug.Log("�� ���� �Ϸ�");        
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        isRoomCreate = false;
    }

    public override void OnLeftLobby()
    {
        lobby.SetActive(false);

        Debug.Log("�κ� ������");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("�� ���� �Ϸ�");

        room.SetActive(true);
        lobby.SetActive(false);
        roomSelectPanel.SetActive(false);
        roomCreatePanel.SetActive(false);

        if (Manager.Data.PlayerData.Name == "")
            Manager.UI.NickNameSelectPanel.Show();

        roomManager.OnJoinedRoom();
    }

    public override void OnLeftRoom()    
    {
        lobby.SetActive(true);
        room.SetActive(false);
        roomManager.OnLeftRoom();
        isRoomCreate = false;
        Debug.Log("�� ����");              
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
