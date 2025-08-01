using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GachaPanel : MonoBehaviour
{
    [SerializeField] Gacha gacha;
    [SerializeField] Button gachaExitButton;

    [Header("GachaButtons")]
    [SerializeField] Button oneGachaButton;
    [SerializeField] Button tenGachaButton;

    [Header("SideButtons")]
    [SerializeField] Button exitButton;
    [SerializeField] Button componentButton;
    [SerializeField] Button componentExitButton;
    [SerializeField] Button gachaRecordButton;
    [SerializeField] Button gachaRecordExitButton;

    [Header("Panel")]
    [SerializeField] GameObject componentPanel;
    [SerializeField] GameObject gachaRecordPanel;
    [SerializeField] GameObject lobby;

    [Header("GachaResult")]
    [SerializeField] GameObject gachaResultPopUp;
    [SerializeField] Transform gachaResultContent;
    private void OnEnable()
    {
        gachaExitButton.onClick.AddListener(GachaExit);
        oneGachaButton.onClick.AddListener(OneGacha);
        tenGachaButton.onClick.AddListener(TenGacha);

        exitButton.onClick.AddListener(Exit);
        componentButton.onClick.AddListener(OpenComponent);
        componentExitButton.onClick.AddListener(CloseComponent);
        gachaRecordButton.onClick.AddListener(OpenGachaRecord);
        gachaRecordExitButton.onClick.AddListener(CloseGachaRecord);
    }

    private void OnDisable()
    {
        gachaExitButton.onClick.RemoveListener(GachaExit);
        oneGachaButton.onClick.RemoveListener(OneGacha);
        tenGachaButton.onClick.RemoveListener(TenGacha);

        exitButton.onClick.RemoveListener(Exit);
        componentButton.onClick.RemoveListener(OpenComponent);
        componentExitButton.onClick.RemoveListener(CloseComponent);
        gachaRecordButton.onClick.RemoveListener(OpenGachaRecord);
        gachaRecordExitButton.onClick.RemoveListener(CloseGachaRecord);
    }

    private void OneGacha()
    {
        gacha.isTen = false;
        Manager.UI.PopUpUI_Action.Show($"잼 {gacha.needGem} 만큼을 사용하여 1회 뽑기를 진행하시겠습니까?", () => GemConsumption(1));
    }

    private void TenGacha()
    {
        gacha.isTen = true;
        Manager.UI.PopUpUI_Action.Show($"잼 {gacha.needGem * 10} 만큼을 사용하여 10회 뽑기를 진행하시겠습니까?", () => GemConsumption(10));
    }

    private void GemConsumption(int count)
    {
        if (Manager.Data.PlayerData.IsBuy(gacha.needGem * count))
        {
            Manager.Data.PlayerData.GemGain(-gacha.needGem * count);
            gacha.TryGacha();
        }
        else
        {
            Manager.UI.PopUpUI.Show("잼이 부족합니다.", Color.red);
        }
    }

    private void OpenComponent() => componentPanel.gameObject.SetActive(true);
    private void CloseComponent() => componentPanel.gameObject.SetActive(false);
    private void OpenGachaRecord() => gachaRecordPanel.gameObject.SetActive(true);
    private void CloseGachaRecord() => gachaRecordPanel.gameObject.SetActive(false);

    private void Exit()
    {
        lobby.SetActive(true);
        gameObject.SetActive(false);
    }

    private void GachaExit() => StartCoroutine(GachaExitRoutine());

    private IEnumerator GachaExitRoutine()
    {
        Manager.UI.FadeScreen.FadeIn(.5f);
        
        yield return new WaitForSeconds(1);
        gacha.gameObject.SetActive(false);

        StartCoroutine(CheckLevelUpRoutine());

        Manager.UI.FadeScreen.FadeOut(.5f);
    }

    public IEnumerator CheckLevelUpRoutine()
    {
        foreach (var kvp in gacha.beforeLevel)
        {
            if(gacha.afterLevel.ContainsKey(kvp.Key))
            {
                if (kvp.Value != gacha.afterLevel[kvp.Key])
                {
                    Instantiate(gachaResultPopUp, gachaResultContent).GetComponent<TankUpgradePopUp>().SetUp(kvp.Key);
                    yield return new WaitForSeconds(.1f);
                }
            }            
        }
    }
}
