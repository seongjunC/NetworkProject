using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{    
    [SerializeField] GameObject nickNameSelectPanel;
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

    [Header("Sub Buttons")]
    [SerializeField] Button roomCloseSelectButton;    

    private bool isRoomCreate;

    #region LifeCycle
    private void Start()
    {
        lobby.SetActive(true);
        room.SetActive(false);
        PhotonNetwork.JoinLobby();
    }
    public override void OnEnable()
    {
        base.OnEnable();

        Subscribe();
        isRoomCreate = false;
        if (Manager.Data.PlayerData.Name == "")
            nickNameSelectPanel.SetActive(true);
    }

    public override void OnDisable()
    {
        UnSubscribe();
    }

    #endregion

    #region EventSubscribe
    private void Subscribe()
    {
        roomOpenSelectButton.onClick.AddListener(OpenRoomSelectPanel);
        roomCloseSelectButton.onClick.AddListener(CloseRoomSelectPanel);

        logOutButton.onClick.AddListener(LogOut);
        isPassword.onValueChanged.AddListener(PasswordToggleChanged);

        roomNameField.onEndEdit.AddListener(EnterCreateRoom);
        passwordField.onEndEdit.AddListener(EnterCreateRoom);
        maxPlayerField.onEndEdit.AddListener(EnterCreateRoom);
    }
    private void UnSubscribe()
    {
        roomOpenSelectButton.onClick.RemoveListener(OpenRoomSelectPanel);
        roomCloseSelectButton.onClick.RemoveListener(CloseRoomSelectPanel);

        logOutButton.onClick.RemoveListener(LogOut);
        isPassword.onValueChanged.RemoveListener(PasswordToggleChanged);

        roomNameField.onEndEdit.RemoveListener(EnterCreateRoom);
        passwordField.onEndEdit.RemoveListener(EnterCreateRoom);
        maxPlayerField.onEndEdit.RemoveListener(EnterCreateRoom);
    }
    #endregion

    public void CreateRoom()
    {
        if (isRoomCreate) return;

        isRoomCreate = true;

        if (string.IsNullOrEmpty(roomNameField.text))
        {
            Manager.UI.PopUpUI.Show("방 이름을 입력해 주세요.");
            return;
        }

        if (int.TryParse(maxPlayerField.text, out int maxPlayer))
        {
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

    #region ButtonEvent
    private void LogOut()
    {
        title.SetActive(true);
        lobby.SetActive(false);        
    }
    private void ActiveRoomSelectPanel(bool isActive)
    {
        roomSelectPanel.SetActive(isActive);
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
        Debug.Log("방 생성 완료");        
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

    public override void OnJoinedRoom()
    {
        Debug.Log("방 입장 완료");
        lobby.SetActive(false);
        room.SetActive(true);

        roomManager.OnJoinedRoom();
    }

    public override void OnLeftRoom()    
    {
        lobby.SetActive(true);
        room.SetActive(false);
        roomManager.OnLeftRoom();
        isRoomCreate = false;
        Debug.Log("방 나감");              
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
        Debug.Log("OnRoomListUpdate");
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                Debug.Log($"RemoveRoom : {room.Name}");
                if (roomListDic.TryGetValue(room.Name, out RoomSlot roomSlot))
                {
                    Debug.Log($"RemoveRoom1 : {room.Name}");
                    roomListDic[room.Name].OnPasswordRoomSelected -= OpenPasswordPanel;
                    Destroy(roomSlot.gameObject);
                    roomListDic.Remove(room.Name);
                }

                continue;
            }

            if (!roomListDic.ContainsKey(room.Name))
            {
                Debug.Log($"Instantiate Room : {room.Name}");
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
