using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class ItemInstance : MonoBehaviour
{
    private ItemData data;
    public void Init(ItemData _itemData)
    {
        data = _itemData;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log($"▶ itemInstance 충돌 감지: {other.name}");
        if (other.CompareTag("Player"))
        {
            var controller = other.GetComponent<PlayerController>();
            if (controller != null && controller.photonView.IsMine)
            {
                controller.myInfo.ItemAcquire(data);
                Debug.Log("▶ 바로 직후 ItemAcquire 호출");
                Destroy(gameObject);
            }
        }
    }
}
