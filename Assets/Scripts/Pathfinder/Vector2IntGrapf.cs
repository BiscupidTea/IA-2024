using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vector2IntGrapf<NodeType> : IGraph<NodeType>
    where NodeType : INode<Vector2Int>, new()
{
    public IDictionary<int, NodeType> nodes = new Dictionary<int, NodeType>();

    public Vector2IntGrapf(int x, int y)
    {
        int id = 0;

        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                NodeType node = new NodeType();
                node.SetCoordinate(new Vector2Int(i, j));
                node.SetBloqued(false);
                node.SetNodeCost(0);
                nodes.Add(0, node);
                id++;
            }
        }
        
        SetNeighborsNodes();
    }

    public void SetNeighborsNodes()
    {
        foreach (NodeType currentNode in nodes.Values)
        {
            //up
            currentNode.AddNeighbour();
            
            //down
            currentNode.AddNeighbour();
            
            //left
            currentNode.AddNeighbour();
            
            //right
            currentNode.AddNeighbour();
        }
    }

    public ICollection<NodeType> GetNeighborsNodes(int nodeId)
    {
        List<NodeType> Neighbors = new List<NodeType>();

        for (int i = 0; i < nodes[nodeId].GetNeighbours().Count; i++)
        {
            Neighbors.Add(nodes[nodes[nodeId].GetNeighbours()[i]]);
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