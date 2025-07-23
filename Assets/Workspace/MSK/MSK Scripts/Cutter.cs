using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 절단 지점 정보를 담는 포인트 클래스
[System.Serializable]
public class Point
{
    public Vector2 Position;             // 현재 포인트의 월드 위치
    public Point NextPoint;              // 절단된 후 다음 포인트
    public bool IsCross;                 // 교차 지점 여부
    public Segment LandSegment;          // 이 포인트가 속한 지형 세그먼트
    public Segment CircleSegment;        // 이 포인트가 속한 원 세그먼트
}
// 두 개의 포인트로 구성된 선분
public class Segment
{
    public Point A;
    public Point B;
    public List<Point> CrossPoints = new List<Point>(); // 교차된 지점들
}


// 포인트들과 세그먼트 리스트로 구성된 라인 (폴리곤의 외곽선 구조)
[System.Serializable]
public class Line
{
    public List<Point> Points;
    public List<Segment> Segments;
}
// 지정된 원형 콜라이더로부터 땅 콜라이더를 자르는 Cutter 클래스
public class Cutter : MonoBehaviour
{
    [SerializeField] PolygonCollider2D _landCollider;   // 잘릴 대상인 땅 콜라이더
    [SerializeField] PolygonCollider2D _circleCollider; // 잘라낼 영역인 원 콜라이더
    [SerializeField] int _testIterations = 10;          // 절단 루프의 최대 반복 횟수 (무한루프 방지)
    
    // 실제 절단 실행 함수
    public void DoCut()
    {
        // 원형 콜라이더를 월드 좌표로 변환하여 Line으로 변환
        List<Vector2> _circlePointsPositions = _circleCollider.GetPath(0).ToList();
        for (int i = 0; i < _circlePointsPositions.Count; i++)
            _circlePointsPositions[i] = _circleCollider.transform.TransformPoint(_circlePointsPositions[i]);

        Line circleLine = LineFromCollider(_circlePointsPositions);

        List<List<Point>> allSplines = new List<List<Point>>();

        // 지형의 모든 패스를 순회하며 절단 진행
        for (int p = 0; p < _landCollider.pathCount; p++)
        {
            List<Vector2> _linePointsPositions = _landCollider.GetPath(p).ToList();
            for (int i = 0; i < _linePointsPositions.Count; i++)
                _linePointsPositions[i] = _landCollider.transform.TransformPoint(_linePointsPositions[i]);

            Line landLine = LineFromCollider(_linePointsPositions);

            // 절단 대상이 원 안에 있을 경우 정렬
            for (int i = 0; i < landLine.Points.Count; i++)
            {
                if (_circleCollider.ClosestPoint(landLine.Points[0].Position) == landLine.Points[0].Position)
                {
                    ReorderList(landLine.Points);
                    ReorderList(landLine.Segments);
                }
                else break;
            }

            // 절단 결과 저장
            var result = Substraction(landLine, circleLine);
            allSplines.InsertRange(0, result);
        }

        // 결과를 Land 클래스에 전달
        _landCollider.GetComponent<Land>().SetPath(allSplines);
    }

