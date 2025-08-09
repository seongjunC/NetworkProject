using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(PolygonCollider2D))]
public class MapBoundary : MonoBehaviour
{
    public SpriteRenderer mapSpriteRenderer;

    public float padding = 2f;
    public float paddingUp = 10f;  //위로 쐈을 때는 맵 영역을 벗어났다고 생각하면 안됨

    private BoxCollider2D boxCollider;
    private PolygonCollider2D polygonCollider;

    public Vector2 mapSize;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();

        polygonCollider.isTrigger = true;
    }

    private void OnEnable()
    {
        MapManager.OnMapLoaded += HandleMapLoaded;
    }
    private void OnDisable()
    {
        MapManager.OnMapLoaded -= HandleMapLoaded;
    }
    private void HandleMapLoaded(DeformableTerrain terrain)
    {
        if (terrain != null)
        {
            mapSpriteRenderer = terrain.GetComponent<SpriteRenderer>();

            if (mapSpriteRenderer == null)
            {
                Debug.Log("spriterenderer 없음");
                return;
            }

            ResizeCollider();
        }
        else
        {
            Debug.Log("deformableterrian 없음");
        }
    }
   /* private void OnTriggerExit2D(Collider2D collision)
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
    }*/

    public void ResizeCollider()
    {
        transform.position = mapSpriteRenderer.transform.position ;
        mapSize = mapSpriteRenderer.bounds.size;
        boxCollider.size = mapSize + new Vector2(padding,padding + 2 * paddingUp);
        boxCollider.offset = new Vector2(0, paddingUp);
        Debug.Log("맵 크기에 맞게 트리거 영역 설정 완료");

        Vector2 halfSize = boxCollider.size / 2f;
        Vector2 offset = boxCollider.offset;

        Vector2[] points = new Vector2[4];
        points[0] = new Vector2(-halfSize.x, -halfSize.y) + offset; // 왼쪽 아래
        points[1] = new Vector2(-halfSize.x, halfSize.y) + offset; // 왼쪽 위
        points[2] = new Vector2(halfSize.x, halfSize.y) + offset; // 오른쪽 위
        points[3] = new Vector2(halfSize.x, -halfSize.y) + offset; // 오른쪽 아래

        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, points);
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
