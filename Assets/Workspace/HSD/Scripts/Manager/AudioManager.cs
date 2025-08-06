using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public enum SoundType
{
    BGM, SFX
}

public class AudioManager : Singleton<AudioManager>, ISavable
{
    private AudioMixer audioMixer;
    private AudioMixerGroup bgmGroup;
    private AudioMixerGroup sfxGroup;

    public AudioSource bgmSource;
    private AudioSource sfxSource;

    private Dictionary<string, AudioClip> sfxCached;
    private Dictionary<int, AudioSource> audioSourceCached;

    private Transform audioParent;

    private const string SOUND_PATH = "Sound/";

    private void Awake()
    {
        sfxCached = new Dictionary<string, AudioClip>();

        audioMixer = Manager.Resources.Load<AudioMixer>($"{SOUND_PATH}AudioMixer");
        bgmGroup = audioMixer.FindMatchingGroups("BGM")[0];
        sfxGroup = audioMixer.FindMatchingGroups("SFX")[0];
          
        audioParent = new GameObject("Audio").transform;
        audioParent.parent = transform;
        GameObject bgm = new GameObject("BGM_Source");
        bgm.transform.parent = audioParent;
        bgmSource = bgm.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.outputAudioMixerGroup = bgmGroup;

        GameObject effect = new GameObject("SFX_Source");
        effect.transform.parent = audioParent;
        sfxSource = effect.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;

        ResetCached();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ResetCached();
    }

    public void ResetCached()
    {
        sfxCached = new();
        audioSourceCached = new();
    }

    public void PlaySFX(string name, Vector3 position, float volume = 1, float pitch = 1)
    {
        if (string.IsNullOrEmpty(name)) return;

        if(!sfxCached.TryGetValue(name, out var clip))
            clip = LoadClip(name, SoundType.SFX);

        GameObject audioObj = Manager.Resources.Instantiate<GameObject>($"{SOUND_PATH}SFX_Obj", position, true);

        if (!audioSourceCached.TryGetValue(audioObj.GetInstanceID(), out var audio))
        {
            audio = audioObj.GetOrAddComponent<AudioSource>();
            
            if (audio != null)
                audioSourceCached[audioObj.GetInstanceID()] = audio;
        }

        audio.clip = clip;
        audio.pitch = pitch;
        audio.volume = volume * 2;
        audio.spatialBlend = 1;
        audio.outputAudioMixerGroup = sfxGroup;

        audio.Play();
        Manager.Resources.Destroy(audioObj, clip.length / pitch);
    }

    public void PlaySFX(AudioClip clip, Vector3 position, float volume = 1, float pitch = 1)
    {        
        GameObject audioObj = Manager.Resources.Instantiate<GameObject>($"{SOUND_PATH}SFX_Obj", position, true);

        if (!audioSourceCached.TryGetValue(audioObj.GetInstanceID(), out var audio))
        {
            audio = audioObj.GetComponent<AudioSource>();

            if (audio != null)

                audioSourceCached[audioObj.GetInstanceID()] = audio;
        }

        audio.clip = clip;
        audio.pitch = pitch;
        audio.volume = volume * 2;
        audio.spatialBlend = 1;
        audio.outputAudioMixerGroup = sfxGroup;

        audio.Play();
        Manager.Resources.Destroy(audioObj, clip.length / pitch);
    }

    public void PlaySFX(AudioClip clip, float volume = 1, float pitch = 1)
    {
        sfxSource.clip = clip;
        sfxSource.pitch = pitch;
        sfxSource.volume = volume * 2;
        sfxSource.spatialBlend = 1;
        sfxSource.outputAudioMixerGroup = sfxGroup;

        sfxSource.Play();
    }    

    public void PlayBGM(string name, float volume = 1, float pitch = 1)
    {
        if (string.IsNullOrEmpty(name)) return;

        if(bgmSource != null)
        {
            bgmSource.Stop();
        }
        bgmSource.clip = LoadClip(name, SoundType.BGM);
        bgmSource.volume = volume;
        bgmSource.pitch = pitch;
        bgmSource.Play();
    }

    public void PlayBGMFade(string name, float volume = 1f, float pitch = 1f, float fadeTime = 1f)
    {
        if (string.IsNullOrEmpty(name)) return;
        StartCoroutine(FadeOutAndPlayNewBGM(name, volume, pitch, fadeTime));
    }

    private IEnumerator FadeOutAndPlayNewBGM(string name, float volume, float pitch, float fadeTime)
    {
        if (bgmSource.isPlaying)
        {
            float startVolume = bgmSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeTime);
                bgmSource.volume = Mathf.Lerp(startVolume, 0, t);
                yield return null;
            }

            bgmSource.Stop();
        }

        bgmSource.clip = LoadClip(name, SoundType.BGM);
        bgmSource.pitch = pitch;
        bgmSource.Play();

        for (float t = 0; t < fadeTime; t += Time.deltaTime)
        {
            bgmSource.volume = Mathf.Lerp(0, volume, t / fadeTime);
            yield return null;
        }

        bgmSource.volume = volume;
    }

    private AudioClip LoadClip(string name, SoundType type)
    {
        if (string.IsNullOrEmpty(name)) return null;

        if(type == SoundType.SFX)
        {
            if(sfxCached.ContainsKey(name))
                return sfxCached[name];

            AudioClip clip = Manager.Resources.Load<AudioClip>($"{SOUND_PATH}{type}/{name}");

            if(clip != null)
                sfxCached[name] = clip;

            return clip;
        }
        Debug.Log($"{SOUND_PATH}{type}/{name}");
        return Manager.Resources.Load<AudioClip>($"{SOUND_PATH}{type}/{name}");
    }

    public void StopBGM() => bgmSource.Stop();

    public void SetVolume(SoundType type, float volume)
    {
        if (type == SoundType.SFX)
            audioMixer.SetFloat("SFX", volume);
        else
            audioMixer.SetFloat("BGM", volume);
    }
    public float GetVolume(SoundType type)
    {
        float db;
        audioMixer.GetFloat(type.ToString(), out db);
        return db;
    }

    public void Save(ref GameData data)
    {
        data.bgm = GetVolume(SoundType.BGM);
        data.sfx = GetVolume(SoundType.SFX);
    }

    public void Load(GameData data)
    {
        SetVolume(SoundType.BGM, data.bgm);
        SetVolume(SoundType.SFX, data.sfx);        
    }
}