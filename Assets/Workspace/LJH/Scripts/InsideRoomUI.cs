using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class InsideRoomUI : MonoBehaviour
{
    [SerializeField] private Image mapImage;
    [SerializeField] private Sprite[] mapSprites;
    [SerializeField] private Button startButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button outButton;
    [SerializeField] private Button sendButton;
    [SerializeField] private Button previousMapButton;
    [SerializeField] private Button nextMapButton;

    [Header("연결할 패널")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject inGamePanel;

    private int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        UpdateMapImage();
    }

    private void Awake()
    {
        startButton.onClick.AddListener(OnClickStart);
        readyButton.onClick.AddListener(OnClickReady);
        outButton.onClick.AddListener(OnClickOut);
        sendButton.onClick.AddListener(() => Debug.Log("Send button clicked")); // To do : 채팅 메시지 전송 로직 구현
        previousMapButton.onClick.AddListener(OnClickPreviousMap);
        nextMapButton.onClick.AddListener(OnClickNextMap);
    }
    void Update()
    {
        // 방장이면 시작 버튼 활성화, 플레이어면 준비 버튼 활성화
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        readyButton.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
    }
    private void OnClickStart()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // 모든 플레이어가 Ready인지 확인
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey("IsReady") || !(bool)p.CustomProperties["IsReady"])
            {
                // 플레이어가 준비되지 않은 경우
                // To do : 플레이어에게 준비되지 않았음을 알리는 로직 구현
                Debug.Log($"{p.NickName} is not ready!");
                return;
            }
        }

        PhotonNetwork.LoadLevel("GameScene"); // 예: 게임 씬 이름
    }
    private void OnClickReady()
    {
        //To do : 플레이어 준비 상태 토글 로직 구현
        ToggleReady();
    }
    private void OnClickOut()
    {
        //To do : 방 나가기 로직 구현
        gameObject.SetActive(false);         // 방 패널 닫기
        lobbyPanel.SetActive(true);         // 로비 패널 열기
    }
    private void OnClickPreviousMap()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = mapSprites.Length - 1;

        UpdateMapImage();
    }
    private void OnClickNextMap()
    {
        currentIndex++;
        if (currentIndex >= mapSprites.Length)
            currentIndex = 0;

        UpdateMapImage();
    }
    private void UpdateMapImage()
    {
        if (mapSprites.Length > 0)
        {
            mapImage.sprite = mapSprites[currentIndex];
        }
    }

    public int GetSelectedMapIndex()
    {
        return currentIndex;
    }

    public void ToggleReady()
    {
        bool current = PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("IsReady") &&
                       (bool)PhotonNetwork.LocalPlayer.CustomProperties["IsReady"];

        Hashtable props = new Hashtable { { "IsReady", !current } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

}
