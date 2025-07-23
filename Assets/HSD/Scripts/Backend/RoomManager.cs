using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] GameObject playerSlotPrefab;
    [SerializeField] Transform playerContent;

    [Header("Map")]
    [SerializeField] GameObject mapPrefab;
    [SerializeField] GameObject mapSelectPanel;
    [SerializeField] List<string> maps;
    [SerializeField] Button changeButton;

    [Header("Game")]
    [SerializeField] Button readyButton;
    [SerializeField] Button startButton;
    [SerializeField] TMP_Text readyCount;
}
