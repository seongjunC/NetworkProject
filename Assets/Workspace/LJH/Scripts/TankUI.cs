using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TankUI : MonoBehaviour
{
    [SerializeField] private Button gachaButton;
    [SerializeField] private Button outButton;
    [SerializeField] private Button promotionButton;

    [Header("¿¬°áÇÒ ÆÐ³Î")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gachaPanel;
    [SerializeField] private GameObject promotionPanel;
    // Start is called before the first frame update
    void Start()
    {
        gachaButton.onClick.AddListener(OnClickGacha);
        outButton.onClick.AddListener(OnClickOut);
        promotionButton.onClick.AddListener(OnClickPromotion);
    }

    private void OnClickGacha()
    {
        gameObject.SetActive(false);         // ÅÊÅ© ÆÐ³Î ´Ý±â
        gachaPanel.SetActive(true);         // °¡Ã­ ÆÐ³Î ¿­±â
    }
    private void OnClickOut()
    {
        gameObject.SetActive(false);         // ÅÊÅ© ÆÐ³Î ´Ý±â
        mainMenuPanel.SetActive(true);       // ¸ÞÀÎ ¸Þ´º ÆÐ³Î ¿­±â
    }
    private void OnClickPromotion()
    {
        gameObject.SetActive(false);         // ÅÊÅ© ÆÐ³Î ´Ý±â
        promotionPanel.SetActive(true);      // ÇÁ·Î¸ð¼Ç ÆÐ³Î ¿­±â
    }
}
