using System;
using System.Collections.Generic;

public class Node<Coordinate> : INode<Coordinate> where Coordinate : IEquatable<Coordinate>
{
    private int Id;
    private Coordinate coordinate;
    private bool isBlocked;
    private int nodeCost;
    private List<int> neightboursId = new List<int>();

    public void SetCoordinate(Coordinate newCoordinate)
    {
        this.coordinate = newCoordinate;
    }

    public Coordinate GetCoordinate()
    {
        return coordinate;
    }

    public void AddNeighbour(int newNeighbourId)
    {
        neightboursId.Add(newNeighbourId);
    }

    public List<int> GetNeighboursID()
    {
        return neightboursId;
    }

    public int GetId()
    {
        return Id;
    }

    public void SetId(int Id)
    {
        this.Id = Id;
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