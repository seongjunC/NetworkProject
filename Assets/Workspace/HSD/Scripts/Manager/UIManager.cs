using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    private Canvas mainCanvas;

    public PopUpUI PopUpUI;
    public PopUpUI_Action PopUpUI_Action;
    public PlayerInfoPanel PlayerInfoPanel;
    public AccountDeletePanel AccountDeletePanel;
    public NickNameSelectPanel NickNameSelectPanel;
    public SettingPanel SettingPanel;
    public FadeScreen FadeScreen;

    #region LifeCycle
    private void Awake()
    {
        CreateMainCanvas();

        PopUpUI = Instantiate(Resources.Load<PopUpUI>("UI/PopupUI"), mainCanvas.transform);
        FadeScreen = Instantiate(Resources.Load<FadeScreen>("UI/FadeScreen"), mainCanvas.transform);
        SettingPanel = Instantiate(Resources.Load<SettingPanel>("UI/SettingPanel"), mainCanvas.transform);
        PopUpUI_Action = Instantiate(Resources.Load<PopUpUI_Action>("UI/PopUpUI_Action"), mainCanvas.transform);
        PlayerInfoPanel = Instantiate(Resources.Load<PlayerInfoPanel>("UI/PlayerInfoPanel"), mainCanvas.transform);
        AccountDeletePanel = Instantiate(Resources.Load<AccountDeletePanel>("UI/AccountDeletePanel"), mainCanvas.transform);
        NickNameSelectPanel = Instantiate(Resources.Load<NickNameSelectPanel>("UI/NickNameSelectPanel"), mainCanvas.transform);
    }
    #endregion

    private void CreateMainCanvas()
    {
        mainCanvas = new GameObject("MainCanvas").AddComponent<Canvas>();

        mainCanvas.pixelPerfect = true;
        mainCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        mainCanvas.sortingOrder = 5;

        CanvasScaler scaler = mainCanvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        mainCanvas.AddComponent<GraphicRaycaster>();

        mainCanvas.transform.SetParent(transform, false);
    }
}
