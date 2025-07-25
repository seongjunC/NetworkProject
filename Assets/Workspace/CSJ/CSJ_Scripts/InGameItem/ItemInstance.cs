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
        if (other.CompareTag("Player"))
        {
            var controller = other.GetComponent<PlayerController>();
            if (controller != null && controller.photonView.IsMine)
            {
                controller.myInfo.ItemAcquire(data);
                Destroy(gameObject);
            }
        }
    }
}
