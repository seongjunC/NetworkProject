using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private FileHandler handler;
    [SerializeField] private GameData gameData;
    private List<ISavable> savables;

    [SerializeField] private string fileName = "HSD.json";

    private void Start()
    {
        handler = new FileHandler(Application.persistentDataPath, fileName);
        savables = FindSavables();

        LoadGame();
    }

    public void SaveGame()
    {
        foreach (var savable in savables)
        {
            savable.Save(ref gameData);
        }

        handler.SaveData(gameData);
    }

    public void LoadGame()
    {
        gameData = handler.LoadData();

        if (gameData == null)
            gameData = new GameData();

        foreach (var savable in savables)
        {
            savable.Load(gameData);
        }
    }

    [ContextMenu("=== Delete Save Data ===")]
    public void DeleteFile()
    {
        handler = new FileHandler(Application.persistentDataPath, fileName);
        handler.DeleteFile();
    }

    private List<ISavable> FindSavables()
        => FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .OfType<ISavable>().ToList();

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
