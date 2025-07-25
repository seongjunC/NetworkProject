using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapUI : MonoBehaviour
{
    [Header("References")]
    public DeformableTerrain terrain;  
    public RawImage miniMapRaw;  
    public RectTransform playerIcon;   
    public Transform player;       

    Vector3 min;
    Vector3 size;
    private IEnumerator Start()
    {
        yield return null;
        yield return null;

        terrain = FindObjectOfType<DeformableTerrain>();
        miniMapRaw.texture = terrain.deformableTexture;
        miniMapRaw.uvRect = new Rect(0, 0, 1, 1);

        min = terrain.GetComponent<SpriteRenderer>().bounds.min;
        size = terrain.GetComponent<SpriteRenderer>().bounds.size;
    }

    void LateUpdate()
    {
        // 플레이어 월드 좌표
        Vector3 wp = player.position;

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
