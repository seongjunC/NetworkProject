using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class ItemSpawner : MonoBehaviourPun
{
    [Header("아이템 생성 지점")]
    [SerializeField]
    private List<Transform> spawnPoints;
    [Header("생성할 아이템 리스트")]
    [SerializeField]
    private List<ItemData> itemList;

    public void SpawnRandomItem()
    {
        ItemData selectedItem = RandItem();
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GameObject item = PhotonNetwork.Instantiate(
            selectedItem.prefab.name,
            spawnPoint.position,
            Quaternion.identity
        );

        item.GetComponent<ItemInstance>().Init(selectedItem);
    }

    public ItemData RandItem()
    {
        int rate = 0;
        List<int> rateArr = new();
        foreach (ItemData item in itemList)
        {
            rate += item.appearRate;
            rateArr.Add(rate);
        }
        int rands = Random.Range(0, rate);

        int i = 0;
        while (rands > 0)
        {
            rands -= rateArr[i];
            i++;
        }
        return itemList[--i];
    }


}
