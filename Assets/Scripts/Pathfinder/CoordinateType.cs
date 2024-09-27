using System;
using UnityEngine;

public class CoordinateType : ICoordType<int>, IEquatable<CoordinateType>
{
    private Vector2Int coordinate;
    
    public void Init(params int[] parameter)
    {
        coordinate = new Vector2Int((int)parameter[0], (int)parameter[1]);
    }

    public int[] GetXY()
    {
        return new int[] { coordinate.x, coordinate.y };
    }

    public float DistanceTo(int[] coordinates)
    {
        return Vector2Int.Distance(coordinate, new Vector2Int(coordinates[0], coordinates[1]));
    }

    public bool Equals(CoordinateType other)
    {
        return GetXY() == other.GetXY();
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((CoordinateType)obj);
    }

    public override int GetHashCode()
    {
        return coordinate.GetHashCode();
    }

    public CoordinateType Subtract(CoordinateType coordinates)
    {
        int x = GetXY()[0] - coordinates.GetXY()[0];
        int y = GetXY()[1] - coordinates.GetXY()[1];

        CoordinateType result = new CoordinateType();
        result.Init(x, y);
        return result;
    }
    public CoordinateType Sum(CoordinateType coordinates)
    {
        int x = GetXY()[0] + coordinates.GetXY()[0];
        int y = GetXY()[1] + coordinates.GetXY()[1];

        CoordinateType result = new CoordinateType();
        result.Init(x, y);
        return result;
    }

    public CoordinateType Multiply(int coordinates)
    {
        int x = GetXY()[0] * coordinates;
        int y = GetXY()[1] * coordinates;

        CoordinateType result = new CoordinateType();
        result.Init(x, y);
        return result;
    }

}