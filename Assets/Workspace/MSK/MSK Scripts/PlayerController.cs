using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviourPun, IPunObservable
{
    [SerializeField] private Rigidbody2D _rigidbody;
    [SerializeField] private Transform player;
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    public TankData _data;

    public bool _isDead { get; private set; } = false;
    public bool OnBarrier { get; private set; } = false;
    public PlayerInfo myInfo;
    public float _hp;
    public float _movable;
    public float _damage;
    public bool isControllable { get; private set; } = false;
    public bool IsAttacked { get; private set; } = false;
    public Action OnPlayerAttacked;

    [Header("이동 및 지면 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = .3f;
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private float maxSlopeAngle = 40f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float stickToGroundForce = 5f;

    [Header("포격 조준")]
    [SerializeField] private Transform muzzleRotatePos;
    [SerializeField] private float muzzleRotationSpeed = 50f; // 포신 회전 속도

    private float turretAngle = 0f;
    private bool isFacingRight = true;

    private float network_turretAngle = 0f;
    private Vector3 network_localScale = Vector3.one;

    private float originalGravityScale;
    private bool isGrounded = false;
    private Vector2 groundNormal = Vector2.up;
    private float horizontalInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        originalGravityScale = _rigidbody.gravityScale;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;

        player = GetComponent<Transform>();
        _textMeshPro = GetComponentInChildren<TextMeshProUGUI>();

        myInfo = new PlayerInfo(photonView.Owner);
        if (photonView.IsMine)
        {
            TestBattleManager battleManager = FindObjectOfType<TestBattleManager>();
            MSK_UIManager uiManager = FindObjectOfType<MSK_UIManager>();

            if (battleManager != null)
                battleManager.RegisterPlayer(this);
            if (uiManager != null)
                uiManager.RegisterPlayer(this);

            PlayerSetUp();
        }
        else
        {
            // 다른 클라이언트의 탱크는 물리적으로 직접 제어하지 않음
            _rigidbody.isKinematic = true;
        }

        InitPlayecr(photonView.InstantiationData);
        _textMeshPro.text = photonView.Owner.NickName;
        _textMeshPro.color = CustomProperty.GetTeam(photonView.Owner) == Game.Team.Red ? Color.red : Color.blue;
    }
    void FixedUpdate()
    {
        // 내꺼만 물리처리
        if (!photonView.IsMine)
        {
            return;
        }
        CheckGroundStatus();
        ApplyMovementAndRotation();
    }

    private void Update()
    {
        // 입력 처리는 IsMine인 클라이언트에서만 실행
        if (!photonView.IsMine || _isDead || !isControllable)
        {
            return;
        }

        // 이동 입력
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            isFacingRight = horizontalInput > 0;
            player.localScale = new Vector3(isFacingRight ? 1f : -1f, 1f, 1f);

            if (_movable > 0)
            {
                _movable -= Time.deltaTime;
                Debug.Log("움직이는중");
            }         
        }

        

        if (_movable <= 0)
        {
            horizontalInput = 0;
        }


        // 포신 각도 조절 입력
        float verticalInput = Input.GetAxisRaw("Vertical");
        if (verticalInput != 0)
        {
            float angleDelta = verticalInput * muzzleRotationSpeed * Time.deltaTime;
            turretAngle = Mathf.Clamp(turretAngle + angleDelta, 0f, 90f);
        }
        // 포신 각도 즉시 반영
        muzzleRotatePos.localRotation = Quaternion.Euler(0, 0, turretAngle);
    }

    // 시각적 동기화는 LateUpdate에서 처리
    private void LateUpdate()
    {
        if (!photonView.IsMine)
        {
            // Lerp를 사용하여 다른 클라이언트 탱크의 움직임을 부드럽게 보간
            player.localScale = Vector3.Lerp(player.localScale, network_localScale, Time.deltaTime * 10f);
            muzzleRotatePos.localRotation = Quaternion.Lerp(muzzleRotatePos.localRotation, Quaternion.Euler(0, 0, network_turretAngle), Time.deltaTime * 10f);
        }
    }

    // IPunObservable 인터페이스 구현
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 이 클라이언트가 소유자일 경우, 데이터를 스트림에 씁니다.
            stream.SendNext(player.localScale);
            stream.SendNext(turretAngle);
        }
        else
        {
            // 다른 클라이언트의 경우, 스트림에서 데이터를 읽습니다.
            this.network_localScale = (Vector3)stream.ReceiveNext();
            this.network_turretAngle = (float)stream.ReceiveNext();
        }
    }

    /// <summary>
    /// 플레이어 초기화함수 (생성될 때 실행됨)
    /// </summary>
    private void InitPlayecr(object[] datas)
    {
        TankData data = Manager.Data.TankDataController.TankDatas[(string)datas[0]];
        _data = Instantiate(data);      // 개별 인스턴스를 생성해야 다른 클라에 영향을 주지 않음
        _data.Level = (int)datas[1];
        _data.InitStat();

        _movable = _data.maxMove;
        _hp = _data.maxHp;
        _damage = _data.damage;
        // 달라져야 할 데이터들을 모두 세팅함
        // 예를 들어 Animator, 총알 프리팹
    }
    private void PlayerSetUp()
    {
        myInfo = new PlayerInfo(photonView.Owner);
    }

    [PunRPC]
    public void OnHit(float damage)
    {
        if (damage > 100000)
        {
            PlayerDead();
        }
        if (OnBarrier)
        {
            OnBarrier = false;
            Debug.Log("배리어로 1회 피격 방어");
            return;
        }

        _hp -= damage;
        Debug.Log("피격");

        MSKTurnController.Instance.photonView.RPC(
            "RPC_RecordDamage",
            RpcTarget.MasterClient,
            damage
        );

        if (_hp <= 0)
            PlayerDead();
    }

    public void EndPlayerTurn()
    {
        //_movable = 0;
        SetAttacked(true);
        isControllable = false;
    }

    public void ResetTurn()
    {
        _movable = _data.maxMove;
        SetAttacked(false);
        isControllable = true;
    }

    public void SetAttacked(bool value)
    {
        if (IsAttacked == value) return;

        IsAttacked = value;

        if (IsAttacked)
        {
            OnPlayerAttacked?.Invoke();
            isControllable = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("MapBoundary"))
        {
            if (player == null)
                return;
            PlayerDead();
        }
    }

    public void PlayerDead()
    {
        if (_isDead)
            return;
        _isDead = true;
        OnPlayerAttacked = null;
        Debug.Log("플레이어 사망");
        if (PhotonView.Find(2) == null)
            return;

        PhotonView.Find(2).RPC("RPC_PlayerDead", RpcTarget.All, photonView.Owner.ActorNumber);
    }

    public void EnableControl(bool enable)
    {
        isControllable = enable;
    }
    public void ApplyBarrier()
    {
        OnBarrier = true;
    }

    public void FixedHealToPlayer(int Amount)
    {
        float beforeHp = _hp;
        _hp = MathF.Min(_hp + Amount, _data.maxHp);
        Debug.Log($"{_hp - beforeHp} 만큼 체력 회복");
    }
    public void RatioHealToPlayer(int Amount)
    {
        float beforeHp = _hp;
        _hp = MathF.Min(_hp + _data.maxHp * Amount / 100, _data.maxHp);
        Debug.Log($"{_hp - beforeHp} 만큼 체력 회복");
    }
    private void CheckGroundStatus()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, groundCheckRadius, Vector2.down, groundCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            float slopeAngle = Vector2.Angle(Vector2.up, hit.normal);
            if (slopeAngle <= maxSlopeAngle)
            {
                isGrounded = true;
                groundNormal = hit.normal;
                return;
            }
        }

        isGrounded = false;
        groundNormal = Vector2.up;
    }

    /// <summary>
    /// 지면 상태에 따라 이동, 중력, 회전을 적용합니다.
    /// </summary>
    private void ApplyMovementAndRotation()
    {
        if (isGrounded)
        {
            _rigidbody.gravityScale = 0f;

            // 지상에서는 입력을 받아 움직입니다.
            Vector2 moveDirection = new Vector2(groundNormal.y, -groundNormal.x).normalized;
            Vector2 targetVelocity = moveDirection * horizontalInput * moveSpeed;
            _rigidbody.velocity = targetVelocity - (groundNormal * stickToGroundForce);
        }
        else
        {
            // [수정] 공중에서는 중력만 적용하고, 키보드 입력으로 인한 수평 이동은 막습니다.
            _rigidbody.gravityScale = originalGravityScale;
        }

        // 몸체 회전 로직은 항상 적용됩니다.
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.down * groundCheckDistance;
        Gizmos.DrawWireSphere(startPos, groundCheckRadius);
        Gizmos.DrawWireSphere(endPos, groundCheckRadius);
    }

    public bool HasFreeItemSlot()
    {
        return !myInfo.Isfull();
    }

    public bool TryAcquireItem(ItemData data)
    {
        return myInfo.ItemAcquire(data);
    }
}
