﻿using System;
using System.Collections.Generic;

public class AStarPathfinder<NodeType, CoordType> : Pathfinder<NodeType, CoordType>
    where NodeType : INode<CoordType>
    where CoordType : IEquatable<CoordType>, ICoordType<int>, new()
{
    protected override float Distance(NodeType A, NodeType B, IGraph<NodeType> graph)
    {
        return graph.GetDistanceBetweenNodes(A, B);
    }

    protected override ICollection<NodeType> GetNeighbors(NodeType node, IGraph<NodeType> graph)
    {
        return graph.GetNeighborsNodes(node.GetId());
    }

    protected override bool IsBloqued(NodeType node)
    {
        return node.GetBloqued();
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType b)
    {
        return (A.GetNodeCost() - b.GetNodeCost());
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return Equals(A, B);
    }
}