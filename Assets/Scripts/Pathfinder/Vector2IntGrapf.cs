using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2IntGrapf<NodeType> : IGraph<NodeType>
    where NodeType : INode<Vector2Int>, new()
{
    private int id = 0;
    private Traveler.Algorithm algorithmType;
    public IDictionary<int, NodeType> nodes = new Dictionary<int, NodeType>();
    private int nodeGap = 0;

    public Vector2IntGrapf(int rows, int collumns, int nodeGap, Traveler.Algorithm algorithmType)
    {
        this.nodeGap = nodeGap;
        this.algorithmType = algorithmType;
        Vector2Int startPosition = new Vector2Int(0, 0);

        for (int y = 0; y < collumns; y++)
        {
            for (int x = 0; x < rows; x++)
            {
                NodeType node = new NodeType();
                node.SetCoordinate(new Vector2Int(startPosition.x + (x * nodeGap), startPosition.y - (y * nodeGap)));
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
        
        // nodes[40].SetBloqued(true);
        // nodes[41].SetBloqued(true);
        // nodes[42].SetBloqued(true);
        // nodes[43].SetBloqued(true);
        // nodes[44].SetBloqued(true);
        // nodes[45].SetBloqued(true);
        // nodes[46].SetBloqued(true);
        // nodes[47].SetBloqued(true);
        // nodes[51].SetBloqued(true);
        // nodes[61].SetBloqued(true);
        // nodes[71].SetBloqued(true);
    }

    public void SetNeighborsNodes(NodeType currentNode)
    {
        foreach (NodeType neighbor in nodes.Values)
        {
            if (neighbor.GetCoordinate().x == currentNode.GetCoordinate().x &&
                Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1)
            {
                currentNode.AddNeighbour(neighbor.GetId());
            }

            else if (neighbor.GetCoordinate().y == currentNode.GetCoordinate().y &&
                     Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
            {
                currentNode.AddNeighbour(neighbor.GetId());
            }

            if (algorithmType == Traveler.Algorithm.AStarPathfinder || algorithmType == Traveler.Algorithm.DijstraPathfinder)
            {
                if (Math.Abs(neighbor.GetCoordinate().y - currentNode.GetCoordinate().y) == 1 &&
                    Math.Abs(neighbor.GetCoordinate().x - currentNode.GetCoordinate().x) == 1)
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