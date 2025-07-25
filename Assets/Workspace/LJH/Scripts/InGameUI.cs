using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [Header("Bars")]
    public Slider hpBar;
    public Slider moveBar;
    public Slider powerBar;

    [Header("Wind")]
    public Slider windBar;         // 좌/우 바람 세기
    public RectTransform flagImage; // 깃발 방향용 이미지

    // 체력 업데이트
    public void SetHP(float current, float max)
    {
        hpBar.value = current / max;
    }

    // 이동 게이지 업데이트
    public void SetMove(float current, float max)
    {
        moveBar.value = current / max;
    }

    // 파워 차지 업데이트
    public void SetPower(float charge)
    {
        powerBar.value = charge;
    }

    // 바람 세기 업데이트
    public void SetWind(float windStrength)
    {
        // windStrength는 -10 ~ +10 범위로 가정
        windBar.value = Mathf.Abs(windStrength);
    }
}
