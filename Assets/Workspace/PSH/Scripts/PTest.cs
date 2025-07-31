using UnityEngine;

/// <summary>
/// 플레이어의 이동과 발사를 담당합니다.
/// GroundFollower와 연동하여 경사면을 따라 이동하고, 입력이 없으면 확실히 멈춥니다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(GroundFollower))] // GroundFollower가 필수임을 명시
public class PTest : MonoBehaviour
{
    [Header("이동 설정")]
    public float moveSpeed = 5f;

    [Header("발사체 설정")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public Transform muzzlePoint;

    // private 컴포넌트 및 변수
    private Rigidbody2D rb;
    private GroundFollower groundFollower; // GroundFollower 스크립트 참조
    private float horizontalInput = 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        groundFollower = GetComponent<GroundFollower>();
    }

    private void Update()
    {
        // 이동 입력을 받습니다. (-1, 0, 1)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 발사 입력을 받습니다.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    void FixedUpdate()
    {
        // GroundFollower가 지면에 있다고 판단할 때만 이동 로직을 처리합니다.
        if (groundFollower.IsGrounded())
        {
            // 1. 이동 방향 계산
            // GroundFollower가 감지한 지면 경사(groundNormal)와 평행한 방향을 구합니다.
            Vector2 moveDirection = new Vector2(groundFollower.GetGroundNormal().y, -groundFollower.GetGroundNormal().x);

            // 2. 최종 속도 계산
            // 입력 값(horizontalInput)과 속력(moveSpeed)을 곱해 최종 속도를 결정합니다.
            Vector2 targetVelocity = moveDirection * horizontalInput * moveSpeed;

            // 3. Rigidbody에 속도 적용
            rb.velocity = targetVelocity;
        }
        // 공중에 있을 때는 GroundFollower가 중력을 처리하므로 별도 로직이 필요 없습니다.
    }

    void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile Prefab이 할당되지 않았습니다.");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, muzzlePoint.transform.position, Quaternion.identity);
        // CameraController.Instance.FollowBullet(projectile.transform); // 이 부분은 CameraController가 있어야 작동합니다.
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            // 발사 방향을 포구(muzzlePoint)의 up 방향(포신 방향)으로 설정합니다.
            Vector2 launchDirection = muzzlePoint.up;
            projectileRb.velocity = launchDirection * projectileSpeed;
        }
        else
        {
            Debug.LogWarning("투사체 프리팹에 Rigidbody2D 컴포넌트가 없습니다.");
        }
    }
}