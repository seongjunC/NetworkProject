using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniMapUI : MonoBehaviour
{
    [Header("References")]
    public DeformableTerrain terrain;   // ���� �����ϴ� ��ũ��Ʈ
    public RawImage miniMapRaw;   // ������ ���� RawImage
    public RectTransform playerIcon;   // ���� ������(RectTransform)
    public Transform player;       // �÷��̾� Ʈ������

    [Header("Settings")]
    public float worldWidth;   // ������ ���忡�� �����ϴ� ���� ũ��
    public float worldHeight;  // ������ ���忡�� �����ϴ� ���� ũ��

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
        // �÷��̾� ���� ��ǥ
        Vector3 wp = player.position;

        // �� ���� ��� (terrain�̳� tilemap.bounds�κ���)
        Vector3 min = terrain.GetComponent<SpriteRenderer>().bounds.min;
        Vector3 size = terrain.GetComponent<SpriteRenderer>().bounds.size;
        float u = (wp.x - min.x) / size.x;
        float v = (wp.y - min.y) / size.y;

        u = Mathf.Clamp01(u);
        v = Mathf.Clamp01(v);

        // RawImage ũ��
        Rect rect = miniMapRaw.rectTransform.rect;
        float px = u * rect.width;
        float py = v * rect.height;

        playerIcon.anchoredPosition = new Vector2(px, py);
    }
}
