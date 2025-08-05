using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : MonoBehaviour
{
    [SerializeField] string bgmName;
    [SerializeField] float volume;
    [SerializeField] float pitch;
    [SerializeField] float fadeTime;

    private void Awake()
    {
        volume = volume == 0 ? .8f : volume;
        pitch = pitch == 0 ? 1 : pitch;
        fadeTime = fadeTime == 0 ? .5f : fadeTime;
    }

    private void OnEnable()
    {        
        Manager.Audio.PlayBGMFade(bgmName,volume,pitch,fadeTime);
    }
}
