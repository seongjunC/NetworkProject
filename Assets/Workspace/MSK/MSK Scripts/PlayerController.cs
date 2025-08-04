using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
public class PlayerController : MonoBehaviourPun
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

    public bool isControllable { get; private set; } = false;
    public bool IsAttacked { get; private set; } = false;

    public Action OnPlayerAttacked;
    public Action OnPlayerDied;

    [Header("이동 및 지면 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 1f;
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private float maxSlopeAngle = 60f;
    [SerializeField] private float rotationSpeed = 10f;
    [Tooltip("지면에 붙어있게 하는 힘입니다. 높을수록 잘 붙고 미끄러지지 않습니다.")]
    [SerializeField] private float stickToGroundForce = 5f;

    private float originalGravityScale;
    private bool isGrounded = false;
    private Vector2 groundNormal = Vector2.up;

    private float horizontalInput;
    private float angle = 45f;
    private float powerCharge = 0f;
    private bool isCharging = false;
    private bool isFacingRight = true;
    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        originalGravityScale = _rigidbody.gravityScale;
        _rigidbody.bodyType = RigidbodyType2D.Dynamic;

        if (photonView.IsMine)
        {
            _movable = _data.maxMove;
            _hp = _data.maxHp;
            myInfo = new PlayerInfo(photonView.Owner);
            TestBattleManager battleManager = FindObjectOfType<TestBattleManager>();
            MSK_UIManager uiManager = FindObjectOfType<MSK_UIManager>();

            if (battleManager != null)
                battleManager.RegisterPlayer(this);
            if (uiManager != null)
                uiManager.RegisterPlayer(this);

            PlayerSetUp();
        }

        // 닉네임 색상 설정
        //_textMeshPro.text = photonView.IsMine
        //    ? $"<color=#00aaff>{PhotonNetwork.NickName}</color>"
        //    : $"<color=#ff4444>{photonView.Owner.NickName}</color>";

        _textMeshPro.text = photonView.Owner.NickName;
        _textMeshPro.color = CustomProperty.GetTeam(photonView.Owner) == Game.Team.Red ? Color.red : Color.blue;
    }

    private void PlayerSetUp()
    {
        myInfo = new PlayerInfo(photonView.Owner);
    }
    void FixedUpdate()
    {
        CheckGroundStatus();
        ApplyMovementAndRotation();
    }
    private void Update()
    {
        if (!photonView.IsMine || _isDead || !isControllable)
            return;

        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (_movable <= 0)
        {
            horizontalInput = 0;
            return;
        }

        if (horizontalInput != 0)
        {
            _movable -= Mathf.Abs(horizontalInput) * _data.speed * Time.deltaTime;
            player.localScale = new Vector3(horizontalInput, 1f, 1f);
        }
    }

    [PunRPC]
    public void OnHit(int damage)
    {
        if (OnBarrier)
        {
            if (damage > 100000)
            {
                PlayerDead();
            }
            OnBarrier = false;
            Debug.Log("배리어로 1회 피격 방어");
            return;
        }
        _hp -= damage;
        Debug.Log("피격");

        if (_hp <= 0)
            PlayerDead();
    }

    public void EndPlayerTurn()
    {
        _movable = 0;
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
            PlayerDead();
    }

    public void PlayerDead()
    {
        if (_isDead) return;
        _isDead = true;
        OnPlayerAttacked = null;
        Debug.Log("플레이어 사망");
        OnPlayerDied?.Invoke();
    }

    [PunRPC]
    public void RPC_PCDead()
    {
        gameObject.SetActive(false);
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
}
