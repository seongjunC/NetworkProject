using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.UI;

public class PromotionUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI successRateText;
    [SerializeField] private Button promotionButton;
    [SerializeField] private Button outButton;

    [Header("������ �г�")]
    [SerializeField] private GameObject tankPanel;

    [SerializeField] private Transform tankGridParent;
    [SerializeField] private GameObject tankButtonPrefab;
    [SerializeField] private Image predictedTankImage;

    //private List<TankData> ownedTanks; // ���� ��ũ ����

    //private List<TankData> selectedTanks = new List<TankData>();
    //[SerializeField] private TankSlot[] slots;
    //private void OnEnable()
    //{
    //    selectedTanks.Clear();
    //    RefreshTankSlots();

    //    foreach (Transform child in tankGridParent)
    //        Destroy(child.gameObject);

    //    InitTankList();
    //}
    void Start()
    {
        promotionButton.onClick.AddListener(OnClickPromotion);
        outButton.onClick.AddListener(OnClickOut);
    }
    //void InitTankList() // ���� ��ũ ��� �ʱ�ȭ
    //{
    //    ownedTanks = TankManager.Instance.GetOwnedTanks(); // TankManager���� ���� ��ũ ���� ��������
    //    if (ownedTanks.Count == 0)
    //    {
    //        Debug.LogWarning("���� ��ũ�� �����ϴ�.");
    //        return;
    //    }
    //    PopulateTankButtons();
    //}
    //void PopulateTankButtons() // ��ũ ��ư ����
    //{
    //    foreach (var tank in ownedTanks)
    //    {
    //        var btn = Instantiate(tankButtonPrefab, tankGridParent);
    //        btn.GetComponent<TankButton>().Setup(tank, OnTankSelected);
    //    }
    //}
    //void OnTankSelected(TankData tank) //��ũ ���� �� ���Կ� �߰�
    //{
    //    if (selectedTanks.Count >= 3 || selectedTanks.Contains(tank))
    //        return;

    //    if (selectedTanks.Count > 0 && selectedTanks[0].id != tank.id)
    //        return;

    //    slots[selectedTanks.Count].SetTank(tank);
    //    selectedTanks.Add(tank);

    //    UpdateTankListUI(tank.id); // ���͸� ����

    //    if (selectedTanks.Count == 3)
    //        PredictUpgrade(); // �±� ����
    //}
    //void UpdateTankListUI(string selectedTankId) // ���õ� ��ũ ID�� ���� ��ư Ȱ��ȭ/��Ȱ��ȭ
    //{
    //    foreach (Transform t in tankGridParent)
    //    {
    //        var button = t.GetComponent<TankButton>();
    //        bool isSame = button.Tank.id == selectedTankId;
    //        button.SetInteractable(isSame || selectedTanks.Count == 0);
    //    }
    //}
    //void PredictUpgrade() // �±� ����
    //{
    //    var baseTank = selectedTanks[0];
    //    var resultTank = TankDatabase.GetNextGrade(baseTank);

    //    if (resultTank == null)
    //    {
    //        predictedTankImage.sprite = null;
    //        successRateText.text = "0%";
    //        return;
    //    }

    //    predictedTankImage.sprite = resultTank.icon;
    //    successRateText.text = CalcSuccessRate(selectedTanks).ToString("F0") + "%";
    //}
    //float CalcSuccessRate(List<TankData> tanks) // �±� ������ ���
    //{
    //    if (tanks.Count == 0) return 0f;
    //    float rate = 0f;
    //    foreach (var tank in tanks)
    //    {
    //        rate += tank.successRate; // �� ��ũ�� ������ �ջ�
    //    }
    //    return rate / tanks.Count; // ��� ������
    //}
    //public void RemoveTank(int index) // ���Կ��� ��ũ ���� �� �ٽ� ���� �ʱ�ȭ
    //{
    //    if (index >= selectedTanks.Count) return;

    //    selectedTanks.RemoveAt(index);
    //    slots[index].Clear();

    //    RefreshTankSlots();
    //    UpdateTankListUI(selectedTanks.Count > 0 ? selectedTanks[0].id : "");
    //}
    //public void RefreshTankSlots() // ���� ����
    //{
    //    for (int i = 0; i < slots.Length; i++)
    //    {
    //        if (i < selectedTanks.Count)
    //        {
    //            slots[i].SetTank(selectedTanks[i]);
    //        }
    //        else
    //        {
    //            slots[i].Clear();
    //        }
    //    }
    //}
    private void OnClickPromotion()
    {
        //if (selectedTanks.Count != 3)
        //{
        //    Debug.LogWarning("��ũ�� 3�� �����ؾ� �±��� �����մϴ�.");
        //    return;
        //}

        //float successRate = CalcSuccessRate(selectedTanks);
        //float roll = Random.Range(0f, 100f);

        //if (roll <= successRate)
        //{
        //    // ����: ���ο� ��ũ ȹ��
        //    var resultTank = TankDatabase.GetNextGrade(selectedTanks[0]);
        //    if (resultTank != null)
        //    {
        //        TankManager.Instance.AddTank(resultTank);
        //        Manager.UI.PopUpUI.Show("�±� ����!", () =>
        //        {
        //            ClearPromotionUI();
        //        });
        //    }
        //}
        //else
        //{
        //    // ����
        //    Manager.UI.PopUpUI.Show("�±� ����!", () =>
        //    {
        //        ClearPromotionUI();
        //    });
        //}

        //// ��� ��ũ ����
        //foreach (var tank in selectedTanks)
        //{
        //    TankManager.Instance.RemoveTank(tank);
        //}
    }
    //void ClearPromotionUI()
    //{
    //    selectedTanks.Clear();
    //    RefreshTankSlots();
    //    predictedTankImage.sprite = null;
    //    successRateText.text = "";

    //    foreach (Transform t in tankGridParent)
    //    {
    //        var button = t.GetComponent<TankButton>();
    //        button.SetInteractable(true);
    //    }

    //    // TankGrid �ٽ� ���� ���� (���õ� ��ũ�� ���ŵ��� �� �����Ƿ�)
    //    foreach (Transform child in tankGridParent)
    //        Destroy(child.gameObject);

    //    InitTankList();
    //}
    private void OnClickOut()
    {
        gameObject.SetActive(false);         // ���θ�� �г� �ݱ�
        tankPanel.SetActive(true);       // ��ũ �г� ����
    }

}
