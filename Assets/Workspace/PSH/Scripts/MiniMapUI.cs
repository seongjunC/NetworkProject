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
        miniMapRaw.texture = terrain.deformableTexture;
        // 1) 플레이어 월드 좌표 → 0~1 정규화
        Vector3 p = player.position;
        float u = (p.x - terrain.transform.position.x) / worldWidth;
        float v = (p.y - terrain.transform.position.y) / worldHeight;

        // 2) RawImage 크기
        var r = miniMapRaw.rectTransform.rect;
        // 로컬 좌표계: 왼쪽 아래 (0,0), 오른쪽 위 (width, height)
        float px = Mathf.Clamp01(u) * r.width;
        float py = Mathf.Clamp01(v) * r.height;

        // 3) 아이콘 위치 업데이트 (왼쪽 아래 기준)
        playerIcon.anchoredPosition = new Vector2(px, py);
    }
}
