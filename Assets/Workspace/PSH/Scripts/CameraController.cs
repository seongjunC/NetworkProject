using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    public CinemachineVirtualCamera vcamPlayer;
    public CinemachineVirtualCamera vcamBullet;

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
        if(vcamPlayer == null) return;
        vcamBullet.Priority = 5;
        vcamBullet.Follow = null;
        Debug.Log("카메라가 돌아옴");
    }
}
