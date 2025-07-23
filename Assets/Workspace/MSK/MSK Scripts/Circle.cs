using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class Circle : MonoBehaviour
{
    [SerializeField] ColliderRenderer _colliderRenderer;  // 메시 생성 스크립트 참조
    [SerializeField] int _sides;                           // 다각형 변 개수
    [SerializeField] PolygonCollider2D _collider;          // 원형 콜라이더 참조

    private void Update()
    {
        if (Application.isPlaying)
        {
            // 마우스 스크롤로 변 개수 조절
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
            {
                _sides += (int)scroll;
                CreateCircle();
            }

            // +/- 키로 스케일 조절
            if (Input.GetKeyDown(KeyCode.Equals))
            {
                transform.localScale *= 1.1f;
            }
            if (Input.GetKeyDown(KeyCode.Minus))
            {
                transform.localScale /= 1.1f;
            }
        }
    }

    private void OnValidate()
    {
        CreateCircle();   // 에디터에서 값 바뀌면 원 재생성
    }

    // 콜라이더를 다각형 원형으로 생성하고 메시 갱신
    void CreateCircle()
    {
        _collider.CreatePrimitive(_sides, Vector2.one);
        _colliderRenderer.CreateMesh();
    }
}