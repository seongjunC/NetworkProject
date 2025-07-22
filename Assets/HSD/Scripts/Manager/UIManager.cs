using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Canvas mainCanvas;
    public PopupUI PopupUI;

    #region LifeCycle
    private void Awake()
    {
        CreateMainCanvas();

        PopupUI = Instantiate(Resources.Load<PopupUI>("UI/PopupUI"), mainCanvas.transform);
    }
    #endregion

    private void CreateMainCanvas()
    {
        mainCanvas = new GameObject("MainCanvas").AddComponent<Canvas>();
        mainCanvas.pixelPerfect = true;
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler         = mainCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode          = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution  = new Vector2(1920, 1080);

        mainCanvas.AddComponent<GraphicRaycaster>();

        mainCanvas.transform.SetParent(transform, false);
    }
}
