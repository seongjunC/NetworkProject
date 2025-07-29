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
    public Slider windBar;         // ��/�� �ٶ� ����
    public RectTransform flagImage; // ��� ����� �̹���

    [Header("Button")]
    public Button item1Button;     // ������1 ��ư
    public Button item2Button;     // ������2 ��ư
    public Button endTurnButton;

    [Header("Item Icons")]
    [SerializeField] private Image item1Icon;
    [SerializeField] private Image item2Icon;

    // ������ ���Կ� ������ ������ ����
    private Sprite item1Sprite = null;
    private Sprite item2Sprite = null;

    [Header("Text")]
    public TextMeshProUGUI healthPointText; // ü�� �ؽ�Ʈ

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
            Debug.Log("������ ������ ���� á���ϴ�.");
        }
    }

    // ������1 ��ư Ŭ�� �̺�Ʈ
    private void OnItem1Click()
    {
        Debug.Log("������ 1 ���");
        // ������ 1 ��� ���� �߰�
        ClearSlot(1);
    }
    // ������2 ��ư Ŭ�� �̺�Ʈ
    private void OnItem2Click()
    {
        Debug.Log("������ 2 ���");
        // ������ 2 ��� ���� �߰�
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

    // �� ���� ��ư Ŭ�� �̺�Ʈ
    private void OnEndTurnClick()
    {
        Debug.Log("�� ����");
        // �� ���� ���� �߰�
    }

    // ü�� ������Ʈ
    public void SetHP(float current, float max)
    {
        hpBar.value = current / max;
    }

    // �̵� ������ ������Ʈ
    public void SetMove(float current, float max)
    {
        moveBar.value = current / max;
    }

    // �Ŀ� ���� ������Ʈ
    public void SetPower(float charge)
    {
        powerBar.value = charge;
    }

    // �ٶ� ���� ������Ʈ
    public void SetWind(float windStrength)
    {
        // windStrength�� -10 ~ +10 ������ ����
        windBar.value = Mathf.Abs(windStrength);
    }
}
