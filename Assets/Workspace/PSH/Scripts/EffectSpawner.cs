using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    public static EffectSpawner Instance { get; private set; }

    public GameObject explosionPrefab;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void SpawnExplosion(Vector3 position)
    {
        GameObject explosionEffect = Instantiate(explosionPrefab, position, Quaternion.identity);

        ExplosionController controller = explosionEffect.GetComponent<ExplosionController>();

        controller.TriggerHit();
    }
    void SpawnFire(Vector3 position, Quaternion rotation)
    {
        GameObject explosionEffect = Instantiate(explosionPrefab, position, rotation);

        ExplosionController controller = explosionEffect.GetComponent<ExplosionController>();

        controller.TriggerFire();
    }
}
