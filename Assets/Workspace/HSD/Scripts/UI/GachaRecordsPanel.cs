using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaRecordsPanel : MonoBehaviour
{
    [SerializeField] GameObject gachaResultSlot;
    [SerializeField] Transform gachaResultContent;
    [SerializeField] ScrollRect scrollRect;

    [SerializeField] List<GachaResult> gachaResults;
    [SerializeField] List<GachaResultSlot> gachaResultSlots = new();

    private void Start()
    {
        Manager.Firebase.OnLogOut += Clear;
    }

    private void OnEnable()
    {
        Manager.Data.GachaManager.GachaResultsOrderBy();
        gachaResults = Manager.Data.GachaManager.gachaResults;
        scrollRect.normalizedPosition = Vector3.one;
        UpdateAllSlot();
    }

    private void UpdateAllSlot()
    {        
        while(gachaResultSlots.Count < gachaResults.Count)
        {
            GachaResultSlot slot = Instantiate(gachaResultSlot, gachaResultContent).GetComponent<GachaResultSlot>();                        
            gachaResultSlots.Add(slot);
        }

        for (int i = 0; i < gachaResults.Count; i++)
        {
            TankData data = Manager.Data.TankDataController.TankDatas[gachaResults[i].Name];
            gachaResults[i].Name = data.tankMetaName;
            gachaResultSlots[i].gameObject.SetActive(true);
            gachaResultSlots[i].SetUp(gachaResults[i]);

            gachaResultSlots[i].transform.SetSiblingIndex(i);
        }
    }

    private void Clear()
    {
        foreach(var slot in gachaResultSlots)
        {
            Destroy(slot.gameObject);
        }
    }
}
