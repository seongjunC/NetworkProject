using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class Circle : MonoBehaviour
{
    [SerializeField] ColliderRenderer _colliderRenderer;  // �޽� ���� ��ũ��Ʈ ����
    [SerializeField] int _sides;                           // �ٰ��� �� ����
    [SerializeField] PolygonCollider2D _collider;          // ���� �ݶ��̴� ����

    private void Update()
    {
        if (Application.isPlaying)
        {
            // ���콺 ��ũ�ѷ� �� ���� ����
            float scroll = Input.mouseScrollDelta.y;
            if (scroll != 0)
            {
                _sides += (int)scroll;
                CreateCircle();
            }

            // +/- Ű�� ������ ����
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
        CreateCircle();   // �����Ϳ��� �� �ٲ�� �� �����
    }

    // �ݶ��̴��� �ٰ��� �������� �����ϰ� �޽� ����
    void CreateCircle()
    {
        _collider.CreatePrimitive(_sides, Vector2.one);
        _colliderRenderer.CreateMesh();
    }
}