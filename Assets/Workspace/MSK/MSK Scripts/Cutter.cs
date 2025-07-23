using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ���� ���� ������ ��� ����Ʈ Ŭ����
[System.Serializable]
public class Point
{
    public Vector2 Position;             // ���� ����Ʈ�� ���� ��ġ
    public Point NextPoint;              // ���ܵ� �� ���� ����Ʈ
    public bool IsCross;                 // ���� ���� ����
    public Segment LandSegment;          // �� ����Ʈ�� ���� ���� ���׸�Ʈ
    public Segment CircleSegment;        // �� ����Ʈ�� ���� �� ���׸�Ʈ
}
// �� ���� ����Ʈ�� ������ ����
public class Segment
{
    public Point A;
    public Point B;
    public List<Point> CrossPoints = new List<Point>(); // ������ ������
}


// ����Ʈ��� ���׸�Ʈ ����Ʈ�� ������ ���� (�������� �ܰ��� ����)
[System.Serializable]
public class Line
{
    public List<Point> Points;
    public List<Segment> Segments;
}
// ������ ���� �ݶ��̴��κ��� �� �ݶ��̴��� �ڸ��� Cutter Ŭ����
public class Cutter : MonoBehaviour
{
    [SerializeField] PolygonCollider2D _landCollider;   // �߸� ����� �� �ݶ��̴�
    [SerializeField] PolygonCollider2D _circleCollider; // �߶� ������ �� �ݶ��̴�
    [SerializeField] int _testIterations = 10;          // ���� ������ �ִ� �ݺ� Ƚ�� (���ѷ��� ����)
    
    // ���� ���� ���� �Լ�
    public void DoCut()
    {
        // ���� �ݶ��̴��� ���� ��ǥ�� ��ȯ�Ͽ� Line���� ��ȯ
        List<Vector2> _circlePointsPositions = _circleCollider.GetPath(0).ToList();
        for (int i = 0; i < _circlePointsPositions.Count; i++)
            _circlePointsPositions[i] = _circleCollider.transform.TransformPoint(_circlePointsPositions[i]);

        Line circleLine = LineFromCollider(_circlePointsPositions);

        List<List<Point>> allSplines = new List<List<Point>>();

        // ������ ��� �н��� ��ȸ�ϸ� ���� ����
        for (int p = 0; p < _landCollider.pathCount; p++)
        {
            List<Vector2> _linePointsPositions = _landCollider.GetPath(p).ToList();
            for (int i = 0; i < _linePointsPositions.Count; i++)
                _linePointsPositions[i] = _landCollider.transform.TransformPoint(_linePointsPositions[i]);

            Line landLine = LineFromCollider(_linePointsPositions);

            // ���� ����� �� �ȿ� ���� ��� ����
            for (int i = 0; i < landLine.Points.Count; i++)
            {
                if (_circleCollider.ClosestPoint(landLine.Points[0].Position) == landLine.Points[0].Position)
                {
                    ReorderList(landLine.Points);
                    ReorderList(landLine.Segments);
                }
                else break;
            }

            // ���� ��� ����
            var result = Substraction(landLine, circleLine);
            allSplines.InsertRange(0, result);
        }

        // ����� Land Ŭ������ ����
        _landCollider.GetComponent<Land>().SetPath(allSplines);
    }

    // ���� ������ ���
    public List<List<Point>> Substraction(Line landLine, Line circleLine)
    {
        // �� ����Ʈ�� NextPoint ����
        for (int i = 0; i < circleLine.Points.Count; i++)
        {
            int nextIndex = GetNext(i, circleLine.Points.Count, false);
            circleLine.Points[i].NextPoint = circleLine.Points[nextIndex];
        }

        // ������ ���
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

                // �����ϸ� ������ ���
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

        // ���׸�Ʈ �籸�� (������ �����ϵ���)
        RecalculateLine(landLine);
        RecalculateLine(circleLine);

        {
            // �� ����Ʈ�� NextPoint�� ���� �̾����� ��� ����
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

                    // �������� ������ ���� �������� ����ġ
                    if (thePoint.NextPoint.IsCross)
                        onLand = !onLand;

                    thePoint = thePoint.NextPoint;
                    if (startPoint == thePoint) break;
                }
            }
        }

        {
            // ���� �߸� ��� ��ȯ
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

   
    // ���� ������ �� ���� ����
    public void RecalculateLine(Line line)
    {
        List<Point> newPoints = new List<Point>();

        for (int s = 0; s < line.Segments.Count; s++)
        {
            Segment segment = line.Segments[s];
            newPoints.Add(segment.A);

            // ���������� A���� ����� ������ ����
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

    // ����Ʈ�� �������� �� ĭ ��ȯ �̵�
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

    // ���� ����Ʈ�� Line ��ü�� ��ȯ
    public Line LineFromCollider(List<Vector2> list)
    {
        Line line = new Line();
        List<Point> points = new List<Point>();
        List<Segment> segments = new List<Segment>();

        // ����Ʈ ����
        for (int i = 0; i < list.Count; i++)
        {
            Point point = new Point { Position = list[i] };
            points.Add(point);
        }

        // ���׸�Ʈ ����
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


    // ���� �ε����� �ð�/�ݽð� �������� ��ȯ
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
