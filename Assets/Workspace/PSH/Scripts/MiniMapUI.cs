using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapUI : MonoBehaviour
{
    [Header("References")]
    public DeformableTerrain terrain;   // 땅을 변형하는 스크립트
    public RawImage miniMapRaw;   // 위에서 만든 RawImage
    public RectTransform playerIcon;   // 원형 아이콘(RectTransform)
    public Transform player;       // 플레이어 트랜스폼

    [Header("Settings")]
    public float worldWidth;   // 지형이 월드에서 차지하는 가로 크기
    public float worldHeight;  // 지형이 월드에서 차지하는 세로 크기

    private IEnumerator Start()
    {
        yield return null;
        yield return null;

        terrain = FindObjectOfType<DeformableTerrain>();
        miniMapRaw.texture = terrain.deformableTexture;
        miniMapRaw.uvRect = new Rect(0, 0, 1, 1);
    }

    void LateUpdate()
    {
        // 플레이어 월드 좌표
        Vector3 wp = player.position;

        // 맵 월드 경계 (terrain이나 tilemap.bounds로부터)
        Vector3 min = terrain.GetComponent<SpriteRenderer>().bounds.min;
        Vector3 size = terrain.GetComponent<SpriteRenderer>().bounds.size;
        float u = (wp.x - min.x) / size.x;
        float v = (wp.y - min.y) / size.y;

        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);

        // RawImage 크기
        Rect rect = miniMapRaw.rectTransform.rect;
        float px = u * rect.width;
        float py = v * rect.height;

        playerIcon.anchoredPosition = new Vector2(px, py);
    }
}
