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
            gachaResultSlots[i].gameObject.SetActive(true);
            gachaResultSlots[i].SetUp(gachaResults[i]);

            gachaResultSlots[i].transform.SetSiblingIndex(i);
        }
    }
}
