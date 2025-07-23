using UnityEditor;
using UnityEngine;

// 콜라이더 데이터를 기반으로 메쉬를 생성해 MeshFilter에 할당하는 역할
[ExecuteAlways]
public class ColliderRenderer : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D _collider;   // 참조할 콜라이더
    [SerializeField] private MeshFilter _meshFilter;        // 메시 렌더링용 필터

    // 트랜스폼이 변경되면 메시 다시 생성
    private void Update()
    {
        if (transform.hasChanged)
        {
            CreateMesh();
            transform.hasChanged = false;
        }
    }

    // 에디터에서 값 변경 시 메시 생성
    private void OnValidate()
    {
        CreateMesh();
    }

    // 콜라이더 경로로부터 메시 생성
    public void CreateMesh()
    {
        Mesh mesh = _collider.CreateMesh(false, false);
        _meshFilter.mesh = mesh;
    }

    // 씬 뷰에서 콜라이더 점 인덱스 표시
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
