using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBoundary : MonoBehaviour
{
    public SpriteRenderer mapSpriteRenderer;

    public float padding = 2f;

    private BoxCollider2D boxCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

    }
    private void Start()
    {
        DeformableTerrain terrain = FindObjectOfType<DeformableTerrain>();
        if (terrain != null)
        {
            mapSpriteRenderer = terrain.GetComponent<SpriteRenderer>();

            if (mapSpriteRenderer == null)
            {
                Debug.Log("spriterenderer 없음");
                return;
            }
        }
        else
        {
            Debug.Log("deformableterrian 없음");
        }

        ResizeCollider();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        //플레이어가 맵 밖을 나갈 경우
        if (collision.CompareTag("Player"))
        {
            //플레이어 낙사 판정
        }
        //투사체가 맵 밖을 나갈 경우
        else if (collision.CompareTag("Bullet"))
        {
            Projectile p = collision.GetComponent<Projectile>();

            if (p != null)
            {
                p.BeginDestroyRoutine(false);
                Debug.Log("투사체가 맵 밖으로 나감");
            }

        }
    }

    public void ResizeCollider()
    {
        transform.position = mapSpriteRenderer.transform.position;
        Vector2 mapSize = mapSpriteRenderer.bounds.size;
        boxCollider.size = mapSize + new Vector2(padding,padding);
        boxCollider.offset = Vector2.zero;
        Debug.Log("맵 크기에 맞게 트리거 영역 설정 완료");
    }

}
