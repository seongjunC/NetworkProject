using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// 교차점 정보를 저장하는 구조체
[System.Serializable]
public struct IntersectionPoint
{
    public int LandIndex;       // 지형 경로에서의 교차점 인덱스
    public int SphIndex;        // 구체 경로에서의 교차점 인덱스
    public Vector2 Position;    // 교차 지점 좌표
    public bool CCW;            // 반시계 방향 여부

    public IntersectionPoint(int landIndex, int sphIndex, Vector2 position, bool ccw)
    {
        LandIndex = landIndex;
        SphIndex = sphIndex;
        Position = position;
        CCW = ccw;
    }
}

// 지형과 충돌하는 원형 영역(Sph)을 처리하고, 충돌 경로 기반으로 지형을 수정하는 클래스
public class CircleCutArea : MonoBehaviour
{
    [SerializeField] MeshFilter _meshFilter;
    [SerializeField] int _sides; // 원형 메쉬의 면 개수

    [SerializeField] PolygonCollider2D _sphCollider;   // 구체(원형) 콜라이더
    [SerializeField] PolygonCollider2D _landCollider;  // 지형 콜라이더

    [SerializeField] List<IntersectionPoint> IntersectionPointsList = new List<IntersectionPoint>();

    private void Start()
    {
        CreateMesh();
    }

    private void OnValidate()
    {
        CreateMesh();
    }

    private void Update()
    {
        CreateMesh();

        // 우클릭 시 교차점 탐색
        if (Input.GetMouseButtonDown(1))
        {
            FindIntersections();
        }
    }

    // 원형 콜라이더 기반의 메쉬를 생성하고 MeshFilter에 적용
    [ContextMenu("CreateMesh")]
    private void CreateMesh()
    {
        _sphCollider.CreatePrimitive(_sides, Vector2.one);
        Mesh mesh = _sphCollider.CreateMesh(false, false);
        _meshFilter.mesh = mesh;
    }
    // 두 콜라이더의 교차점을 찾아내고, 그 정보를 기반으로 Land 경로를 수정
    [ContextMenu("FindIntersections")]
    public void FindIntersections()
    {
        IntersectionPointsList.Clear();

        // 두 콜라이더의 경로를 월드 좌표로 변환
        Vector2[] landPathWorld = _landCollider.GetPath(0);
        for (int i = 0; i < landPathWorld.Length; i++)
            landPathWorld[i] = _landCollider.transform.TransformPoint(landPathWorld[i]);

        Vector2[] sphPathWorld = _sphCollider.GetPath(0);
        for (int i = 0; i < sphPathWorld.Length; i++)
            sphPathWorld[i] = _sphCollider.transform.TransformPoint(sphPathWorld[i]);

        // 모든 선분 쌍 간 교차점 탐색
        for (int l = 0; l < landPathWorld.Length; l++)
        {
            int landIndex = l;
            int nextLandIndex = (l + 1) % landPathWorld.Length;

            Vector2 a = landPathWorld[landIndex];
            Vector2 b = landPathWorld[nextLandIndex];

            for (int s = 0; s < sphPathWorld.Length; s++)
            {
                int sphIndex = s;
                int nextSphIndex = (s + 1) % sphPathWorld.Length;

                Vector2 c = sphPathWorld[sphIndex];
                Vector2 d = sphPathWorld[nextSphIndex];

                if (Intersection.IsIntersecting(a, b, c, d))
                {
                    Vector2 intersection = Intersection.GetIntersection(a, b, c, d);
                    Debug.DrawRay(intersection, Vector3.up * 0.2f, Color.red, 5f);

                    // 교차점이 외부인지 확인
                    bool ccw = _landCollider.ClosestPoint(c) == c;
                    IntersectionPoint intersectionPoint = new IntersectionPoint(landIndex, sphIndex, intersection, !ccw);
                    IntersectionPointsList.Add(intersectionPoint);
                }
            }
        }

        // 기존 Land 경로 수정
        List<Vector2> newPath = landPathWorld.ToList();
        Debug.Log(newPath.Count);

        for (int i = 0; i < IntersectionPointsList.Count; i += 2)
        {
            int sphereStart = IntersectionPointsList[i].SphIndex;
            int sphereEnd = IntersectionPointsList[i + 1].SphIndex;
            int landStart = IntersectionPointsList[i].LandIndex;
            int landEnd = IntersectionPointsList[i + 1].LandIndex;

            // 교차된 Land 경로 삭제 (영역 제거)
            int landIndex = landStart;
            while (true)
            {
                Debug.Log(landIndex + " " + landStart + " " + landEnd);
                if (landIndex == landEnd) break;
                landIndex = GetNext(landIndex, _landCollider.GetTotalPointCount(), true);
                newPath[landIndex] = Vector2.zero; // 제거할 영역 표시
            }

            // Sph 경로의 일부분 삽입
            int sphIndex = sphereStart;
            List<Vector2> arc = new List<Vector2>();
            while (true)
            {
                if (sphIndex == sphereEnd) break;
                Vector2 pos = sphPathWorld[sphIndex];
                arc.Add(pos);
                sphIndex = GetNext(sphIndex, _sphCollider.GetTotalPointCount(), false);
            }
            newPath.InsertRange(landStart + 1, arc);

            // 제거된 부분(=Vector2.zero) 삭제
            for (int d = 0; d < newPath.Count; d++)
            {
                if (newPath[d] == Vector2.zero)
                {
                    newPath.RemoveAt(d);
                }
            }
        }
    }

    // 순방향 또는 역방향으로 인덱스를 순환
    private int GetNext(int index, int length, bool isCW)
    {
        int nextIndex = index + (isCW ? 1 : -1);
        if (nextIndex >= length)
        {
            nextIndex = 0;
        }
        else if (nextIndex < 0)
        {
            nextIndex = length - 1;
        }
        return nextIndex;
    }

    // 콜라이더 경로의 각 포인트에 Gizmo 라벨 표시
    private void OnDrawGizmos()
    {
        for (int i = 0; i < _sphCollider.GetTotalPointCount(); i++)
        {
            Handles.Label(_sphCollider.transform.TransformPoint(_sphCollider.GetPath(0)[i]), i.ToString());
        }
        for (int i = 0; i < _landCollider.GetTotalPointCount(); i++)
        {
            Handles.Label(_landCollider.transform.TransformPoint(_landCollider.GetPath(0)[i]), i.ToString());
        }
    }
}

