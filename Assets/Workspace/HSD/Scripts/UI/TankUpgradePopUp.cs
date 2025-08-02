using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankUpgradePopUp : MonoBehaviour
{
    [SerializeField] Image tankIcon;
    [SerializeField] TMP_Text message;    

    public void SetUp(TankData data)
    {
        tankIcon.sprite = data.icon;
        message.text = $"{data.tankName}의 레벨이 {data.Level}로 올랐습니다!";        
    }

    private void Delete() => Destroy(gameObject);
}
