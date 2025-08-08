using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

[Serializable]
public class PlayerInfo
{
    public Player player;
    public string NickName => player.NickName;
    public int ActorNumber => player.ActorNumber;
    public ItemData[] items = new ItemData[2];
    public float damageDealt { get; private set; }
    public int KillCount { get; private set; }
    public Action<ItemData> OnItemAcquired;
    public Team team;

    public PlayerInfo(Player _player)
    {
        player = _player;
        damageDealt = 0;
        KillCount = 0;
        team = CustomProperty.GetTeam(_player);
    }

    public bool ItemAcquire(ItemData item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                Debug.Log($"▶ ItemAcquire 성공: {item.name} / 슬롯 {i}");
                OnItemAcquired?.Invoke(item);
                return true;
            }
        }
        return false;

        #region 기획 수정으로 미사용
        //         // 인게임 UI에서 Item을 선택받아 오기
        //         ItemData removeItem;
        //         // removeItem = 
        //         // 임시 지정
        //         removeItem = new ItemData();
        // 
        //         int itemNum = ItemRemove(removeItem);
        //         if (itemNum < items.Length)
        //         {
        //             items[itemNum] = item;
        //         }
        #endregion
    }

    public int ItemRemove(ItemData removeItem)
    {
        // TODO : 삭제할 item 위치에 따라 다른 값을 리턴 
        // UI와 연계
        // 코루틴을 사용해서 선택을 받을때 까지 대기
        // 삭제할 아이템을 선택받으면 해당 아이템을 전달

        if (items.Contains(removeItem))
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == removeItem)
                {
                    items[i] = null;
                    return i + 1;
                }
            }
        }

        return items.Length;
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
        if (items[order] == null)
        {
            Debug.Log($"{player.NickName} : 해당 위치 {order + 1} 칸에 아이템이 없습니다.");
            return;
        }
        items[order].UseItem();
        ItemRemove(order);
    }

    public void ItemUse(ItemData item)
    {
        int index;
        if ((index = GetItemIndex(item)) != -99)
            ItemUse(index);
        else
        {
            Debug.Log("해당 아이템이 없습니다");
        }
    }
    public int GetItemIndex(ItemData item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == item) return i;
        }
        return -99;
    }

    public bool Isfull()
    {
        foreach (var h in items)
        {
            if (h == null) return false;
        }
        return true;
    }

    public void ToDealDamage(float amount)
    {
        damageDealt += amount;
    }

    public void RecordKillCount()
    {
        KillCount++;
        Debug.Log($"{NickName}이 플레이어를 사망에 이르게 하였습니다. \n 킬 스코어 : {KillCount}");
    }
}
