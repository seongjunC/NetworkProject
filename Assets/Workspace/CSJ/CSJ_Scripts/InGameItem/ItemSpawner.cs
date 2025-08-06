using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ItemSpawner : MonoBehaviourPun
{
    [Header("MapBoundary 참조")]
    [SerializeField] private MapBoundary mapBoundary;

    [Header("낙하산 프리팹")]
    [SerializeField]
    private GameObject RandomBoxPrefab;

    [Header("낙하 관련 설정")]
    [SerializeField] private float fallSpeed = 5f;
    [SerializeField] private float swayAmp = 1f;
    [SerializeField] private float swayFreq = 2f;

    [Header("생성할 아이템 리스트")]
    [SerializeField]
    private ItemDatabase itemDatabase;

    public void SpawnRandomItem()
    {
        ItemData selectedItem = RandItem();

        Vector2 center = mapBoundary.transform.position;
        Vector2 size = mapBoundary.mapSize;

        float halfw = size.x * 0.5f;
        float halfh = size.y * 0.5f;

        float spawnX = Random.Range(center.x - halfw, center.x + halfw);
        float spawnY = center.y + halfh;
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

        GameObject drop = PhotonNetwork.Instantiate(
            "Prefabs/RandomBoxPrefab",
            spawnPos,
            Quaternion.identity
        );

        var pd = drop.GetComponent<PhotonView>();
        pd.RPC("Init",
        RpcTarget.AllBuffered,
        itemDatabase.GetIndex(selectedItem).ToString(), fallSpeed, swayAmp, swayFreq);

    }

    public ItemData RandItem()
    {
        int rate = 0;
        List<int> rateArr = new();
        foreach (ItemData item in itemDatabase.GetAll())
        {
            rate += item.appearRate;
            rateArr.Add(rate);
        }
        int rands = Random.Range(0, rate);

        for (int i = 0; i < rateArr.Count; i++)
        {
            if (rands < rateArr[i])
            {
                return itemDatabase.Get($"{i}");
            }

            else rands -= rateArr[i];
        }
        return itemDatabase.Get("0");
    }


}
