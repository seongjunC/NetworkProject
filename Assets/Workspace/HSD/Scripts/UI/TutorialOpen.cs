using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialOpen : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] GameObject tutorial;

    private void OnEnable()
    {
        button.onClick.AddListener(TutorialPanelOpen);
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }

    private void TutorialPanelOpen() => tutorial.SetActive(true);

}
