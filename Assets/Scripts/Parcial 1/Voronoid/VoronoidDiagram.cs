using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class VoronoidController
{
    [SerializeField] private List<MinesTemp> mines = new List<MinesTemp>();
    Vector2 mid;

    List<Vector3> vertex = new List<Vector3>();

    List<Side> aux = new List<Side>();

    public void StartVornonoid(List<Node<CoordinateType>> list, Vector2Int grid)
    {
        mines.Clear();
        vertex.Clear();
        aux.Clear();
        
        vertex.Add(new Vector3(grid.x, -grid.y, 0));
        vertex.Add(new Vector3(grid.x, 1, 0));
        vertex.Add(new Vector3(-1, 1, 0));
        vertex.Add(new Vector3(-1, -grid.y, 0));

        for (int i = 0; i < list.Count; i++)
        {
            mines.Add(new MinesTemp(
                new Vector3(list[i].GetCoordinate().GetXY()[0], list[i].GetCoordinate().GetXY()[1], 1),
                new Poligon(vertex)));
        }

        for (int i = 0; i < mines.Count; i++)
        {
            for (int j = 0; j < mines.Count; j++)
            {
                if (i != j)
                    GetSide(mines[i], mines[j]);
            }
        }
    }

    public void GetSide(MinesTemp A, MinesTemp B)
    {
        Vector3 mid = new Vector3((A.position.x + B.position.x) / 2.0f, (A.position.y + B.position.y) / 2,
            (A.position.z + B.position.z) / 2.0f);

        Vector3 connector = A.position - B.position;

        Vector3 direction = new Vector3(-connector.y, connector.x, 0).normalized;

        Side outCut = new Side(mid - direction * 100.0f / 2, mid + direction * 100.0f / 2);

        aux.Add(outCut);

        PolygonCutter polygonCutter = new PolygonCutter();

        polygonCutter.CutPolygon(A, outCut);
    }

    public Vector2? GetVoronoiCenter(Vector3 point)
    {
        PolygonCutter polygonCutter = new PolygonCutter();

        foreach (var mine in mines)
        {
            if (polygonCutter.IsPointInPolygon(point, mine.poligon))
            {
                return mine.position;
            }
        }

        return null;
    }

    public void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        for (int i = 0; i < mines.Count; i++)
        {
            Gizmos.color = mines[i].color;
            Gizmos.DrawCube(mines[i].position, Vector3.one / 2);
        }

        Gizmos.color = UnityEngine.Color.magenta;
        for (int i = 0; i < vertex.Count; i++)
        {
            Vector3 current = vertex[i];
            Vector3 next = vertex[(i + 1) % vertex.Count];
            Gizmos.DrawLine(current, next);
        }


        for (int i = 0; i < mines.Count; i++)
        {
            Gizmos.color = mines[i].color;
            //Handles.color = mines[i].color;
            //Handles.DrawAAConvexPolygon(mines[i].poligon.vertices.ToArray());
        }

        Gizmos.color = UnityEngine.Color.black;
        for (int j = 0; j < mines.Count; j++)
        {
            for (int i = 0; i < mines[j].poligon.vertices.Count; i++)
            {
                Vector3 vertex1 = mines[j].poligon.vertices[i];
                Vector3 vertex2 = mines[j].poligon.vertices[(i + 1) % mines[j].poligon.vertices.Count];

                Gizmos.DrawLine(vertex1, vertex2);
            }
        }

        Gizmos.color = UnityEngine.Color.cyan;
        for (int i = 0; i < aux.Count; i++)
        {
            //Gizmos.DrawLine(aux[i].start, aux[i].end);
        }


        Gizmos.color = UnityEngine.Color.yellow;
        for (int i = 0; i < vertex.Count; i++)
        {
            Gizmos.DrawSphere(vertex[i], 0.25f);
        }

        for (int i = 0; i < mines.Count; i++)
        {
            GUIStyle style = new GUIStyle();
            style.fontSize = 35;
            style.normal.textColor = Color.black;
            //Handles.Label(new Vector3(mines[i].position.x, mines[i].position.y + 0.5f, mines[i].position.z),
            //    (i + 1).ToString(),
            //    style);
        }
    }
}

public struct Side
{
    public Vector3 start;
    public Vector3 end;

    public Side(Vector3 start, Vector3 end)
    {
        this.start = start;
        this.end = end;
    }
}

[Serializable]
public struct Poligon
{
    public List<Vector3> vertices;

    public Poligon(List<Vector3> vertices)
    {
        this.vertices = vertices;
    }
}

[Serializable]
public class MinesTemp
{
    public Vector3 position;
    public Poligon poligon;
    public UnityEngine.Color color;

