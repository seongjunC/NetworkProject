using UnityEditor;
using UnityEngine;

// �ݶ��̴� �����͸� ������� �޽��� ������ MeshFilter�� �Ҵ��ϴ� ����
[ExecuteAlways]
public class ColliderRenderer : MonoBehaviour
{
    [SerializeField] PolygonCollider2D _collider;   // ������ �ݶ��̴�
    [SerializeField] MeshFilter _meshFilter;        // �޽� �������� ����

    // Ʈ�������� ����Ǹ� �޽� �ٽ� ����
    private void Update()
    {
        if (transform.hasChanged)
        {
            CreateMesh();
            transform.hasChanged = false;
        }
    }

    // �����Ϳ��� �� ���� �� �޽� ����
    private void OnValidate()
    {
        CreateMesh();
    }

    // �ݶ��̴� ��ηκ��� �޽� ����
    public void CreateMesh()
    {
        Mesh mesh = _collider.CreateMesh(true, true);
        _meshFilter.mesh = mesh;
    }

    // �� �信�� �ݶ��̴� �� �ε��� ǥ��
    private void OnDrawGizmos()
    {
        for (int p = 0; p < _collider.pathCount; p++)
        {
            for (int i = 0; i < _collider.GetPath(p).Length; i++)
            {
                Handles.Label(_collider.transform.TransformPoint(_collider.GetPath(p)[i]), i.ToString());
            }
        }
    }
}
