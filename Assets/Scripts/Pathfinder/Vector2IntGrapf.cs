﻿using System;
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
        
        nodes[40].SetNodeCost(50);
        nodes[41].SetNodeCost(50);
        nodes[42].SetNodeCost(50);
        nodes[43].SetNodeCost(50);
        nodes[44].SetNodeCost(50);
        nodes[45].SetNodeCost(50);
        nodes[46].SetNodeCost(50);
        nodes[47].SetNodeCost(50);
        nodes[51].SetNodeCost(50);
        nodes[61].SetNodeCost(50);
        nodes[71].SetNodeCost(50);
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

    public float GetDistanceBetweenNodes(NodeType A, NodeType B)
    {
        return Vector2Int.Distance(A.GetCoordinate(), B.GetCoordinate());
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