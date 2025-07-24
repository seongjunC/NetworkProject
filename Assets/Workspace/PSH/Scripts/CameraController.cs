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
            Debug.Log("�ν��Ͻ� �̹� ����. ���� ������ �ν��Ͻ� �ı�");
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
        Debug.Log("ī�޶� ����ü������");
    }

    public void ReturnToPlayerCam()
    {
        if(vcamPlayer == null) return;
        vcamBullet.Priority = 5;
        vcamBullet.Follow = null;
        Debug.Log("ī�޶� ���ƿ�");
    }
}
