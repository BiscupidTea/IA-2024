using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class VoronoidDiagram : MonoBehaviour
{
    public int numberOfPoints = 10;
    public Vector2 area = new Vector2(10, 10);
    private List<PointData> points = new List<PointData>();

    void Start()
    {
        GenerateRandomPoints();

        foreach (var point in points)
        {
            ProcessSinglePoint(point);
        }
    }

    void GenerateRandomPoints()
    {
        for (int i = 0; i < numberOfPoints; i++)
        {
            Vector2 randomPoint = new Vector2(
                Random.Range(0, area.x),
                Random.Range(0, area.y)
            );

            points.Add(new PointData(randomPoint));
        }
    }

    void ProcessSinglePoint(PointData referencePoint)
    {
        var otherPoints = points.Where(p => p != referencePoint)
            .OrderBy(p => Vector2.Distance(referencePoint.position, p.position))
            .ToList();

        foreach (var otherPoint in otherPoints)
        {
            Vector2 midpoint = (referencePoint.position + otherPoint.position) / 2;
            Vector2 direction = (otherPoint.position - referencePoint.position).normalized;
            Vector2 perpendicularDirection = new Vector2(-direction.y, direction.x);

            Vector2
                start = midpoint +
                        perpendicularDirection * 20;
            Vector2 end = midpoint - perpendicularDirection * 20;

            GetIntersectionPoint(new Vector2(0, 0), new Vector2(area.x, 0), start, end);
            GetIntersectionPoint(new Vector2(0, area.y), new Vector2(area.x, area.y), start, end);
            GetIntersectionPoint(new Vector2(0, 0), new Vector2(0, area.y), start, end);
            GetIntersectionPoint(new Vector2(area.x, 0), new Vector2(area.x, area.y), start, end);

            referencePoint.mediatrices.Add((start, end));
            otherPoint.mediatrices.Add((start, end));
        }
    }

    public Vector2 GetIntersectionPoint(Vector2 Astart, Vector2 Aend, Vector2 Bstart, Vector2 Bend)
    {
        float denominator = (Aend.x - Astart.x) * (Bend.y - Bstart.y) - (Aend.y - Astart.y) * (Bend.x - Bstart.x);

        if (Mathf.Abs(denominator) < Mathf.Epsilon)
        {
            Debug.Log("No intersection, lines are parallel.");
            return Vector2.zero;
        }

        float t = ((Astart.x - Bstart.x) * (Bend.y - Bstart.y) - (Astart.y - Bstart.y) * (Bend.x - Bstart.x)) /
                  denominator;
        float u = ((Astart.x - Bstart.x) * (Aend.y - Astart.y) - (Astart.y - Bstart.y) * (Aend.x - Astart.x)) /
                  denominator;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            Vector2 intersection = new Vector2(Astart.x + t * (Aend.x - Astart.x), Astart.y + t * (Aend.y - Astart.y));
            return intersection;
        }

        Debug.Log("No intersection within the segment bounds.");
        return Vector2.zero;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(area.x, 0, 0));
        Gizmos.DrawLine(new Vector3(0, area.y, 0), new Vector3(area.x, area.y, 0));
        Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, area.y, 0));
        Gizmos.DrawLine(new Vector3(area.x, 0, 0), new Vector3(area.x, area.y, 0));

        if (Application.isPlaying)
        {
            //puntos
            foreach (PointData point in points)
            {
                Gizmos.color = (point == points[0]) ? Color.yellow : Color.blue;
                Gizmos.DrawSphere(new Vector3(point.position.x, point.position.y, 0), 0.2f);
            }

            //mediatrices
            foreach (var line in points[0].mediatrices)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(new Vector3(line.start.x, line.start.y, 0), new Vector3(line.end.x, line.end.y, 0));
            }
        }
    }

    private class PointData
    {
        public Vector2 position;
        public List<(Vector2 start, Vector2 end)> mediatrices = new List<(Vector2, Vector2)>();

        public PointData(Vector2 position)
        {
            this.position = position;
        }
    }
}