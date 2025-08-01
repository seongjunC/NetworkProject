using UnityEngine;

public class EffectSpawner : MonoBehaviour
{
    public GameObject explosionPrefab;



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            SpawnExplosion(new Vector3(0, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            SpawnFire(new Vector3(0, 0, 0));
        }
    }

    void SpawnExplosion(Vector3 position)
    {
        GameObject explosionEffect = Instantiate(explosionPrefab, position, Quaternion.identity);

        ExplosionController controller = explosionEffect.GetComponent<ExplosionController>();

        controller.TriggerHit();
    }
    void SpawnFire(Vector3 position)
    {
        GameObject explosionEffect = Instantiate(explosionPrefab, position, Quaternion.identity);

        ExplosionController controller = explosionEffect.GetComponent<ExplosionController>();

        controller.TriggerFire();
    }
}
