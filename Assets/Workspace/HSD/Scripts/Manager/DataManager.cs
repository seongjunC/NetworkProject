using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    public static Sprite currentLoadingImage;
    public static Sprite loadingImage 
    {   
        get 
        { 
            currentLoadingImage = GetRamdomLoadingImage();

            return currentLoadingImage;
        } 
    }
    private static Sprite[] loadingSprite;

    public PlayerData PlayerData;
    public TankInventoryData TankInventoryData;    
    
    public TankDataController TankDataController = new();
    public GachaManager GachaManager = new();
    public SaveManager saveManager;

    private void Start()
    {
        loadingSprite = Resources.LoadAll<Sprite>("Loading");
        saveManager = gameObject.AddComponent<SaveManager>();
        Manager.Firebase.OnLogOut += () => TankDataController.TankDatas.Clear();
    }

    public void Init()
    {
        TankInventoryData = new();
        TankDataController.Init();
        TankInventoryData.OnTankCountUpdated += TankDataController.UpdateCount;
        TankInventoryData.Init();
        GachaManager.Init();
    }

    private static Sprite GetRamdomLoadingImage()
    {
        return loadingSprite[Random.Range(0, loadingSprite.Length)];
    }
}
