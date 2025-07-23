using System.Collections.Generic;
using UnityEngine;

// ���� �ݶ��̴��� �޽ø� ������ �����ϴ� ��ũ��Ʈ
[ExecuteAlways]
public class Land : MonoBehaviour
{
    [SerializeField] PolygonCollider2D _collider;       // ���� �ݶ��̴� ����
    [SerializeField] MeshFilter _meshFilter;            // �޽� �������� �޽�����
    [SerializeField] ColliderRenderer _colliderRenderer; // �޽� ���� ��� ��ũ��Ʈ

    // �ܺο��� ��θ� �޾� �ݶ��̴� �н� �� �޽ø� ������ϴ� �Լ�
    public void SetPath(List<List<Point>> paths)
    {
        _collider.pathCount = paths.Count;

        // �� ��θ� PolygonCollider2D�� ����
        for (int i = 0; i < paths.Count; i++)
        {
            List<Vector2> path = new List<Vector2>();
            for (int p = 0; p < paths[i].Count; p++)
            {
                path.Add(paths[i][p].Position);
            }
            _collider.SetPath(i, path);
        }

        // �޽� ���� ��û
        _colliderRenderer.CreateMesh();
    }
}
