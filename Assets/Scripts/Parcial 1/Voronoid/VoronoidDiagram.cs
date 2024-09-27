using System;
using System.Collections.Generic;
using System.Numerics;

public class VoronoidController<CoordType>
    where CoordType : IEquatable<CoordType>, ICoordType<int>, new()
{
    public List<MinesTemp> mines = new List<MinesTemp>();
    public CoordinateType mid;

    public List<CoordinateType> vertex = new List<CoordinateType>();

    public List<Side> aux = new List<Side>();

    public void StartVornonoid(List<Node<CoordinateType>> list, CoordinateType grid)
    {
        mines.Clear();
        vertex.Clear();
        aux.Clear();


        CoordinateType a = new CoordinateType();
        a.Init(grid.GetXY()[0], -grid.GetXY()[1]);
        CoordinateType b = new CoordinateType();
        b.Init(grid.GetXY()[0], 1);
        CoordinateType c = new CoordinateType();
        c.Init(-1, 1);
        CoordinateType d = new CoordinateType();
        d.Init(-1, -grid.GetXY()[1]);

        vertex.Add(a);
        vertex.Add(b);
        vertex.Add(c);
        vertex.Add(d);

        for (int i = 0; i < list.Count; i++)
        {
            CoordinateType temp = new CoordinateType();
            temp.Init(list[i].GetCoordinate().GetXY()[0], list[i].GetCoordinate().GetXY()[1]);

            mines.Add(new MinesTemp(temp, new Poligon(vertex)));
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
        CoordinateType mid = new CoordinateType();
        mid.Init(((A.position.GetXY()[0] + B.position.GetXY()[0]) / 2), (A.position.GetXY()[1] + B.position.GetXY()[1]) / 2);

        CoordinateType connector = new CoordinateType();



        connector = A.position.Subtract(B.position);

        CoordinateType direction = new CoordinateType();
        Vector2 norm = Vector2.Normalize(new Vector2(-connector.GetXY()[1], connector.GetXY()[0]));

        direction.Init((int)norm.X, (int)norm.Y);

        Side outCut = new Side(mid.Subtract(direction.Multiply(100)), mid.Sum(direction.Multiply(100)));

        aux.Add(outCut);

        PolygonCutter polygonCutter = new PolygonCutter();

        polygonCutter.CutPolygon(A, outCut);
    }

    public CoordinateType GetVoronoiCenter(CoordinateType point)
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

    //public void OnDrawGizmos()
    //{
    //    if (!Application.isPlaying)
    //        return;

    //    for (int i = 0; i < mines.Count; i++)
    //    {
    //        Gizmos.color = mines[i].color;
    //        Gizmos.DrawCube(mines[i].position, Vector3.one / 2);
    //    }

    //    Gizmos.color = UnityEngine.Color.magenta;
    //    for (int i = 0; i < vertex.Count; i++)
    //    {
    //        Vector3 current = vertex[i];
    //        Vector3 next = vertex[(i + 1) % vertex.Count];
    //        Gizmos.DrawLine(current, next);
    //    }


    //    for (int i = 0; i < mines.Count; i++)
    //    {
    //        Gizmos.color = mines[i].color;
    //        // Handles.color = mines[i].color;
    //        // Handles.DrawAAConvexPolygon(mines[i].poligon.vertices.ToArray());
    //    }

    //    Gizmos.color = UnityEngine.Color.black;
    //    for (int j = 0; j < mines.Count; j++)
    //    {
    //        for (int i = 0; i < mines[j].poligon.vertices.Count; i++)
    //        {
    //            Vector3 vertex1 = mines[j].poligon.vertices[i];
    //            Vector3 vertex2 = mines[j].poligon.vertices[(i + 1) % mines[j].poligon.vertices.Count];

    //            Gizmos.DrawLine(vertex1, vertex2);
    //        }
    //    }

    //    Gizmos.color = UnityEngine.Color.cyan;
    //    for (int i = 0; i < aux.Count; i++)
    //    {
    //        //Gizmos.DrawLine(aux[i].start, aux[i].end);
    //    }


    //    Gizmos.color = UnityEngine.Color.yellow;
    //    for (int i = 0; i < vertex.Count; i++)
    //    {
    //        Gizmos.DrawSphere(vertex[i], 0.25f);
    //    }

    //    for (int i = 0; i < mines.Count; i++)
    //    {
    //        GUIStyle style = new GUIStyle();
    //        style.fontSize = 35;
    //        style.normal.textColor = Color.black;
    //        //Handles.Label(new Vector3(mines[i].position.x, mines[i].position.y + 0.5f, mines[i].position.z),
    //        //    (i + 1).ToString(),
    //        //    style);
    //    }
    //}
}

public struct Side
{
    public CoordinateType start;
    public CoordinateType end;

    public Side(CoordinateType start, CoordinateType end)
    {
        this.start = start;
        this.end = end;
    }
}

[Serializable]
public struct Poligon
{
    public List<CoordinateType> vertices;

    public Poligon(List<CoordinateType> vertices)
    {
        this.vertices = vertices;
    }
}

[Serializable]
public class MinesTemp
{
    public CoordinateType position;
    public Poligon poligon;

    public MinesTemp(CoordinateType position, Poligon poligon)
    {
        this.position = position;
        this.poligon = poligon;
    }
}

public class PolygonCutter
{
    public void CutPolygon(MinesTemp mine, Side cut)
    {
        List<CoordinateType> polygon1 = new List<CoordinateType>();
        List<CoordinateType> polygon2 = new List<CoordinateType>();

        List<CoordinateType> intersections = new List<CoordinateType>();

        bool isFirstPolygon = true;


        for (int i = 0; i < mine.poligon.vertices.Count; i++)
        {
            CoordinateType p1 = mine.poligon.vertices[i];
            CoordinateType p2 = mine.poligon.vertices[(i + 1) % mine.poligon.vertices.Count];

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

            if (DetectarInterseccion(side1, side2, out CoordinateType intersection))
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
        }
    }

    public static bool DetectarInterseccion(Side side1, Side side2, out CoordinateType puntoInterseccion)
    {
        puntoInterseccion = new CoordinateType();

        int denominator = (side1.start.GetXY()[0] - side1.end.GetXY()[0]) * (side2.start.GetXY()[1] - side2.end.GetXY()[1]) -
                            (side1.start.GetXY()[1] - side1.end.GetXY()[1]) * (side2.start.GetXY()[0] - side2.end.GetXY()[0]);

        if (Math.Abs(denominator) < Math.E)
        {
            return false;
        }

        int t = ((side1.start.GetXY()[0] - side2.start.GetXY()[0]) * (side2.start.GetXY()[1] - side2.end.GetXY()[1]) -
                   (side1.start.GetXY()[0] - side2.start.GetXY()[0]) * (side2.start.GetXY()[0] - side2.end.GetXY()[0])) / denominator;
        int u = ((side1.start.GetXY()[0] - side2.start.GetXY()[0]) * (side1.start.GetXY()[1] - side1.end.GetXY()[1]) -
                   (side1.start.GetXY()[0] - side2.start.GetXY()[0]) * (side1.start.GetXY()[0] - side1.end.GetXY()[0])) / denominator;

        if (t >= 0 && t <= 1 && u >= 0 && u <= 1)
        {
            puntoInterseccion = new CoordinateType();
            puntoInterseccion.Init(side1.start.GetXY()[0] + t * (side1.end.GetXY()[0] - side1.start.GetXY()[0]),
                side1.start.GetXY()[1] + t * (side1.end.GetXY()[1] - side1.start.GetXY()[1]));
            return true;
        }

        return false;
    }

    public bool IsPointInPolygon(CoordinateType point, Poligon polygon)
    {
        int intersectCount = 0;

        for (int i = 0; i < polygon.vertices.Count; i++)
        {
            CoordinateType vertex1 = polygon.vertices[i];
            CoordinateType vertex2 = polygon.vertices[(i + 1) % polygon.vertices.Count];

            if (RayIntersectsSegment(point, vertex1, vertex2))
            {
                intersectCount++;
            }
        }

        return (intersectCount % 2) == 1;
    }

    public bool RayIntersectsSegment(CoordinateType point, CoordinateType v1, CoordinateType v2)
    {
        if (v1.GetXY()[1] > v2.GetXY()[1])
        {
            CoordinateType temp = v1;
            v1 = v2;
            v2 = temp;
        }

        if (point.GetXY()[1] < v1.GetXY()[1] || point.GetXY()[1] > v2.GetXY()[1])
        {
            return false;
        }

        if (point.GetXY()[0] > Math.Max(v1.GetXY()[0], v2.GetXY()[0]))
        {
            return false;
        }

        if (point.GetXY()[0] < Math.Min(v1.GetXY()[0], v2.GetXY()[0]))
        {
            return true;
        }

        float deltaZ = v2.GetXY()[1] - v1.GetXY()[1];
        float deltaX = v2.GetXY()[0] - v1.GetXY()[0];

        if (Math.Abs(deltaZ) < 0.001f)
        {
            return false;
        }

        float slope = deltaX / deltaZ;
        float xIntersect = v1.GetXY()[0] + (point.GetXY()[1] - v1.GetXY()[1]) * slope;

        return point.GetXY()[0] < xIntersect;
    }
}