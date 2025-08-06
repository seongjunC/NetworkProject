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

    [Header("ButtonImage")]
    public Image Button1Image;
    public Image Button2Image;

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
        endTurnButton.onClick.AddListener(OnEndTurnClick);
        Debug.Log($"AddListener ���: {item1Button.gameObject.name}");
    }


    public void RegisterPlayer(PlayerController playerController)
    {
        _player = playerController;
        _fire = _player.GetComponentInChildren<Fire>();

        if (_fire != null)
            powerBar.maxValue = _fire.maxPower;

        hpBar.maxValue = _player._hp;
        moveBar.maxValue = _player._movable;
        playerController.myInfo.OnItemAcquired -= AddItem;
        playerController.myInfo.OnItemAcquired += AddItem;
        foreach (var item in playerController.myInfo.items)
            if (item != null) AddItem(item);
        Debug.Log("IngameUI ��� �Ϸ�");
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

        Debug.Log($"�� InGameUI.AddItem ȣ��: {item.name}");
        if (item1Sprite == null)
        {
            Debug.Log("item1 ����");
            item1 = item;
            Button1Image.sprite = item.icon;
            item1Button.image.sprite = Button1Image.sprite;
            item1Button.gameObject.SetActive(true);
            item1Button.onClick.RemoveAllListeners();
            item1Button.onClick.AddListener(() => OnClickSlot(0));

            item1Button.interactable = true;
            item1Button.gameObject.SetActive(true);

            Debug.Log($"�� ���� 1�� ������ {item.name} ���� �Ϸ�");
        }
        else if (item2Sprite == null)
        {
            Debug.Log("item2 ����");
            item2 = item;
            Button2Image.sprite = item.icon;
            item2Button.gameObject.SetActive(true);
            item2Button.onClick.RemoveAllListeners();
            item2Button.onClick.AddListener(() => OnClickSlot(1));

            item2Button.interactable = true;
            item2Button.gameObject.SetActive(true);

            Debug.Log($"�� ���� 2�� ������ {item.name} ���� �Ϸ�");
        }
        else
        {
            Debug.Log("������ ������ ���� á���ϴ�.");
        }
    }
    void OnClickSlot(int slot)
    {
        Button btn = slot == 0 ? item1Button : item2Button;
        Debug.Log($"�� ���� {slot} Ŭ��! ������: {btn}");
        if (btn != null)
        {
            MSKTurnController.Instance.photonView.RPC(
                nameof(MSKTurnController.RPC_UseItem),
                RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber,
                slot);
            ClearSlot(slot);
        }
    }

    // // ������1 ��ư Ŭ�� �̺�Ʈ
    // private void OnItem1Click()
    // {
    //     Debug.Log("������ 1 ���");
    //     MSKTurnController.Instance.photonView.RPC(nameof(MSKTurnController.RPC_UseItem), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, 0);
    //     // ������ 1 ��� ���� �߰�
    //     ClearSlot(1);
    // }
    // // ������2 ��ư Ŭ�� �̺�Ʈ
    // private void OnItem2Click()
    // {
    //     Debug.Log("������ 2 ���");
    //     MSKTurnController.Instance.photonView.RPC(nameof(MSKTurnController.RPC_UseItem), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, 1);
    //     // ������ 2 ��� ���� �߰�
    //     ClearSlot(2);
    // }

    public void ClearSlot(int slot)
    {
        if (slot == 0)
        {
            item1Sprite = null;
            item1 = null;
            item1Button.GetComponent<Image>().sprite = null;
            item1Button.gameObject.SetActive(false);
        }
        else if (slot == 1)
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