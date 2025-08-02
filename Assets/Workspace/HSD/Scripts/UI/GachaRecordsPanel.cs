using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaRecordsPanel : MonoBehaviour
{
    [SerializeField] GameObject gachaResultSlot;
    [SerializeField] Transform gachaResultContent;

    [SerializeField] List<GachaResult> gachaResults;
    [SerializeField] List<GachaResultSlot> gachaResultSlots = new();

    private void Awake()
    {
        gachaResults = Manager.Data.GachaManager.gachaResults;
    }

    private void OnEnable()
    {
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
