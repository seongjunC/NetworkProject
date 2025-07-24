using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int explosionRadiusx = 10; // �ؽ�ó �ȼ� ����
    public int explosionRadiusy = 10;
    public Texture2D explosionMask;
    public float explosionScale = 1f;
    public float damage = 100f;

    private float worldPerPixel; // Terrain ����
    private DeformableTerrain terrain;

    private bool hasCollided = false;

    [SerializeField] GameObject explosionEffect;
    [SerializeField] float delay = 2f;

    private Rigidbody2D rb;

    [Header("�����")]
    private Vector2 gizmoCenter;
    private float gizmoRadius;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("Projectile�� Rigidbody2D ������Ʈ�� �����ϴ�!");
        }
    }

    private void Start()
    {
        terrain = FindObjectOfType<DeformableTerrain>();

        // Terrain�� ��������Ʈ/�ؽ�ó �������� ���
        var sr = terrain.GetComponent<SpriteRenderer>();
        var tex = sr.sprite.texture;
        worldPerPixel = sr.bounds.size.x / tex.width;

        Debug.Log($"worldPerPixel = {worldPerPixel}");
    }

    private void Update()
    {
        // �ӵ��� 0�� ����� ������ ���� ���� ���� ������ ������Ʈ�մϴ�.
        if (rb != null && rb.velocity.sqrMagnitude > 0.01f)
        {
            // �ӵ� ������ ������ ������ ��ȯ�մϴ�.
            // Atan2�� y, x ������ ���ڸ� �޽��ϴ�.
            float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

            // Z���� �������� ȸ���ϴ� ���ʹϾ��� �����մϴ�.
            // �� �ڵ�� ��������Ʈ�� �⺻������ ������(��)�� ���ϰ� ���� ���� �����մϴ�.
            // ���� ��������Ʈ�� ����(��)�� ���ϰ� �ִٸ� 'angle - 90f'�� �����ؾ� �մϴ�.
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {


        Vector2 explosionPoint = collision.contacts[0].point;

        // Terrain �ı�
        if (explosionMask == null)
            terrain.DestroyTerrain(explosionPoint, explosionRadiusx, explosionRadiusy);
        else
            terrain.DestroyTerrain(explosionPoint, explosionMask, explosionScale);

        // ������ ó��
        if (collision.gameObject.CompareTag("Player"))
        {
            // ����: 100%
            Debug.Log($"�÷��̾�� {damage} ������ (����)");
            // player.TakeDamage(maxDamage);
        }
        else
        {
            // �ֺ� ����
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
        //����ü ��Ȱ��ȭ
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().simulated = false;

        //��������Ʈ
        if (explosionEffect != null && hasExplosionEffect)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);


        //������ ī�޶� ����
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
                Debug.Log($"�÷��̾�� {damage} ������");
            }
        }
    }
    private void OnDrawGizmos()
    {
        // 1) ���� ����
        Gizmos.color = Color.red;

        // 2) 2D�� �� �׸���
        Gizmos.DrawWireSphere(gizmoCenter, gizmoRadius);
    }
}
