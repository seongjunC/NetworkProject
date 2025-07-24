using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int explosionRadiusx = 10; // 텍스처 픽셀 단위
    public int explosionRadiusy = 10;
    public Texture2D explosionMask;
    public float explosionScale = 1f;
    public float damage = 100f;

    private float worldPerPixel; // Terrain 기준
    private DeformableTerrain terrain;

    private bool hasCollided = false;

    [SerializeField] GameObject explosionEffect;
    [SerializeField] float delay = 2f;

    private Rigidbody2D rb;

    [Header("기즈모")]
    private Vector2 gizmoCenter;
    private float gizmoRadius;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Projectile에 Rigidbody2D 컴포넌트가 없습니다!");
        }
    }

    private void Start()
    {
        terrain = FindObjectOfType<DeformableTerrain>();

        // Terrain의 스프라이트/텍스처 기준으로 계산
        var sr = terrain.GetComponent<SpriteRenderer>();
        var tex = sr.sprite.texture;
        worldPerPixel = sr.bounds.size.x / tex.width;

        Debug.Log($"worldPerPixel = {worldPerPixel}");
    }

    private void Update()
    {
        // 속도가 0에 가까울 정도로 작지 않을 때만 방향을 업데이트합니다.
        if (rb != null && rb.velocity.sqrMagnitude > 0.01f)
        {
            // 속도 벡터의 방향을 각도로 변환합니다.
            // Atan2는 y, x 순서로 인자를 받습니다.
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

            // Z축을 기준으로 회전하는 쿼터니언을 생성합니다.
            // 이 코드는 스프라이트가 기본적으로 오른쪽(→)을 향하고 있을 때를 가정합니다.
            // 만약 스프라이트가 위쪽(↑)을 향하고 있다면 'angle - 90f'로 수정해야 합니다.
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {


        Vector2 explosionPoint = collision.contacts[0].point;

        // Terrain 파괴
        if (explosionMask == null)
            terrain.DestroyTerrain(explosionPoint, explosionRadiusx, explosionRadiusy);
        else
            terrain.DestroyTerrain(explosionPoint, explosionMask, explosionScale);

        // 데미지 처리
        if (collision.gameObject.CompareTag("Player"))
        {
            // 직격: 100%
            Debug.Log($"플레이어에게 {damage} 데미지 (직격)");
            // player.TakeDamage(maxDamage);
        }
        else
        {
            // 주변 피해
            float pixelRadius = Mathf.Max(explosionRadiusx, explosionRadiusy);
            float worldRadius = pixelRadius * worldPerPixel;

            DetectPlayerInCircle(explosionPoint, worldRadius);
        }

        BeginDestroyRoutine(true);
    }
    public void BeginDestroyRoutine(bool hasExplosionEffect)
    {
        if (hasCollided) return;
        hasCollided = true;
        StartCoroutine(DestroyRoutine(hasExplosionEffect));
    }
    private IEnumerator DestroyRoutine(bool hasExplosionEffect)
    {
        //투사체 비활성화
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        //폭발이펙트
        if (explosionEffect != null && hasExplosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);


        //몇초후 카메라 무브
        yield return new WaitForSeconds(delay);

        if (CameraController.Instance != null)
            CameraController.Instance.ReturnToPlayerCam();   

        Destroy(gameObject);
    }

    public void DetectPlayerInCircle(Vector2 centerWorld, float radiusWorld)
    {
        gizmoCenter = centerWorld;
        gizmoRadius = radiusWorld;

        var colliders = Physics2D.OverlapCircleAll(centerWorld, radiusWorld);

        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"플레이어에게 {damage} 데미지");
            }
        }
    }
    private void OnDrawGizmos()
    {
        // 1) 색상 설정
        Gizmos.color = Color.red;

        // 2) 2D용 원 그리기
        Gizmos.DrawWireSphere(gizmoCenter, gizmoRadius);
    }
}
