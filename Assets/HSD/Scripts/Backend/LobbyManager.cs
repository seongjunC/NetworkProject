using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Button logOutButton;
    [SerializeField] GameObject nickNameSelectPanel;

    [Header("Room")]
    [SerializeField] GameObject roomPrefab;
    [SerializeField] Transform roomContent;
    private static readonly Dictionary<string, RoomSlot> roomListDic = new Dictionary<string, RoomSlot>();

    [Header("RoomCreate")]
    [SerializeField] TMP_InputField roomNameField;
    [SerializeField] TMP_InputField maxPlayerField;
    [SerializeField] int maxPlayerCount;

    [Header("Panel")]
    [SerializeField] GameObject lobby;
    [SerializeField] GameObject room;

    #region LifeCycle
    public override void OnEnable()
    {
        base.OnEnable();

        logOutButton.onClick.AddListener(Manager.Firebase.LogOut);

        if (Manager.Data.PlayerData.Name == "")
            nickNameSelectPanel.SetActive(true);
    }

    public override void OnDisable()
    {
        logOutButton.onClick.RemoveListener(Manager.Firebase.LogOut);
    }
    #endregion

    public void CreateRoom()
    {
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
            }
        }
       
        RoomOptions option = new RoomOptions();
        option.MaxPlayers = maxPlayer;        
        option.CustomRoomPropertiesForLobby = new string[] { "Map" };
        PhotonNetwork.CreateRoom(roomNameField.text, option);
        roomNameField.text = "";
        maxPlayerField.text = "";
    }

    #region PhotonCallbacks
    public override void OnCreatedRoom()
    {
        ExitGames.Client.Photon.Hashtable roomProperty = new ExitGames.Client.Photon.Hashtable();
        roomProperty["Map"] = 0;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperty);
        Debug.Log("방 생성 완료");
    }
    public override void OnJoinedRoom()
    {
        lobby.SetActive(false);
        room.SetActive(true);
        Debug.Log("방 입장 완료");
    }
    public override void OnLeftRoom()    
    {
        lobby.SetActive(true);
        room.SetActive(false);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                if (roomListDic.TryGetValue(room.Name, out RoomSlot roomSlot))
                {
                    Destroy(roomSlot);
                    roomListDic.Remove(room.Name);
                }

                continue;
            }

            if (!roomListDic.ContainsKey(room.Name))
            {
                RoomSlot slot = Instantiate(roomPrefab, roomContent).GetComponent<RoomSlot>();
                slot.SetUp(room);
                roomListDic.Add(room.Name, slot);
            }
            else
            {
                roomListDic[room.Name].SetUp(room);
            }
        }
    }
    #endregion
}
