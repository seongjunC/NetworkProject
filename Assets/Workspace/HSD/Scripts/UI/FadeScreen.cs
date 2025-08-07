using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScreen : MonoBehaviour
{
    [SerializeField] Image fadeImage;    

    public void FadeInOut(float fadeTime, float delay, Sprite sprite = null)
    {        
        fadeImage.sprite = sprite;
        fadeImage.color = sprite == null ? Color.black : Color.white;

        StartCoroutine(FadeRoutine(fadeTime, delay));
    }    

    public void FadeIn(float fadeTime = 1, Sprite sprite = null)
    {
        fadeImage.raycastTarget = true;
        fadeImage.sprite = sprite;
        fadeImage.color = sprite == null ? Color.black : Color.white;
        StartCoroutine(Utils.Fade(fadeImage, 0, 1, fadeTime));
    }

    public void FadeOut(float fadeTime = 1)
    {
        StartCoroutine(Utils.Fade(fadeImage, 1, 0, fadeTime));
        fadeImage.raycastTarget = false;
    }

    private IEnumerator FadeRoutine(float fadeTime, float delay)
    {
        fadeImage.raycastTarget = true;
        yield return Utils.Fade(fadeImage, 0, 1, fadeTime);

        yield return new WaitForSecondsRealtime(delay);

        yield return Utils.Fade(fadeImage, 1, 0, fadeTime);
        fadeImage.raycastTarget = false;
    }
}
