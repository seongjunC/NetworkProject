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

    [Header("연결할 패널")]
    [SerializeField] private GameObject tankPanel;

    [SerializeField] private Transform tankGridParent;
    [SerializeField] private GameObject tankButtonPrefab;
    [SerializeField] private Image predictedTankImage;

    //private List<TankData> ownedTanks; // 보유 탱크 정보

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
    //void InitTankList() // 보유 탱크 목록 초기화
    //{
    //    ownedTanks = TankManager.Instance.GetOwnedTanks(); // TankManager에서 보유 탱크 정보 가져오기
    //    if (ownedTanks.Count == 0)
    //    {
    //        Debug.LogWarning("보유 탱크가 없습니다.");
    //        return;
    //    }
    //    PopulateTankButtons();
    //}
    //void PopulateTankButtons() // 탱크 버튼 생성
    //{
    //    foreach (var tank in ownedTanks)
    //    {
    //        var btn = Instantiate(tankButtonPrefab, tankGridParent);
    //        btn.GetComponent<TankButton>().Setup(tank, OnTankSelected);
    //    }
    //}
    //void OnTankSelected(TankData tank) //탱크 선택 시 슬롯에 추가
    //{
    //    if (selectedTanks.Count >= 3 || selectedTanks.Contains(tank))
    //        return;

    //    if (selectedTanks.Count > 0 && selectedTanks[0].id != tank.id)
    //        return;

    //    slots[selectedTanks.Count].SetTank(tank);
    //    selectedTanks.Add(tank);

    //    UpdateTankListUI(tank.id); // 필터링 적용

    //    if (selectedTanks.Count == 3)
    //        PredictUpgrade(); // 승급 예측
    //}
    //void UpdateTankListUI(string selectedTankId) // 선택된 탱크 ID에 따라 버튼 활성화/비활성화
    //{
    //    foreach (Transform t in tankGridParent)
    //    {
    //        var button = t.GetComponent<TankButton>();
    //        bool isSame = button.Tank.id == selectedTankId;
    //        button.SetInteractable(isSame || selectedTanks.Count == 0);
    //    }
    //}
    //void PredictUpgrade() // 승급 예측
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
    //float CalcSuccessRate(List<TankData> tanks) // 승급 성공률 계산
    //{
    //    if (tanks.Count == 0) return 0f;
    //    float rate = 0f;
    //    foreach (var tank in tanks)
    //    {
    //        rate += tank.successRate; // 각 탱크의 성공률 합산
    //    }
    //    return rate / tanks.Count; // 평균 성공률
    //}
    //public void RemoveTank(int index) // 슬롯에서 탱크 제거 시 다시 필터 초기화
    //{
    //    if (index >= selectedTanks.Count) return;

    //    selectedTanks.RemoveAt(index);
    //    slots[index].Clear();

    //    RefreshTankSlots();
    //    UpdateTankListUI(selectedTanks.Count > 0 ? selectedTanks[0].id : "");
    //}
    //public void RefreshTankSlots() // 슬롯 갱신
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
        //    Debug.LogWarning("탱크를 3개 선택해야 승급이 가능합니다.");
        //    return;
        //}

        //float successRate = CalcSuccessRate(selectedTanks);
        //float roll = Random.Range(0f, 100f);

        //if (roll <= successRate)
        //{
        //    // 성공: 새로운 탱크 획득
        //    var resultTank = TankDatabase.GetNextGrade(selectedTanks[0]);
        //    if (resultTank != null)
        //    {
        //        TankManager.Instance.AddTank(resultTank);
        //        Manager.UI.PopUpUI.Show("승급 성공!", () =>
        //        {
        //            ClearPromotionUI();
        //        });
        //    }
        //}
        //else
        //{
        //    // 실패
        //    Manager.UI.PopUpUI.Show("승급 실패!", () =>
        //    {
        //        ClearPromotionUI();
        //    });
        //}

        //// 재료 탱크 제거
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

    //    // TankGrid 다시 새로 구성 (선택된 탱크가 제거됐을 수 있으므로)
    //    foreach (Transform child in tankGridParent)
    //        Destroy(child.gameObject);

    //    InitTankList();
    //}
    private void OnClickOut()
    {
        gameObject.SetActive(false);         // 프로모션 패널 닫기
        tankPanel.SetActive(true);       // 탱크 패널 열기
    }

}
