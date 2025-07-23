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

    private void Start()
    {
        terrain = FindObjectOfType<DeformableTerrain>();

        // Terrain의 스프라이트/텍스처 기준으로 계산
        var sr = terrain.GetComponent<SpriteRenderer>();
        var tex = sr.sprite.texture;
        worldPerPixel = sr.bounds.size.x / tex.width;

        Debug.Log($"worldPerPixel = {worldPerPixel}");
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

        Destroy(gameObject);
    }

    public void DetectPlayerInCircle(Vector2 centerWorld, float radiusWorld)
    {
        var colliders = Physics2D.OverlapCircleAll(centerWorld, radiusWorld);

        foreach (var hit in colliders)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log($"플레이어에게 {damage} 데미지");
            }
        }
    }
}
