using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyMap : MonoBehaviour
{
    public SpriteRenderer mapSpriteRenderer;

    public float padding = 2f;

    private BoxCollider2D boxCollider;
    private void Start() { }
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();

    }
    private void OnEnable()
    {
        MapManager.OnMapLoaded += OnMapGenerated;
    }

    private void OnDisable()
    {
        MapManager.OnMapLoaded -= OnMapGenerated;
    }

    private void OnMapGenerated(DeformableTerrain terrain)
    {
        terrain = FindObjectOfType<DeformableTerrain>();
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
            //큰 데미지를 줘서 낙사
            var p = collision.GetComponent<PlayerController>();
            p.OnHit(9999999);
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
        boxCollider.size = mapSize + new Vector2(padding, padding);
        boxCollider.offset = Vector2.zero;
        Debug.Log("맵 크기에 맞게 트리거 영역 설정 완료");
    }
    private void OnDrawGizmos()
    {
        if (mapSpriteRenderer == null)
            return;

        // 컬라이더 크기/오프셋이 최신이라면, Awake/Start에서 ResizeCollider()를 한 번 호출해두세요
        if (boxCollider == null)
            boxCollider = GetComponent<BoxCollider2D>();

        // Gizmo 색상
        Gizmos.color = Color.green;

        // 월드 위치 계산 (Transform 위치 + 콜라이더 offset)
        Vector2 center = (Vector2)transform.position + boxCollider.offset;
        Vector2 size = boxCollider.size;

        // 2D 박스 그리기
        Gizmos.DrawWireCube(center, size);
    }
}
