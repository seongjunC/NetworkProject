using Game;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject playerSlotPrefab;
    [SerializeField] Transform playerContent;
    private Dictionary<int, PlayerSlot> playerSlotDic = new Dictionary<int, PlayerSlot>();

    [Header("Map")]
    [SerializeField] GameObject mapPrefab;
    [SerializeField] Transform mapContent;
    [SerializeField] GameObject mapSelectPanel;
    [SerializeField] RawImage mapImage;
    [SerializeField] Button mapChangeButton;
    private int mapIdx;

    [Header("Game")]
    [SerializeField] Button readyButton;
    [SerializeField] Button startButton;
    [SerializeField] Button turnSwitchButton;
    [SerializeField] TMP_Text turnType;
    [SerializeField] TMP_Text readyCount;

    [Header("Ready")]
    [SerializeField] Color readyColor;
    [SerializeField] Color defaultColor;

    [Header("Panel")]
    [SerializeField] GameObject lobby;
    [SerializeField] GameObject room;

    private bool isReady;
    private bool isRandom;
    private int currentReadyCount;

    #region LifeCycle
    private void OnEnable()
    {
        readyButton.onClick.AddListener(Ready);
        startButton.onClick.AddListener(GameStart);
        mapChangeButton.onClick.AddListener(OpenMapPanel);
        turnSwitchButton.onClick.AddListener(TurnTypeSwitch);
    }
    private void OnDisable()
    {
        readyButton.onClick.RemoveListener(Ready);
        startButton.onClick.RemoveListener(GameStart);
        mapChangeButton.onClick.RemoveListener(OpenMapPanel);
        turnSwitchButton.onClick.RemoveListener(TurnTypeSwitch);
    }
    #endregion

    #region PlayerSlot
    public void CreatePlayerSlot(Player player)
    {
        if (!playerSlotDic.ContainsKey(player.ActorNumber))
        {
            GameObject obj = Instantiate(playerSlotPrefab, playerContent);
            PlayerSlot playerSlot = obj.GetComponent<PlayerSlot>();
            playerSlot.SetUp(player);
            playerSlotDic.Add(player.ActorNumber, playerSlot);
        }
        else
        {
            playerSlotDic[player.ActorNumber].SetUp(player);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            mapChangeButton.interactable = true;
            startButton.interactable = true;
        }
        else
        {
            mapChangeButton.interactable = false;
            startButton.interactable = false;
        }
    }

    public void CreatePlayerSlot()
    {
        PhotonNetwork.AutomaticallySyncScene = true;

        if (!PhotonNetwork.IsMasterClient)
        {
            startButton.interactable = false;
            mapChangeButton.interactable = false;
            MapChange();
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject obj = Instantiate(playerSlotPrefab, playerContent);
            PlayerSlot playerSlot = obj.GetComponent<PlayerSlot>();
            playerSlot.SetUp(player);
            playerSlotDic.Add(player.ActorNumber, playerSlot);
        }
    }

    public void DestroyPlayerSlot(Player player)
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
    #endregion

    public void Init()
    {
        isReady = false;
        isRandom = true;
    }

    public void LeaveRoom()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            Destroy(playerSlotDic[player.ActorNumber].gameObject);
        }

        playerSlotDic.Clear();

        PhotonNetwork.LeaveRoom();
    }
       
    #region Ready
    public void Ready()
    {        
        isReady = !isReady;
        playerSlotDic[PhotonNetwork.LocalPlayer.ActorNumber].UpdateReady(isReady ? readyColor : defaultColor);
        ReadyPropertyUpdate();
        UpdateReadyCountText();
    }

    public void ReadyCheck(Player player)
    {
        if(player.CustomProperties.TryGetValue("Ready", out object value))
        {
            playerSlotDic[player.ActorNumber].UpdateReady(player.GetReady() ? readyColor : defaultColor);
            UpdateReadyCountText();
        }
    }

    public void ReadyPropertyUpdate()
    {
        PhotonNetwork.LocalPlayer.SetReady(isReady);
    }
    public void UpdateReadyCountText()
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
    public void MapChange()
    {
        mapIdx = (int)PhotonNetwork.CurrentRoom.CustomProperties["Map"];
        mapImage.texture = Manager.Resources.Load<Texture2D>($"MapIcon/{((MapType)mapIdx).ToString()}"); 
    }
    public void CreateMapSlot()
    {
        for (int i = 0; i <= (int)MapType.Length-1; i ++)
        {
            GameObject obj = Instantiate(mapPrefab, mapContent);
            MapSlot slot = obj.GetComponent<MapSlot>();
            slot.SetUp((MapType)i);
        }        
    }
    public void OpenMapPanel()
    {
        mapSelectPanel.SetActive(true);
    }
    public void CloseMapPanel()
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

    public void UpdateTurnType()
    {
        turnType.text = PhotonNetwork.CurrentRoom.GetTurnRandom() ? "Random" : "NotRandom";
    }
    #endregion
    public void GameStart()
    {
        if(currentReadyCount != PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log("방에 인원이 부족하거나 모든 플레이어가 레디하지 않았습니다.");
            return;
        }

        //PhotonNetwork.LoadLevel(""); // 씬이동
    }
}
