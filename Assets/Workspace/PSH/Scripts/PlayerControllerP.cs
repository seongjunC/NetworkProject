using UnityEngine;

public class PlayerControllerP : MonoBehaviour
{
    // =================================================================================
    // 컴포넌트 및 설정 변수
    // =================================================================================

    [Header("이동 및 지면 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 1f;
    [SerializeField] private float groundCheckDistance = 1.1f;
    [SerializeField] private float maxSlopeAngle = 60f;
    [SerializeField] private float rotationSpeed = 10f;
    [Tooltip("지면에 붙어있게 하는 힘입니다. 높을수록 잘 붙고 미끄러지지 않습니다.")]
    [SerializeField] private float stickToGroundForce = 5f;

    [Header("조준 및 발사 설정")]
    [SerializeField] private Transform firePivot;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float maxPower = 20f;
    [SerializeField] private float chargingSpeed = 10f;
    [SerializeField] private float angleStep = 0.5f;

    // =================================================================================
    // 내부 상태 변수
    // =================================================================================

    private Rigidbody2D rb;
    private float originalGravityScale;
    private bool isGrounded = false;
    private Vector2 groundNormal = Vector2.up;

    private float horizontalInput;
    private float angle = 45f;
    private float powerCharge = 0f;
    private bool isCharging = false;
    private bool isFacingRight = true;

    // =================================================================================
    // Unity 생명주기 함수
    // =================================================================================

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void Update()
    {
        HandleInput();
        Aim();
        Flip();
    }

    void FixedUpdate()
    {
        CheckGroundStatus();
        ApplyMovementAndRotation();
    }

    // =================================================================================
    // 핵심 로직
    // =================================================================================

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            angle += angleStep;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            angle -= angleStep;
        }
        angle = Mathf.Clamp(angle, 0f, 90f);

        if (Input.GetKey(KeyCode.Space))
        {
            isCharging = true;
            powerCharge += chargingSpeed * Time.deltaTime;
            powerCharge = Mathf.Clamp(powerCharge, 0f, maxPower);
        }

        if (isCharging && Input.GetKeyUp(KeyCode.Space))
        {
            Fire();
            powerCharge = 0f;
            isCharging = false;
        }
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
            rb.gravityScale = 0f;

            // 지상에서는 입력을 받아 움직입니다.
            Vector2 moveDirection = new Vector2(groundNormal.y, -groundNormal.x).normalized;
            Vector2 targetVelocity = moveDirection * horizontalInput * moveSpeed;
            rb.velocity = targetVelocity - (groundNormal * stickToGroundForce);
        }
        else
        {
            // [수정] 공중에서는 중력만 적용하고, 키보드 입력으로 인한 수평 이동은 막습니다.
            rb.gravityScale = originalGravityScale;
        }

        // 몸체 회전 로직은 항상 적용됩니다.
        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private void Flip()
    {
        // [수정] 지상에 있을 때만 방향 전환이 가능하도록 합니다.
        if (isGrounded && ((horizontalInput > 0 && !isFacingRight) || (horizontalInput < 0 && isFacingRight)))
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    private void Aim()
    {
        firePivot.localRotation = Quaternion.Euler(0, 0, angle);
    }

    private void Fire()
    {
        if (projectilePrefab == null) return;

        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            projectileRb.velocity = firePoint.right * powerCharge;
        }
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