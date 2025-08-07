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
    public Slider windBar;         // 좌/우 바람 세기
    public RectTransform flagImage; // 깃발 방향용 이미지

    [Header("Button")]
    public Button item1Button;     // 아이템1 버튼
    public Button item2Button;     // 아이템2 버튼
    public Button endTurnButton;

    [Header("ButtonImage")]
    public Image Button1Image;
    public Image Button2Image;

    // 아이템 슬롯에 아이템 데이터 저장
    private Sprite item1Sprite = null;
    private Sprite item2Sprite = null;
    private ItemData item1 = null;
    private ItemData item2 = null;

    [Header("Text")]
    public TextMeshProUGUI healthPointText; // 체력 텍스트


    private PlayerController _player;
    private Fire _fire;


    void Start()
    {
        // 초기화
        hpBar.value = 1500f;          // 체력 게이지 초기화
        hpBar.maxValue = 1500f;      // 체력 게이지 최대값 설정
        healthPointText.text = "1500/1500";  // 체력 텍스트 초기화
        moveBar.value = 100f;        // 이동 게이지 초기화
        powerBar.value = 0f;       // 파워 차지 초기화
        windBar.value = 0f;        // 바람 세기 초기화
        item1Button.gameObject.SetActive(false);
        item2Button.gameObject.SetActive(false);
        endTurnButton.onClick.AddListener(OnEndTurnClick);
        Debug.Log($"AddListener 등록: {item1Button.gameObject.name}");
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
        Debug.Log("IngameUI 등록 완료");
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

        Debug.Log($"▶ InGameUI.AddItem 호출: {item.name}");
        if (item1Sprite == null)
        {
            Debug.Log("item1 실행");
            item1 = item;
            Button1Image.sprite = item.icon;
            item1Button.image.sprite = Button1Image.sprite;
            item1Button.gameObject.SetActive(true);
            item1Button.onClick.RemoveAllListeners();
            item1Button.onClick.AddListener(() => OnClickSlot(0));

            item1Button.interactable = true;
            item1Button.gameObject.SetActive(true);

            Debug.Log($"▶ 슬롯 1에 아이템 {item.name} 세팅 완료");
        }
        else if (item2Sprite == null)
        {
            Debug.Log("item2 실행");
            item2 = item;
            Button2Image.sprite = item.icon;
            item2Button.gameObject.SetActive(true);
            item2Button.onClick.RemoveAllListeners();
            item2Button.onClick.AddListener(() => OnClickSlot(1));

            item2Button.interactable = true;
            item2Button.gameObject.SetActive(true);

            Debug.Log($"▶ 슬롯 2에 아이템 {item.name} 세팅 완료");
        }
        else
        {
            Debug.Log("아이템 슬롯이 가득 찼습니다.");
        }
    }
    void OnClickSlot(int slot)
    {
        Button btn = slot == 0 ? item1Button : item2Button;
        Debug.Log($"▶ 슬롯 {slot} 클릭! 아이템: {btn}");
        if (btn != null)
        {
            MSKTurnController.Instance.photonView.RPC(
                nameof(MSKTurnController.RPC_UseItem),
                RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber,
                slot);
            ClearSlot(slot);
        }
    }

    // // 아이템1 버튼 클릭 이벤트
    // private void OnItem1Click()
    // {
    //     Debug.Log("아이템 1 사용");
    //     MSKTurnController.Instance.photonView.RPC(nameof(MSKTurnController.RPC_UseItem), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, 0);
    //     // 아이템 1 사용 로직 추가
    //     ClearSlot(1);
    // }
    // // 아이템2 버튼 클릭 이벤트
    // private void OnItem2Click()
    // {
    //     Debug.Log("아이템 2 사용");
    //     MSKTurnController.Instance.photonView.RPC(nameof(MSKTurnController.RPC_UseItem), RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, 1);
    //     // 아이템 2 사용 로직 추가
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

    // 턴 종료 버튼 클릭 이벤트
    private void OnEndTurnClick()
    {
        Debug.Log("턴 종료");
        // 턴 종료 로직 추가
    }

    //     // 체력 업데이트
    //     public void SetHP(float current, float max)
    //     {
    //         hpBar.value = current / max;
    //     }
    // 
    //     // 이동 게이지 업데이트
    //     public void SetMove(float current, float max)
    //     {
    //         moveBar.value = current / max;
    //     }
    // 
    //     // 파워 차지 업데이트
    //     public void SetPower(float charge)
    //     {
    //         powerBar.value = charge;
    //     }

    // 바람 세기 업데이트
    public void SetWind(float windStrength)
    {
        // windStrength는 -10 ~ +10 범위로 가정
        windBar.value = Mathf.Abs(windStrength);
    }
}