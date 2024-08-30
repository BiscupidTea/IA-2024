using System;
using System.Collections.Generic;

public interface INode
{
    public bool GetBloqued();
    public void SetBloqued(bool BlockState);
    public int GetNodeCost();
    public void SetNodeCost(int NodeCost);
}

public interface INode<Coordinate> : INode where Coordinate : IEquatable<Coordinate>
{
    public void SetCoordinate(Coordinate newCoordinate);
    public Coordinate GetCoordinate();
    
    public void AddNeighbour(Node<Coordinate> newNeighbour);
    public List<INode<Coordinate>> GetNeighbours();
}