    public MinesTemp(Vector3 position, Poligon poligon)
    {
        this.position = position;
        this.poligon = poligon;
        color = new Color(UnityEngine.Random.value, Random.value, Random.value, 0.4f);
    }
}

public class PolygonCutter
{
    public void CutPolygon(MinesTemp mine, Side cut)
    {
        List<Vector3> polygon1 = new List<Vector3>();
        List<Vector3> polygon2 = new List<Vector3>();

        List<Vector3> intersections = new List<Vector3>();

        bool isFirstPolygon = true;


        for (int i = 0; i < mine.poligon.vertices.Count; i++)
        {
            Vector3 p1 = mine.poligon.vertices[i];
            Vector3 p2 = mine.poligon.vertices[(i + 1) % mine.poligon.vertices.Count];

            if (isFirstPolygon)
            {
                polygon1.Add(p1);
            }
            else
            {
                polygon2.Add(p1);
            }

            Side side1 = new Side(p1, p2);
            Side side2 = new Side(cut.start, cut.end);

            if (DetectarInterseccion(side1, side2, out Vector3 intersection))
            {
                if (!intersections.Contains(intersection) && intersections.Count < 2)
                {
                    intersections.Add(intersection);

                    polygon1.Add(intersection);
                    polygon2.Add(intersection);

                    isFirstPolygon = !isFirstPolygon;
                }
            }
        }

        if (intersections.Count == 2)
        {
            if (IsPointInPolygon(mine.position, new Poligon(polygon1)))
            {
                mine.poligon = new Poligon(polygon1);
            }
            else if (IsPointInPolygon(mine.position, new Poligon(polygon2)))
            {
                mine.poligon = new Poligon(polygon2);
            }
            else
            {
                Debug.LogError("POR FAVOR NO SALTES");
            }
        }
        else
        {
            Debug.LogWarning(
                $"No se encontraron intersecciones o el polígono no fue cortado correctamente. {intersections.Count}");
        }
    }

    public static bool DetectarInterseccion(Side side1, Side side2, out Vector3 puntoInterseccion)
    {
        puntoInterseccion = new Vector3();

        float denominator = (side1.start.x - side1.end.x) * (side2.start.y - side2.end.y) -
                            (side1.start.y - side1.end.y) * (side2.start.x - side2.end.x);

        if (Mathf.Abs(denominator) < Mathf.Epsilon)
        {
            return false;
        }

        float t = ((side1.start.x - side2.start.x) * (side2.start.y - side2.end.y) -
                   (side1.start.y - side2.start.y) * (side2.start.x - side2.end.x)) / denominator;
        float u = ((side1.start.x - side2.start.x) * (side1.start.y - side1.end.y) -
                   (side1.start.y - side2.start.y) * (side1.start.x - side1.end.x)) / denominator;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            puntoInterseccion = new Vector3(
                side1.start.x + t * (side1.end.x - side1.start.x),
                side1.start.y + t * (side1.end.y - side1.start.y),
                0
            );
            return true;
        }

        return false;
    }

    public bool IsPointInPolygon(Vector3 point, Poligon polygon)
    {
        int intersectCount = 0;

        for (int i = 0; i < polygon.vertices.Count; i++)
        {
            Vector3 vertex1 = polygon.vertices[i];
            Vector3 vertex2 = polygon.vertices[(i + 1) % polygon.vertices.Count];

            if (RayIntersectsSegment(point, vertex1, vertex2))
            {
                intersectCount++;
            }
        }

        return (intersectCount % 2) == 1;
    }

    public bool RayIntersectsSegment(UnityEngine.Vector3 point, UnityEngine.Vector3 v1, Vector3 v2)
    {
        if (v1.y > v2.y)
        {
            Vector3 temp = v1;
            v1 = v2;
            v2 = temp;
        }

        if (Mathf.Abs(point.y - v1.y) < 0.001f || Mathf.Abs(point.y - v2.y) < 0.001f)
        {
            point.y += 0.0001f;
        }

        if (point.y < v1.y || point.y > v2.y)
        {
            return false;
        }

        if (point.x > Mathf.Max(v1.x, v2.x))
        {
            return false;
        }

        if (point.x < Mathf.Min(v1.x, v2.x))
        {
            return true;
        }

        float deltaZ = v2.y - v1.y;
        float deltaX = v2.x - v1.x;

        if (Mathf.Abs(deltaZ) < 0.001f)
        {
            return false;
        }

        float slope = deltaX / deltaZ;
        float xIntersect = v1.x + (point.y - v1.y) * slope;

        return point.x < xIntersect;
    }
}