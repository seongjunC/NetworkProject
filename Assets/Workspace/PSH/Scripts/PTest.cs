using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTest : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private float horizontalMove = 0f;

    public GameObject projectilePrefab;
    public float projectileSpeed = 10f;
    public Transform muzzlePoint;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire();
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalMove, rb.velocity.y);
    }

    void Fire()
    {
        if (projectilePrefab == null)
        {
            Debug.LogWarning("Projectile Prefab�� �Ҵ���� �ʾҽ��ϴ�.");
            return;
        }

        GameObject projectile = Instantiate(projectilePrefab, muzzlePoint.transform.position, Quaternion.identity);
        CameraController.Instance.FollowBullet(projectile.transform);
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();

        if (projectileRb != null)
        {
            Vector2 launchDirection = new Vector2(.5f, .5f);
            projectileRb.velocity = launchDirection * projectileSpeed;
        }
        else
        {
            Debug.LogWarning("����ü �����տ� Rigidbody2D ������Ʈ�� �����ϴ�.");
        }
    }
}
