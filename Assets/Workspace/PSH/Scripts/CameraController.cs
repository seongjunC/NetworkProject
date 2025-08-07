using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Linq;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public CinemachineVirtualCamera vcamPlayer;
    public CinemachineVirtualCamera vcamBullet;

    private CinemachineBasicMultiChannelPerlin _perlin;
    private float _shakeTimer;
    private float _shakeDuration;
    private float _startAmplitude;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("인스턴스 이미 존재. 새로 생성된 인스턴스 파괴");
        }
        else
        {
            Instance = this;
        }
    }

    public void FollowBullet(Transform bulletTransform)
    {
        if (vcamBullet == null) return;
        vcamBullet.Follow = bulletTransform;
        vcamBullet.Priority = 20;
        Debug.Log("카메라가 투사체를향함");
    }

    public void ReturnToPlayerCam()
    {
        if (vcamPlayer == null) return;
        vcamBullet.Priority = 5;
        vcamBullet.Follow = null;
        Debug.Log("카메라가 돌아옴");
    }

    public void ShakeCam(float intensity = 3f, float duration = .5f)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        var perlin = vcamBullet.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        if (perlin == null)
        {
            Debug.LogWarning("Perlin Noise 컴포넌트가 없어요~");
            yield break;
        }

        float timer = 0f;
        perlin.m_AmplitudeGain = intensity;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        perlin.m_AmplitudeGain = 0f;
    }

    public void HighlightItems(List<Transform> targets, float totalDuration = 2f)
    {
        StartCoroutine(HighlightRoutine(targets, totalDuration));
    }

    public IEnumerator HighlightRoutine(List<Transform> targets, float totalDuration)
    {
        float per = totalDuration / targets.Count;

        foreach (var target in targets)
        {

            vcamBullet.Follow = target;
            vcamBullet.Priority = 20;

            yield return new WaitForSeconds(per);
        }
        ReturnToPlayerCam();
    }

}
