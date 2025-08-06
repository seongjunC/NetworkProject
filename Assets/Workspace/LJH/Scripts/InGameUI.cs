using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [Header("Bars")]
    public Slider hpBar;
    public Slider moveBar;
    public Slider powerBar;

    [Header("Wind")]
    public Slider windBar;         // ��/�� �ٶ� ����
    public RectTransform flagImage; // ��� ����� �̹���

    [Header("Button")]
    public Button item1Button;     // ������1 ��ư
    public Button item2Button;     // ������2 ��ư
    public Button endTurnButton;

    // ������ ���Կ� ������ ������ ����
    private Sprite item1Sprite = null;
    private Sprite item2Sprite = null;
    private ItemData item1 = null;
    private ItemData item2 = null;

    [Header("Text")]
    public TextMeshProUGUI healthPointText; // ü�� �ؽ�Ʈ


    private PlayerController _player;
    private Fire _fire;


    void Start()
    {
        // �ʱ�ȭ
        hpBar.value = 1500f;          // ü�� ������ �ʱ�ȭ
        hpBar.maxValue = 1500f;      // ü�� ������ �ִ밪 ����
        healthPointText.text = "1500/1500";  // ü�� �ؽ�Ʈ �ʱ�ȭ
        moveBar.value = 100f;        // �̵� ������ �ʱ�ȭ
        powerBar.value = 0f;       // �Ŀ� ���� �ʱ�ȭ
        windBar.value = 0f;        // �ٶ� ���� �ʱ�ȭ
        item1Button.gameObject.SetActive(false);
        item2Button.gameObject.SetActive(false);
        // ��ư Ŭ�� �̺�Ʈ ����
        item1Button.onClick.AddListener(OnItem1Click);
        item2Button.onClick.AddListener(OnItem2Click);
        endTurnButton.onClick.AddListener(OnEndTurnClick);
    }

    public void RegisterPlayer(PlayerController playerController)
    {
        _player = playerController;
        _fire = _player.GetComponentInChildren<Fire>();

        if (_fire != null)
            powerBar.maxValue = _fire.maxPower;

        hpBar.maxValue = _player._hp;
        moveBar.maxValue = _player._movable;
        playerController.myInfo.OnItemAcquired += AddItem;
    }


    private void Update()
    {
        if (_player == null) return;

        hpBar.value = _player._hp;
        moveBar.value = _player._movable;

        if (_fire != null)
            powerBar.value = _fire.powerCharge;
    }


    public void AddItem(ItemData item)
    {
        if (item1Sprite == null)
        {
            item1 = item;
            item1Sprite = item.icon;
            item1Button.GetComponent<Image>().sprite = item.icon;
            item1Button.gameObject.SetActive(true);
        }
        else if (item2Sprite == null)
        {
            item2 = item;
            item2Sprite = item.icon;
            item2Button.GetComponent<Image>().sprite = item.icon;
            item2Button.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("������ ������ ���� á���ϴ�.");
        }
    }

    // ������1 ��ư Ŭ�� �̺�Ʈ
    private void OnItem1Click()
    {
        Debug.Log("������ 1 ���");
        MSKTurnController.Instance.photonView.RPC(nameof(MSKTurnController.RPC_UseItem), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, 0);
        // ������ 1 ��� ���� �߰�
        ClearSlot(1);
    }
    // ������2 ��ư Ŭ�� �̺�Ʈ
    private void OnItem2Click()
    {
        Debug.Log("������ 2 ���");
        MSKTurnController.Instance.photonView.RPC(nameof(MSKTurnController.RPC_UseItem), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, 1);
        // ������ 2 ��� ���� �߰�
        ClearSlot(2);
    }

    public void ClearSlot(int slot)
    {
        if (slot == 1)
        {
            item1Sprite = null;
            item1 = null;
            item1Button.GetComponent<Image>().sprite = null;
            item1Button.gameObject.SetActive(false);
        }
        else if (slot == 2)
        {
            item2Sprite = null;
            item2 = null;
            item2Button.GetComponent<Image>().sprite = null;
            item2Button.gameObject.SetActive(false);
        }
    }

    // �� ���� ��ư Ŭ�� �̺�Ʈ
    private void OnEndTurnClick()
    {
        Debug.Log("�� ����");
        // �� ���� ���� �߰�
    }

    //     // ü�� ������Ʈ
    //     public void SetHP(float current, float max)
    //     {
    //         hpBar.value = current / max;
    //     }
    // 
    //     // �̵� ������ ������Ʈ
    //     public void SetMove(float current, float max)
    //     {
    //         moveBar.value = current / max;
    //     }
    // 
    //     // �Ŀ� ���� ������Ʈ
    //     public void SetPower(float charge)
    //     {
    //         powerBar.value = charge;
    //     }

    // �ٶ� ���� ������Ʈ
    public void SetWind(float windStrength)
    {
        // windStrength�� -10 ~ +10 ������ ����
        windBar.value = Mathf.Abs(windStrength);
    }
}