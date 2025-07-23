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

    private void Start()
    {
        terrain = FindObjectOfType<DeformableTerrain>();

        // Terrain�� ��������Ʈ/�ؽ�ó �������� ���
        var sr = terrain.GetComponent<SpriteRenderer>();
        var tex = sr.sprite.texture;
        worldPerPixel = sr.bounds.size.x / tex.width;

        Debug.Log($"worldPerPixel = {worldPerPixel}");
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

        Destroy(gameObject);
    }

    public void DetectPlayerInCircle(Vector2 centerWorld, float radiusWorld)
    {
        var colliders = Physics2D.OverlapCircleAll(centerWorld, radiusWorld);

        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"�÷��̾�� {damage} ������");
            }
        }
    }
}
