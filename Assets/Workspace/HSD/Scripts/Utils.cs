using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public static class Utils
{
    #region Fade
    public static IEnumerator Fade(Func<Color> getColor, Action<Color> setColor, 
        float start, float end, float fadeTime = 1, float delay = 0, Action action = null)
    {
        if(delay > 0) yield return new WaitForSecondsRealtime(delay);

        if(getColor == null || setColor == null) yield break;

        float percent = 0f;
        float elapsedTime = 0f;
        Color color;

        while (percent < 1)
        {
            elapsedTime += Time.unscaledDeltaTime;
            percent = Mathf.Clamp01(elapsedTime / fadeTime);

            color = getColor();
            color.a = Mathf.Lerp(start, end, percent);
            setColor(color);
            yield return null;
        }

        action?.Invoke();
    }

    public static IEnumerator Fade(UnityEngine.UI.Image target, float start, float end, float fadeTime = 1, float delay = 0, Action action = null)
    {
        yield return Fade(() => target.color, c => target.color = c, start, end, fadeTime, delay, action);
    }

    public static IEnumerator Fade(TextMeshProUGUI target, float start, float end, float fadeTime = 1, float delay = 0, Action action = null)
    {
        yield return Fade(() => target.color, c => target.color = c, start, end, fadeTime, delay, action);
    }
    #endregion
    public static bool Contain(this LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }

    public static TankData GetTankData(this GachaResult result)
    {
        return Manager.Data.TankDataController.TankDatas[result.Name];
    }

    public static Color SColor = new Color(1f, 0.5f, 0f);   // 주황
    public static Color AColor = new Color(0.3f, 0.7f, 1f); // 하늘색
    public static Color BColor = new Color(0.5f, 1f, 0.5f); // 연두
    public static Color CColor = new Color(0.6f, 0.6f, 0.6f); // 회색

    public static Color GetColor(Rank rank)
    {
        return rank switch
        {
            Rank.S => SColor,
            Rank.A => AColor,
            Rank.B => BColor,
            Rank.C => CColor,
            _ => CColor
        };
    }
}
