using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int explosionRadius = 10; // �ȼ� ���� �ݰ�
    public Texture2D explosionMask;
    public float explosionScale = 1;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // DeformableTerrain ������Ʈ�� ���� ������Ʈ�� �浹�ߴ��� Ȯ��
        DeformableTerrain terrain = collision.gameObject.GetComponent<DeformableTerrain>();
        if (terrain != null)
        {
            // �浹 ����(contacts[0].point)�� �ݰ��� �Ѱ� ���� �ı� �Լ� ȣ��
            if (explosionMask == null)
            {
                terrain.DestroyTerrain(collision.contacts[0].point, explosionRadius);
            }
            else
            {
                terrain.DestroyTerrain(collision.contacts[0].point, explosionMask, explosionScale);
            }

            // ��ź �ı�
            Destroy(gameObject);
        }
    }
}
