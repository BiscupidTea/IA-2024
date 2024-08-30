using System;
using System.Collections.Generic;

public class Node<Coordinate> : INode<Coordinate> where Coordinate : IEquatable<Coordinate>
{
    private Coordinate coordinate;
    private bool isBlocked;
    private int nodeCost;
    private List<INode<Coordinate>> neightbours = new List<INode<Coordinate>>();

    public void SetCoordinate(Coordinate newCoordinate)
    {
        this.coordinate = newCoordinate;
    }

    public Coordinate GetCoordinate()
    {
        return coordinate;
    }

    public void AddNeighbour(Node<Coordinate> newNeighbour)
    {
        neightbours.Add(newNeighbour);
    }

    public List<INode<Coordinate>> GetNeighbours()
    {
        return neightbours;
    }

    public bool GetBloqued()
    {
        return isBlocked;
    }

    public void SetBloqued(bool BlockState)
    {
        isBlocked = BlockState;
    }

    public int GetNodeCost()
    {
        return nodeCost;
    }

    public void SetNodeCost(int NodeCost)
    {
        nodeCost = NodeCost;
    }
}