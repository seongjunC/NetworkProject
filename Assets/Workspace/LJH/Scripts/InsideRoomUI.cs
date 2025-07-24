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

    [Header("������ �г�")]
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
        sendButton.onClick.AddListener(() => Debug.Log("Send button clicked")); // To do : ä�� �޽��� ���� ���� ����
        previousMapButton.onClick.AddListener(OnClickPreviousMap);
        nextMapButton.onClick.AddListener(OnClickNextMap);
    }
    void Update()
    {
        // �����̸� ���� ��ư Ȱ��ȭ, �÷��̾�� �غ� ��ư Ȱ��ȭ
        startButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);
        readyButton.gameObject.SetActive(!PhotonNetwork.IsMasterClient);
    }
    private void OnClickStart()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // ��� �÷��̾ Ready���� Ȯ��
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            if (!p.CustomProperties.ContainsKey("IsReady") || !(bool)p.CustomProperties["IsReady"])
            {
                // �÷��̾ �غ���� ���� ���
                // To do : �÷��̾�� �غ���� �ʾ����� �˸��� ���� ����
                Debug.Log($"{p.NickName} is not ready!");
                return;
            }
        }

        PhotonNetwork.LoadLevel("GameScene"); // ��: ���� �� �̸�
    }
    private void OnClickReady()
    {
        //To do : �÷��̾� �غ� ���� ��� ���� ����
        ToggleReady();
    }
    private void OnClickOut()
    {
        //To do : �� ������ ���� ����
        gameObject.SetActive(false);         // �� �г� �ݱ�
        lobbyPanel.SetActive(true);         // �κ� �г� ����
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
