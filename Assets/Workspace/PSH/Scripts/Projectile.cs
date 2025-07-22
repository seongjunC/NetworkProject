using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public int explosionRadius = 10; // 픽셀 단위 반경
    public Texture2D explosionMask;
    public float explosionScale = 1;

    void OnCollisionEnter2D(Collision2D collision)
    {
        // DeformableTerrain 컴포넌트를 가진 오브젝트와 충돌했는지 확인
        DeformableTerrain terrain = collision.gameObject.GetComponent<DeformableTerrain>();
        if (terrain != null)
        {
            // 충돌 지점(contacts[0].point)과 반경을 넘겨 지형 파괴 함수 호출
            if (explosionMask == null)
            {
                terrain.DestroyTerrain(collision.contacts[0].point, explosionRadius);
            }
            else
            {
                terrain.DestroyTerrain(collision.contacts[0].point, explosionMask, explosionScale);
            }

            // 포탄 파괴
            Destroy(gameObject);
        }
    }
}
