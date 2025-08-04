using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;

public class Fire : MonoBehaviourPun
{
    [Header("References")]
    [SerializeField] private Transform firePivot;       // 회전할 포신 부분
    [SerializeField] private Transform firePoint;       // 실제 폭탄이 나갈 위치

    [SerializeField] private GameObject bulletPrefab;

    [Header("Controls")]
    [SerializeField] private float angle = 45f;         // 포신의 현재 각도
    [SerializeField] private float angleStep = 0.5f;      // 각도 변화량
    [SerializeField] private float chargingSpeed = 10f;    // 차지속도

    [SerializeField] public float maxPower = 20f;         // 폭탄 발사 속도

    public float powerCharge = 0f;         // 차지
    private bool isCharging = false;        // 차지 중인지 여부
    private bool isDoubleAttack = false;
    private bool OnDamageBuff = false;
    private List<float?> DamageBuff = new List<float?>(2);


    private ProjectileManager _projectileManager;
    private PlayerController _playerController;

    private void Awake()
    {
        _playerController = GetComponentInParent<PlayerController>();
        _projectileManager = FindObjectOfType<ProjectileManager>();
        InitBuff();
    }


    private void Update()
    {
        if (!photonView.IsMine)
            return;

        if (!_playerController.isControllable)
            return;

        Aim();
        //  이미 공격했다면 공격 불가능
        if (_playerController.IsAttacked)
            return;
        // 스페이스바 누르고 있으면 차지 시작
        if (Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
            powerCharge += chargingSpeed * Time.deltaTime;
            powerCharge = Mathf.Clamp(powerCharge, 0f, maxPower);
        }

        // 스페이스바에서 손을 뗐을 때 발사
        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            Shoot();
            if (isDoubleAttack)
            {
                Invoke(nameof(Shoot), 1f);
                isDoubleAttack = false;
            }
            powerCharge = 0f;
            isCharging = false;
            _playerController.SetAttacked(true);
        }
        Debug.DrawRay(firePoint.position, firePoint.up * 2f, Color.red);


    }

    private void Aim()
    {
        // 각도 조절 (Up/Down)
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            angle += angleStep;
            if (angle > 90f) angle = 90f;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            angle -= angleStep;
            if (angle < 0f) angle = 0f;
        }
        // 포신 회전 
        firePivot.localRotation = Quaternion.Euler(0, 0, angle);
    }

    // 발사 
    private void Shoot()
    {
        // 마스터 클라이언트에게 발사 요청
        object[] damageBuffObjects = new object[DamageBuff.Count];
        for (int i = 0; i < DamageBuff.Count; i++)
        {
            damageBuffObjects[i] = DamageBuff[i];
        }
        _projectileManager.photonView.RPC("RPC_RequestFireProjectile", RpcTarget.MasterClient,
            firePoint.position, firePoint.rotation, powerCharge,
            OnDamageBuff, damageBuffObjects, PhotonNetwork.LocalPlayer.ActorNumber);

        InitDamageBuff(); // 발사 후 데미지 버프 초기화
    }

    public void SetDoubleAttack()
    {
        isDoubleAttack = true;
        Debug.Log("더블 어택 활성화");
    }

    public void SetRatioDamageBuff(float Amount)
    {
        OnDamageBuff = true;
        DamageBuff[1] += Amount;
    }
    public void SetFixedDamageBuff(float Amount)
    {
        OnDamageBuff = true;
        DamageBuff[0] += Amount;
    }
    public void InitBuff()
    {
        isDoubleAttack = false;
        InitDamageBuff();
    }
    public void InitDamageBuff()
    {
        OnDamageBuff = false;
        DamageBuff.Clear();
        for (int i = 0; i < DamageBuff.Count; i++)
        {
            DamageBuff.Add(null);
        }
    }

}
