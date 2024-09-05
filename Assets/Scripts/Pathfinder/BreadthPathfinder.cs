using System;
using System.Collections.Generic;
using System.Linq;

public class BreadthPathfinder<NodeType, Coordinates> : Pathfinder<NodeType, Coordinates> 
    where NodeType : INode<Coordinates>
    where Coordinates : IEquatable<Coordinates>
{
    protected override float Distance(NodeType A, NodeType B,  IGraph<NodeType> graph)
    {
        return 0;
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
        return 0;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return Equals(A, B);
    }
}