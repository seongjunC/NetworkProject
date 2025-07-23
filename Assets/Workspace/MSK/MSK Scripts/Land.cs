using System.Collections.Generic;
using UnityEngine;

// 지형 콜라이더와 메시를 연결해 관리하는 스크립트
[ExecuteAlways]
public class Land : MonoBehaviour
{
    [SerializeField] PolygonCollider2D _collider;       // 지형 콜라이더 참조
    [SerializeField] MeshFilter _meshFilter;            // 메시 렌더러용 메쉬필터
    [SerializeField] ColliderRenderer _colliderRenderer; // 메시 생성 담당 스크립트

    // 외부에서 경로를 받아 콜라이더 패스 및 메시를 재생성하는 함수
    public void SetPath(List<List<Point>> paths)
    {
        _collider.pathCount = paths.Count;

        // 각 경로를 PolygonCollider2D에 설정
        for (int i = 0; i < paths.Count; i++)
        {
            List<Vector2> path = new List<Vector2>();
            for (int p = 0; p < paths[i].Count; p++)
            {
                path.Add(paths[i][p].Position);
            }
            _collider.SetPath(i, path);
        }

        // 메시 생성 요청
        _colliderRenderer.CreateMesh();
    }
}
