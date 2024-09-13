using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapf<NodeType, CoordType> : IGraph<NodeType>
    where NodeType : INode<CoordType>, new()
    where CoordType : IEquatable<CoordType>, ICoordType<int>, new()
{
    private int id = 0;
    private Algorithm algorithmType;
    public IDictionary<int, NodeType> nodes = new Dictionary<int, NodeType>();
    private int nodeGap = 0;

    public Grapf(int rows, int collumns, int nodeGap, Algorithm algorithmType)
    {
        this.nodeGap = nodeGap;
        this.algorithmType = algorithmType;
        CoordinateType startPosition = new CoordinateType();

        for (int y = 0; y < collumns; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                
                NodeType node = new NodeType();
                CoordType coord = new CoordType();
                
                startPosition.Init(0,0);
                
                coord.Init(startPosition.GetXY()[0] + (x * nodeGap), startPosition.GetXY()[1] - (y * nodeGap));
                
                node.SetCoordinate(coord);
                node.SetBloqued(false);
                node.SetNodeCost(0);
                node.SetId(id);
                nodes.Add(id, node);
                id++;
            }
        }

        foreach (NodeType currentNode in nodes.Values)
        {
            SetNeighborsNodes(currentNode);
        }
    }

    public void SetNeighborsNodes(NodeType currentNode)
    {
        foreach (NodeType neighbor in nodes.Values)
        {
            if (neighbor.GetCoordinate().GetXY()[0] == currentNode.GetCoordinate().GetXY()[0] &&
                Math.Abs(neighbor.GetCoordinate().GetXY()[1] - currentNode.GetCoordinate().GetXY()[1]) == 1)
            {
                currentNode.AddNeighbour(neighbor.GetId());
            }

            else if (neighbor.GetCoordinate().GetXY()[1] == currentNode.GetCoordinate().GetXY()[1] &&
                     Math.Abs(neighbor.GetCoordinate().GetXY()[0] - currentNode.GetCoordinate().GetXY()[0]) == 1)
            {
                currentNode.AddNeighbour(neighbor.GetId());
            }

            if (algorithmType == Algorithm.AStarPathfinder ||
                algorithmType == Algorithm.DijstraPathfinder)
            {
                if (Math.Abs(neighbor.GetCoordinate().GetXY()[1] - currentNode.GetCoordinate().GetXY()[1]) == 1 &&
                    Math.Abs(neighbor.GetCoordinate().GetXY()[0] - currentNode.GetCoordinate().GetXY()[0]) == 1)
                    currentNode.AddNeighbour(neighbor.GetId());
            }
        }
    }

    public ICollection<NodeType> GetNeighborsNodes(int nodeId)
    {
        List<NodeType> Neighbors = new List<NodeType>();

        for (int i = 0; i < nodes[nodeId].GetNeighboursID().Count; i++)
        {
            Neighbors.Add(nodes[nodes[nodeId].GetNeighboursID()[i]]);
        }

        return Neighbors;
    }

    public float GetDistanceBetweenNodes(NodeType A, NodeType B)
    {
        return A.GetCoordinate().DistanceTo(B.GetCoordinate().GetXY());
    }

    public IEnumerator<NodeType> GetEnumerator()
    {
        return nodes.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(NodeType item)
    {
        nodes.Values.Add(item);
    }

    public void Clear()
    {
        nodes.Clear();
    }

    public bool Contains(NodeType item)
    {
        return nodes.Values.Contains(item);
    }

    public void CopyTo(NodeType[] array, int arrayIndex)
    {
        nodes.Values.CopyTo(array, arrayIndex);
    }

    public bool Remove(NodeType item)
    {
        return nodes.Values.Remove(item);
    }

    public int Count { get; }
    public bool IsReadOnly { get; }
}