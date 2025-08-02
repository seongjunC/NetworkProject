using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TankInventoryUI : MonoBehaviour
{
    [SerializeField] GameObject tankSlotPrefab;
    [SerializeField] Transform slotContent;
    [SerializeField] TankToolTip tankToolTip;
    [SerializeField] TankData[] datas;

    [Header("Button")]
    [SerializeField] Button exitButton;

    [Header("Color")]
    [SerializeField] Color sColor;
    [SerializeField] Color aColor;
    [SerializeField] Color bColor;
    [SerializeField] Color cColor;

    private Dictionary<TankData, TankSlot> tankSlotDic = new Dictionary<TankData, TankSlot>();

    private void Awake()
    {
        datas = Manager.Data.TankDataController.TankDatas.Values.ToArray();
    }

    private void Start()
    {
        exitButton.onClick.AddListener(() => gameObject.SetActive(false));        
        Manager.Firebase.OnLogOut += ClearSlots;
    }

    private void OnEnable()
    {
        UpdateAllSlot();
        Manager.Data.TankDataController.OnTankDataChanged += UpdateSlot;
    }

    private void OnDisable()
    {
        Manager.Data.TankDataController.OnTankDataChanged -= UpdateSlot;
    }

    private void UpdateAllSlot()
    {        
        foreach (var data in datas)
        {
            if(!tankSlotDic.ContainsKey(data))
            {
                if (data.Count == 0)
                    continue;

                TankSlot slot = Instantiate(tankSlotPrefab, slotContent).GetComponent<TankSlot>();
                slot.SetUp(data, tankToolTip, Utils.GetColor(data.rank));
                tankSlotDic[data] = slot;
            }
            else
            {
                if (data.Count == 0)
                {
                    Destroy(tankSlotDic[data].gameObject);
                    tankSlotDic.Remove(data);
                }
            }
        }
    }

    private void UpdateSlot(TankData data)
    {
        if (!tankSlotDic.ContainsKey(data))
        {
            if (data.Count == 0)
                return;

            TankSlot slot = Instantiate(tankSlotPrefab, slotContent).GetComponent<TankSlot>();
            slot.SetUp(data, tankToolTip, Utils.GetColor(data.rank));
            tankSlotDic[data] = slot;
        }
        else
        {
            if (data.Count == 0)
            {
                Destroy(tankSlotDic[data].gameObject);
                tankSlotDic.Remove(data);
            }
        }
    }    

    private void ClearSlots()
    {
        foreach (var kvp in tankSlotDic)
        {
            Destroy(kvp.Value.gameObject);
        }
        tankSlotDic.Clear();
    }
}
