using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class CouponRewordPanel : MonoBehaviour
{
    [SerializeField] GameObject couponRewordPrefab;
    [SerializeField] Transform content;
    [SerializeField] Button checkButton;

    private Dictionary<string, int> resultDic;
    private List<GameObject> slots = new List<GameObject>();
    [SerializeField] Sprite gemIcon;

    private YieldInstruction delay = new WaitForSeconds(.15f);

    private void Start()
    {
        checkButton.onClick.AddListener(() => gameObject.SetActive(false));
    }

    public void CreateSlot(Dictionary<string, int> resultDic)
    {
        checkButton.gameObject.SetActive(false);
        gameObject.SetActive(true);

        SlotClear();
        this.resultDic = resultDic;
        StartCoroutine(CreateRoutine());
    }

    private IEnumerator CreateRoutine()
    {
        Debug.Log("Start");
        foreach (var kvp in resultDic)
        {
            GameObject obj = Instantiate(couponRewordPrefab, content);

            if(kvp.Key == "Gem")
            {
                obj.GetComponent<RewordSlot>().SetUp(gemIcon, "Ал", kvp.Value);
            }
            else
            {
                TankData data = Manager.Data.TankDataController.TankDatas[kvp.Key];
                obj.GetComponent<RewordSlot>().SetUp(data.Icon, data.tankName, kvp.Value);
            }

            slots.Add(obj);

            Debug.Log("Re");
            yield return delay;
        }

        checkButton.gameObject.SetActive(true);
    }

    private void SlotClear()
    {
        foreach(GameObject obj in slots)
        {
            Destroy(obj);
        }

        slots.Clear();
    }
}
