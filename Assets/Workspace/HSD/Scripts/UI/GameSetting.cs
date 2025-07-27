using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSetting : MonoBehaviour
{
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider bgmSlider;

    private void OnEnable()
    {
        Subscribe();
        InitializeSliders();
    }

    private void OnDisable()
    {
        UnSubscribe();
    }

    private void Subscribe()
    {
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(UpdateSFXSlider);

        if (bgmSlider != null)
            bgmSlider.onValueChanged.AddListener(UpdateBGMSlider);
    }

    private void UnSubscribe()
    {
        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(UpdateSFXSlider);

        if (bgmSlider != null)
            bgmSlider.onValueChanged.RemoveListener(UpdateBGMSlider);
    }
    private void InitializeSliders()
    {
        if (sfxSlider != null)
            sfxSlider.value = DecibelToSliderValue(Manager.Audio.GetVolume(SoundType.SFX));

        if (bgmSlider != null)
            bgmSlider.value = DecibelToSliderValue(Manager.Audio.GetVolume(SoundType.BGM));
    }

    private void UpdateSFXSlider(float value)
    {
        Manager.Audio.SetVolume(SoundType.SFX, SliderValueToDecibel(value));
    }

    private void UpdateBGMSlider(float value)
    {
        Manager.Audio.SetVolume(SoundType.BGM, SliderValueToDecibel(value));
    }

    private float SliderValueToDecibel(float value)
    {
        if (value <= 0.0001f)
            return -80f;
        return Mathf.Log10(value) * 20f;
    }
    private float DecibelToSliderValue(float db)
    {
        return Mathf.Pow(10f, db / 20f);
    }
}
