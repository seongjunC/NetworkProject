using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

public class PlayerInfo
{
    public Player player;
    public string NickName => player.NickName;
    public bool isDead;
    public int ActorNumber;
    public ItemData[] items = new ItemData[2];

    public PlayerInfo(Player _player)
    {
        player = _player;
        ActorNumber = _player.ActorNumber;
        isDead = false;
    }

    public void ItemAcquire(ItemData item)
    {
        int length = items.Length;
        for (int i = 0; i < length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                return;
            }
        }

        items[items.Length] = item;
        int itemNum = ItemRemove();
        if (itemNum == 0 || itemNum == 1)
        {
            items[itemNum] = item;
        }
    }
    public int ItemRemove()
    {
        // TODO : 삭제할 item 위치에 따라 다른 값을 리턴 
        return 2;
    }

    public void ItemRemove(int order)
    {
        if (order < 0 || order >= items.Length)
        {
            Debug.LogError("잘못된 슬롯 번호입니다.");
            return;
        }
        for (int i = order; i < items.Length - 1; i++)
        {
            items[i] = items[i + 1];
        }
        items[items.Length - 1] = null;
    }

    public void ItemUse(int order)
    {
        if (order < 0 || order >= items.Length)
        {
            Debug.LogError("잘못된 슬롯 번호입니다.");
            return;
        }
        if (items[order] = null)
        {
            Debug.LogError($"해당 위치 {order + 1} 칸에 아이템이 없습니다.");
            return;
        }
        items[order].UseItem();
        ItemRemove(order);

    }
}
