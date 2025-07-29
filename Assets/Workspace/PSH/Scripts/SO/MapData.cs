using UnityEngine;

[CreateAssetMenu(menuName = "MapData")]
public class MapData : ScriptableObject
{
    public string mapName;
    public GameObject mapPrefab;
    public GameObject spawnPointPrefab;
}