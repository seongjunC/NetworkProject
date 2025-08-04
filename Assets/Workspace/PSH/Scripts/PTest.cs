using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 플레이어의 이동과 발사를 담당합니다.
/// GroundFollower와 연동하여 경사면을 따라 이동하고, 입력이 없으면 확실히 멈춥니다.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(GroundFollower))] // GroundFollower가 필수임을 명시
public class PTest : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePivot;       // 회전할 포신 부분
    [SerializeField] private Transform firePoint;       // 실제 폭탄이 나갈 위치
    [SerializeField] private float angle = 45f;         // 포신의 현재 각도
    [SerializeField] private float angleStep = 0.5f;      // 각도 변화량

    public float powerCharge = 0f;         // 차지
    private bool isCharging = false;        // 차지 중인지 여부
    [SerializeField] private float chargingSpeed = 10f;    // 차지속도

    [SerializeField] public float maxPower = 20f;         // 폭탄 발사 속도
    [Header("이동 설정")]
    public float moveSpeed = 5f;

    [Header("발사체 설정")]
    public GameObject projectilePrefab;


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
        Flip();
        Aim();
   
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
            Fire();

            powerCharge = 0f;
            isCharging = false;
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
    void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile Prefab이 할당되지 않았습니다.");
            return;
        }


        GameObject projectile = Instantiate(projectilePrefab, firePoint.transform.position, Quaternion.identity);
        CameraController.Instance.FollowBullet(projectile.transform); // 이 부분은 CameraController가 있어야 작동합니다.
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            // 각도를 사용해 발사 방향 벡터를 계산합니다.
            float angleInRadians = angle * Mathf.Deg2Rad;
            Vector2 launchDirection = isFacingRight? new Vector2(Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians))
                : new Vector2(-Mathf.Cos(angleInRadians), Mathf.Sin(angleInRadians));
            projectileRb.velocity = launchDirection * powerCharge;
        }
        else
        {
            Debug.LogWarning("투사체 프리팹에 Rigidbody2D 컴포넌트가 없습니다.");
        }

        //발사 이펙트 재생
        Vector3 effectSpawnPos = firePoint.position;
        Quaternion effectSpawnRot = isFacingRight ? Quaternion.Euler(0, 0, angle) : Quaternion.Euler(0, 0, -angle);
        EffectSpawner.Instance.SpawnFire(effectSpawnPos, effectSpawnRot);
    }
    bool isFacingRight = true;
    void Flip()
    {
        if (horizontalInput > 0 && !isFacingRight)
        {
            isFacingRight = true;
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (horizontalInput < 0 && isFacingRight)
        {
            isFacingRight = false;
            Vector3 scale = transform.localScale;
            scale.x = -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}