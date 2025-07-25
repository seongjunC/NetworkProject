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
    public Slider windBar;         // ��/�� �ٶ� ����
    public RectTransform flagImage; // ��� ����� �̹���

    // ü�� ������Ʈ
    public void SetHP(float current, float max)
    {
        hpBar.value = current / max;
    }

    // �̵� ������ ������Ʈ
    public void SetMove(float current, float max)
    {
        moveBar.value = current / max;
    }

    // �Ŀ� ���� ������Ʈ
    public void SetPower(float charge)
    {
        powerBar.value = charge;
    }

    // �ٶ� ���� ������Ʈ
    public void SetWind(float windStrength)
    {
        // windStrength�� -10 ~ +10 ������ ����
        windBar.value = Mathf.Abs(windStrength);
    }
}
