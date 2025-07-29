using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public PlayerData PlayerData;
    public InventoryData InventoryData;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
            InventoryData.AddTank("A_Tank",1,1);
    }

}