    // 폭파 지형을 계산
    public List<List<Point>> Substraction(Line landLine, Line circleLine)
    {
        // 각 포인트에 NextPoint 설정
        for (int i = 0; i < circleLine.Points.Count; i++)
        {
            int nextIndex = GetNext(i, circleLine.Points.Count, false);
            circleLine.Points[i].NextPoint = circleLine.Points[nextIndex];
        }

        // 교차점 계산
        for (int l = 0; l < landLine.Segments.Count; l++)
        {
            Segment landSegment = landLine.Segments[l];
            Vector2 al = landSegment.A.Position;
            Vector2 bl = landSegment.B.Position;

            for (int c = 0; c < circleLine.Segments.Count; c++)
            {
                Segment circleSegment = circleLine.Segments[c];
                Vector2 ac = circleSegment.A.Position;
                Vector2 bc = circleSegment.B.Position;

                // 교차하면 교차점 등록
                if (Intersection.IsIntersecting(al, bl, ac, bc))
                {
                    Vector2 position = Intersection.GetIntersection(al, bl, ac, bc);
                    Point crossPoint = new Point
                    {
                        Position = position,
                        LandSegment = landSegment,
                        CircleSegment = circleSegment,
                        IsCross = true
                    };
                    landSegment.CrossPoints.Add(crossPoint);
                    circleSegment.CrossPoints.Add(crossPoint);
                }
            }
        }

        // 세그먼트 재구성 (교차점 포함하도록)
        RecalculateLine(landLine);
        RecalculateLine(circleLine);

        {
            // 각 포인트에 NextPoint를 따라 이어지는 경로 설정
            List<Point> allPoints = new List<Point>(landLine.Points);
            bool onLand = true;
            Point startPoint = allPoints[0];

            while (allPoints.Count > 0)
            {
                Point thePoint = allPoints[0];

                if (_circleCollider.ClosestPoint(thePoint.Position) == thePoint.Position || thePoint.IsCross)
                {
                    allPoints.RemoveAt(0);
                    continue;
                }

                for (int i = 0; i < _testIterations; i++)
                {
                    Line currentLine = onLand ? landLine : circleLine;
                    bool ccw = onLand;

                    int currentIndex = currentLine.Points.IndexOf(thePoint);
                    int nextIndex = GetNext(currentIndex, currentLine.Points.Count, ccw);
                    thePoint.NextPoint = currentLine.Points[nextIndex];
                    allPoints.Remove(thePoint);

                    // 교차점을 만나면 다음 라인으로 스위치
                    if (thePoint.NextPoint.IsCross)
                        onLand = !onLand;

                    thePoint = thePoint.NextPoint;
                    if (startPoint == thePoint) break;
                }
            }
        }

        {
            // 최종 잘린 경로 반환
            List<List<Point>> allSplines = new List<List<Point>>();
            List<Point> allPoints = new List<Point>(landLine.Points);

            while (allPoints.Count > 0)
            {
                Point thePoint = allPoints[0];

                if (_circleCollider.ClosestPoint(thePoint.Position) == thePoint.Position || thePoint.IsCross)
                {
                    allPoints.RemoveAt(0);
                    continue;
                }
                else
                {
                    List<Point> newSpline = new List<Point>();
                    allSplines.Add(newSpline);

                    Point startPoint = thePoint;
                    Point point = thePoint;

                    newSpline.Add(point);
                    allPoints.Remove(point);

                    for (int i = 0; i < _testIterations; i++)
                    {
                        point = point.NextPoint;
                        if (point == startPoint) break;
                        newSpline.Add(point);
                        if (allPoints.Contains(point))
                            allPoints.Remove(point);
                    }
                }
            }

            return allSplines;
        }
    }

   
    // 폭파 지점에 새 정점 생성
    public void RecalculateLine(Line line)
    {
        List<Point> newPoints = new List<Point>();

        for (int s = 0; s < line.Segments.Count; s++)
        {
            Segment segment = line.Segments[s];
            newPoints.Add(segment.A);

            // 교차점들을 A에서 가까운 순으로 정렬
            if (segment.CrossPoints.Count > 0)
            {
                segment.CrossPoints.Sort((p1, p2) =>
                    Vector3.Distance(segment.A.Position, p1.Position).
                    CompareTo(Vector3.Distance(segment.A.Position, p2.Position)));
            }

            newPoints.AddRange(segment.CrossPoints);
        }

        line.Points = newPoints;
    }

    // 리스트를 왼쪽으로 한 칸 순환 이동
    void ReorderList<T>(List<T> list)
    {
        var first = list[0];
        for (int i = 0; i < list.Count; i++)
        {
            if (i == list.Count - 1)
                list[i] = first;
            else
                list[i] = list[i + 1];
        }
    }

    // 벡터 리스트를 Line 객체로 변환
    public Line LineFromCollider(List<Vector2> list)
    {
        Line line = new Line();
        List<Point> points = new List<Point>();
        List<Segment> segments = new List<Segment>();

        // 포인트 생성
        for (int i = 0; i < list.Count; i++)
        {
            Point point = new Point { Position = list[i] };
            points.Add(point);
        }

        // 세그먼트 생성
        for (int i = 0; i < list.Count; i++)
        {
            Segment segment = new Segment();
            segment.A = points[i];
            points[i].LandSegment = segment;

            int bIndex = i + 1;
            if (bIndex >= list.Count) bIndex = 0;

            segment.B = points[bIndex];
            points[bIndex].CircleSegment = segment;
            segments.Add(segment);
        }

        line.Points = points;
        line.Segments = segments;
        return line;
    }


    // 다음 인덱스를 시계/반시계 방향으로 반환
    int GetNext(int index, int length, bool isCCW)
    {
        int nextIndex = index + (isCCW ? 1 : -1);
        if (nextIndex >= length)
            nextIndex = 0;
        else if (nextIndex < 0)
            nextIndex = length - 1;
        return nextIndex;
    }
}
