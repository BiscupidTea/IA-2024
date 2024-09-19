using System;
using System.Collections.Generic;

public class Node<Coordinate> : INode<Coordinate> where Coordinate : IEquatable<Coordinate>
{
    private int Id;
    private Coordinate coordinate;
    private bool isBlocked;
    private int nodeCost;
    private List<int> neightboursId = new List<int>();
    private NodeTypeCost nodeTypeCost;
    private TileClass nodeTileClass;

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

    public void Init(Coordinate coordinate)
    {
        this.coordinate = coordinate;
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

    public void SetNodeType(NodeTypeCost nodeType)
    {
        if (nodeType == NodeTypeCost.GoldMine)
        {
            nodeTileClass = new MineInventory();
        }
        
        nodeTypeCost = nodeType;
    }

    public NodeTypeCost GetNodeType()
    {
        return nodeTypeCost;
    }

    public TileClass GetTileClass()
    {
        return nodeTileClass;
    }

    public void SetTileClass(TileClass tileClass)
    {
        this.nodeTileClass = tileClass;
    }
}