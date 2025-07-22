using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int explosionRadius = 10; // ÇÈ¼¿ ´ÜÀ§ ¹Ý°æ
    public Texture2D explosionMask;
    public float explosionScale = 1;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 explosionPoint = collision.contacts[0].point;
        DeformableTerrain terrain = collision.gameObject.GetComponent<DeformableTerrain>();
        if (terrain != null)
        {
            if (explosionMask == null)
            {
                terrain.DestroyTerrain(explosionPoint, explosionRadius);
            }
            else
            {
                terrain.DestroyTerrain(explosionPoint, explosionMask, explosionScale);
            }

        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("ÇÃ·¹ÀÌ¾î¿ÍºÎµúÈû");
            DeformableTerrain activeTerrain = FindObjectOfType<DeformableTerrain>();
            if (activeTerrain != null)
            {
                Debug.Log("Å×¸®¾ÈÃ£À½");
                if (explosionMask == null)
                {
                    activeTerrain.DestroyTerrain(explosionPoint, explosionRadius);
                }
                else
                {
                    activeTerrain.DestroyTerrain(explosionPoint, explosionMask, explosionScale);
                }
            }
        }
        // Æ÷Åº ÆÄ±«
        Destroy(gameObject);
    }
}
