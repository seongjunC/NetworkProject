using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public static class Utils
{
    #region Fade
    public static IEnumerator Fade(Func<Color> getColor, Action<Color> setColor, 
        float start, float end, float fadeTime = 1, float delay = 0, Action action = null)
    {
        if(delay > 0) yield return new WaitForSeconds(delay);

        if(getColor == null || setColor == null) yield break;

        float percent = 0f;
        float elapsedTime = 0f;
        Color color;

        while (percent < 1)
        {
            elapsedTime += Time.deltaTime;
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
    public static bool Contain(this LayerMask layerMask, int layer)
    {
        return ((1 << layer) & layerMask) != 0;
    }
    #endregion
}
