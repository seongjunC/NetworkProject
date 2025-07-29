using System.Collections;
using System.Collections.Generic;
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

    [Header("Item Icons")]
    [SerializeField] private Image item1Icon;
    [SerializeField] private Image item2Icon;

    // 아이템 슬롯에 아이템 데이터 저장
    private Sprite item1Sprite = null;
    private Sprite item2Sprite = null;

    [Header("Text")]
    public TextMeshProUGUI healthPointText; // 체력 텍스트

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
        // 버튼 클릭 이벤트 설정
        item1Button.onClick.AddListener(OnItem1Click);
        item2Button.onClick.AddListener(OnItem2Click);
        endTurnButton.onClick.AddListener(OnEndTurnClick);
    }

    public void AddItem(Sprite itemIcon)
    {
        if (item1Sprite == null)
        {
            item1Sprite = itemIcon;
            item1Icon.sprite = itemIcon;
            item1Button.gameObject.SetActive(true);
        }
        else if (item2Sprite == null)
        {
            item2Sprite = itemIcon;
            item2Icon.sprite = itemIcon;
            item2Button.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("아이템 슬롯이 가득 찼습니다.");
        }
    }

    // 아이템1 버튼 클릭 이벤트
    private void OnItem1Click()
    {
        Debug.Log("아이템 1 사용");
        // 아이템 1 사용 로직 추가
        ClearSlot(1);
    }
    // 아이템2 버튼 클릭 이벤트
    private void OnItem2Click()
    {
        Debug.Log("아이템 2 사용");
        // 아이템 2 사용 로직 추가
        ClearSlot(2);
    }

    private void ClearSlot(int slot)
    {
        if (slot == 1)
        {
            item1Sprite = null;
            item1Button.gameObject.SetActive(false);
        }
        else if (slot == 2)
        {
            item2Sprite = null;
            item2Button.gameObject.SetActive(false);
        }
    }

    // 턴 종료 버튼 클릭 이벤트
    private void OnEndTurnClick()
    {
        Debug.Log("턴 종료");
        // 턴 종료 로직 추가
    }

    // 체력 업데이트
    public void SetHP(float current, float max)
    {
        hpBar.value = current / max;
    }

    // 이동 게이지 업데이트
    public void SetMove(float current, float max)
    {
        moveBar.value = current / max;
    }

    // 파워 차지 업데이트
    public void SetPower(float charge)
    {
        powerBar.value = charge;
    }

    // 바람 세기 업데이트
    public void SetWind(float windStrength)
    {
        // windStrength는 -10 ~ +10 범위로 가정
        windBar.value = Mathf.Abs(windStrength);
    }
}
