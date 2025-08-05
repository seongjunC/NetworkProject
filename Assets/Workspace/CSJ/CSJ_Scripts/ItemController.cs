using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemController
{
    public static void useItem(ItemData item, PlayerInfo playerInfo)
    {
        playerInfo.ItemUse(item);
    }
}
