using UnityEngine;

public class DeformableTerrain : MonoBehaviour
{
    private Texture2D originalTexture;
    private Texture2D deformableTexture;
    private SpriteRenderer spriteRenderer;
    private PolygonCollider2D polygonCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        polygonCollider = GetComponent<PolygonCollider2D>();

        // 원본 텍스처를 복사
        originalTexture = spriteRenderer.sprite.texture;
        deformableTexture = new Texture2D(originalTexture.width, originalTexture.height);
        deformableTexture.SetPixels(originalTexture.GetPixels());
        deformableTexture.Apply();

        // 새로운 스프라이트를 생성
        Sprite newSprite = Sprite.Create(deformableTexture, spriteRenderer.sprite.rect, new Vector2(0.5f, 0.5f));
        spriteRenderer.sprite = newSprite;
    }


    public void DestroyTerrain(Vector2 point, int radius)
    {
        // 월드 좌표를 텍스처의 픽셀 좌표로 변환
        Vector2Int pixelCoord = WorldToPixel(point);

        int px = pixelCoord.x;
        int py = pixelCoord.y;

        // 해당 지점 주변의 픽셀을 원형으로 순회하며 투명하게 만듬
        for (int x = px - radius; x < px + radius; x++)
        {
            for (int y = py - radius; y < py + radius; y++)
            {
                if (Mathf.Pow(x - px, 2) + Mathf.Pow(y - py, 2) < Mathf.Pow(radius, 2))
                {
                    if (x > 0 && x < deformableTexture.width && y > 0 && y < deformableTexture.height)
                    {
                        deformableTexture.SetPixel(x, y, Color.clear);
                    }
                }
            }
        }

        deformableTexture.Apply();

        UpdateCollider();
    }
    public void DestroyTerrain(Vector2 point, int radiusx, int radiusy)
    {
        // 월드 좌표를 텍스처의 픽셀 좌표로 변환
        Vector2Int pixelCoord = WorldToPixel(point);

        int px = pixelCoord.x;
        int py = pixelCoord.y;

        // 해당 지점 주변의 픽셀을 타원형으로 순회하며 투명하게 만듬
        for (int x = px - radiusx; x < px + radiusx; x++)
        {
            for (int y = py - radiusy; y < py + radiusy; y++)
            {
                if (Mathf.Pow(radiusy, 2)*Mathf.Pow(x - px, 2) + Mathf.Pow(radiusx, 2)* Mathf.Pow(y - py, 2) 
                    < Mathf.Pow(radiusx, 2)* Mathf.Pow(radiusy, 2))
                {
                    if (x > 0 && x < deformableTexture.width && y > 0 && y < deformableTexture.height)
                    {
                        deformableTexture.SetPixel(x, y, Color.clear);
                    }
                }
            }
        }

        deformableTexture.Apply();
        UpdateCollider();
    }
    public void DestroyTerrain(Vector2 worldPoint, Texture2D explosionMask, float scale = 1)
    {
        if (explosionMask == null)
        {
            Debug.LogError("Explosion Mask is null!");
            return;
        }

        Vector2Int centerPixel = WorldToPixel(worldPoint);

        float effectiveMaskWidth = explosionMask.width * scale;
        float effectiveMaskHeight = explosionMask.height * scale;

        int minTargetX = Mathf.RoundToInt(centerPixel.x - effectiveMaskWidth / 2f);
        int maxTargetX = Mathf.RoundToInt(centerPixel.x + effectiveMaskWidth / 2f);
        int minTargetY = Mathf.RoundToInt(centerPixel.y - effectiveMaskHeight / 2f);
        int maxTargetY = Mathf.RoundToInt(centerPixel.y + effectiveMaskHeight / 2f);

        minTargetX = Mathf.Max(0, minTargetX);
        maxTargetX = Mathf.Min(deformableTexture.width, maxTargetX);
        minTargetY = Mathf.Max(0, minTargetY);
        maxTargetY = Mathf.Min(deformableTexture.height, maxTargetY);

        for (int targetX = minTargetX; targetX < maxTargetX; targetX++)
        {
            for (int targetY = minTargetY; targetY < maxTargetY; targetY++)
            {
                float maskRelativeX = (targetX - centerPixel.x) / scale;
                float maskRelativeY = (targetY - centerPixel.y) / scale;

                int maskX = Mathf.RoundToInt(maskRelativeX + explosionMask.width / 2f);
                int maskY = Mathf.RoundToInt(maskRelativeY + explosionMask.height / 2f);

                if (maskX >= 0 && maskX < explosionMask.width &&
                    maskY >= 0 && maskY < explosionMask.height)
                {
                    Color maskPixel = explosionMask.GetPixel(maskX, maskY);

                    if (maskPixel.a > 0.1f)
                    {
                        deformableTexture.SetPixel(targetX, targetY, Color.clear);
                    }
                }
            }
        }

        deformableTexture.Apply();
        UpdateCollider();
    }

    void UpdateCollider()
    {
        // 기존 콜라이더를 파괴하고 새로 생성
        Destroy(polygonCollider);
        polygonCollider = gameObject.AddComponent<PolygonCollider2D>();
    }

    private Vector2Int WorldToPixel(Vector2 worldPoint)
    {
        Vector2 localPoint = transform.InverseTransformPoint(worldPoint);
        float pixelsPerUnit = spriteRenderer.sprite.pixelsPerUnit;

        float halfWidth = deformableTexture.width / 2f;
        float halfHeight = deformableTexture.height / 2f;

        int x = Mathf.RoundToInt(localPoint.x * pixelsPerUnit + halfWidth);
        int y = Mathf.RoundToInt(localPoint.y * pixelsPerUnit + halfHeight);

        return new Vector2Int(x, y);
    }
}