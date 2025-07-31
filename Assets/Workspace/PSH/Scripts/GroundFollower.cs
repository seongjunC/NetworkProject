using UnityEngine;

/// <summary>
/// 최대 경사각을 설정하여, 가파른 벽이나 천장에서는 자연스럽게 떨어지도록 수정된 버전입니다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class GroundFollower : MonoBehaviour
{
    [Header("지면 감지 설정")]
    [Tooltip("지면으로 인식할 레이어를 선택합니다.")]
    public LayerMask groundLayer;
    [Tooltip("지면을 감지할 CircleCast의 반지름입니다.")]
    public float groundCheckRadius = 0.4f;
    [Tooltip("오브젝트 중심에서 아래로 발사할 CircleCast의 최대 거리입니다.")]
    public float groundCheckDistance = 0.5f;

    [Header("경사 제한 설정")]
    [Tooltip("캐릭터가 서 있을 수 있는 최대 경사 각도입니다. 이보다 가파르면 떨어집니다.")]
    public float maxSlopeAngle = 50f;

    [Header("물리 효과 설정")]
    [Tooltip("지형 경사에 맞춰 얼마나 부드럽게 회전할지 결정합니다.")]
    public float rotationSpeed = 20f;
    [Tooltip("땅에 붙어있게 하는 미세한 추가 속도입니다.")]
    public float stickiness = 1f;

    private Rigidbody2D rb;
    private float originalGravityScale;
    private bool isGrounded = false;
    private Vector2 groundNormal = Vector2.up;

    // --- Public 함수 --- //
    public bool IsGrounded() { return isGrounded; }
    public Vector2 GetGroundNormal() { return groundNormal; }

    // --- Unity 생명주기 함수 --- //
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale;
    }

    void FixedUpdate()
    {
        CheckGroundStatus();
        ApplyGroundPhysics();
    }

    private void CheckGroundStatus()
    {
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, groundCheckRadius, -transform.up, groundCheckDistance, groundLayer);

        if (hit.collider != null)
        {
            // 충돌 지점의 경사각을 계산합니다. (Vector2.up은 수직 위 방향)
            float slopeAngle = Vector2.Angle(Vector2.up, hit.normal);

            // 경사각이 최대 허용 각도보다 낮은 경우에만 '지상'으로 판단합니다.
            if (slopeAngle <= maxSlopeAngle)
            {
                isGrounded = true;
                groundNormal = hit.normal;
            }
            else
            {
                // 경사가 너무 가파릅니다. 공중 상태로 처리합니다.
                isGrounded = false;
                groundNormal = Vector2.up;
            }
        }
        else
        {
            // 충돌한 지면이 아예 없는 경우
            isGrounded = false;
            groundNormal = Vector2.up;
        }
    }

    private void ApplyGroundPhysics()
    {
        if (isGrounded)
        {
            rb.gravityScale = 0f;

            Vector2 velocity = rb.velocity;
            Vector2 perpendicularVelocity = Vector2.Dot(velocity, groundNormal) * groundNormal;
            rb.velocity = velocity - perpendicularVelocity;
            rb.velocity -= groundNormal * stickiness;
        }
        else
        {
            // 공중에 있거나, 경사가 너무 가파를 때
            rb.gravityScale = originalGravityScale;
        }

        Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}