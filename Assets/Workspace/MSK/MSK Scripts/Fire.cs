using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("References")]
    // [SerializeField] private Transform firePivot; // PlayerController가 제어하므로 주석 처리 또는 삭제
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Controls")]
    // angle, angleStep은 PlayerController로 이동했으므로 삭제
    [SerializeField] private float chargingSpeed = 10f;
    [SerializeField] public float maxPower = 20f;

    public float powerCharge = 0f;
    public bool isCharging = false;
    private bool isDoubleAttack = false;
    private bool OnDamageBuff = false;
    private List<float?> DamageBuff = new List<float?>(2);

    private ProjectileManager _projectileManager;
    private PlayerController _playerController;

    private string projectileName;

    private void Awake()
    {
        _playerController = GetComponentInParent<PlayerController>();
        _projectileManager = FindObjectOfType<ProjectileManager>();
        InitBuff();

        projectileName = bulletPrefab.name;
    }

    private void Update()
    {
        if (!photonView.IsMine ||
        !MSKTurnController.Instance.IsMyTurn() ||
        !MSKTurnController.Instance.isTurnRunning ||
        !_playerController.isControllable)
        {
            if (isCharging)
            {
                isCharging = false;
                powerCharge = 0f;
            }
            return;
        }


        // 스페이스바 누르고 있으면 차지 시작
        if (Input.GetKey(KeyCode.Space))
        {
            // isCharging 상태를 네트워크로 동기화
            isCharging = true;
            powerCharge += chargingSpeed * Time.deltaTime;
            powerCharge = Mathf.Clamp(powerCharge, 0f, maxPower);
        }

        // 스페이스바에서 손을 뗐을 때 또는 풀차지시 발사
        if (isCharging && Input.GetKeyUp(KeyCode.Space) || powerCharge >= maxPower)
        {
            Shoot();
            if (isDoubleAttack)
            {
                Invoke(nameof(Shoot), 1f);
                isDoubleAttack = false;
            }
            MSKTurnController.Instance.testBattleManager.SetTurnEndButton(false);
            powerCharge = 0f;
            isCharging = false;
            _playerController.SetAttacked(true);
        }
        Debug.DrawRay(firePoint.position, firePoint.up * 2f, Color.red);
    }

    private void Shoot()
    {
        // 마스터 클라이언트에게 발사 요청
        object[] damageBuffObjects = new object[DamageBuff.Count];
        for (int i = 0; i < DamageBuff.Count; i++)
        {
            damageBuffObjects[i] = DamageBuff[i];
        }
        // PlayerController의 firePoint 정보를 사용합니다.
        // 포신 각도 * 로컬스케일 + 지면 각도
        bool isRight = transform.localScale.z > 0;
        float playerAngle = gameObject.transform.eulerAngles.z;
        _projectileManager.photonView.RPC(nameof(ProjectileManager.RPC_RequestFireProjectile), RpcTarget.MasterClient,
            firePoint.position, firePoint.rotation, powerCharge,
            OnDamageBuff, damageBuffObjects, PhotonNetwork.LocalPlayer.ActorNumber, playerAngle, isRight
            , projectileName, _playerController._damage);

        InitDamageBuff();
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
        powerCharge = 0f;
        isCharging = false;

    }

    // IPunObservable 인터페이스 구현
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 로컬 플레이어는 자신의 isCharging 상태를 다른 클라이언트에게 보냅니다.
            stream.SendNext(isCharging);
        }
        else
        {
            // 다른 클라이언트는 스트림에서 isCharging 상태를 받습니다.
            isCharging = (bool)stream.ReceiveNext();
        }
    }
}