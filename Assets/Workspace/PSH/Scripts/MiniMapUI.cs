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
        miniMapRaw.texture = terrain.deformableTexture;
        // 1) �÷��̾� ���� ��ǥ �� 0~1 ����ȭ
        Vector3 p = player.position;
        float u = (p.x - terrain.transform.position.x) / worldWidth;
        float v = (p.y - terrain.transform.position.y) / worldHeight;

        // 2) RawImage ũ��
        var r = miniMapRaw.rectTransform.rect;
        // ���� ��ǥ��: ���� �Ʒ� (0,0), ������ �� (width, height)
        float px = Mathf.Clamp01(u) * r.width;
        float py = Mathf.Clamp01(v) * r.height;

        // 3) ������ ��ġ ������Ʈ (���� �Ʒ� ����)
        playerIcon.anchoredPosition = new Vector2(px, py);
    }
}
