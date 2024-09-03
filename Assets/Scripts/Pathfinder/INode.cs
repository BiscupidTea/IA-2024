using System;
using System.Collections.Generic;

public interface INode
{
    public int GetId();
    public void SetId(int Id);
    public bool GetBloqued();
    public void SetBloqued(bool BlockState);
    public int GetNodeCost();
    public void SetNodeCost(int NodeCost);
}

public interface INode<Coordinate> : INode where Coordinate : IEquatable<Coordinate>
{
    public void SetCoordinate(Coordinate newCoordinate);
    public Coordinate GetCoordinate();

    public void AddNeighbour(int newNeighbourId);
    public List<int> GetNeighboursID();
}