using System;
using System.Collections.Generic;

public class DepthFirstPathfinder<NodeType, Coordinates> : Pathfinder<NodeType, Coordinates>
    where NodeType : INode<Coordinates>
    where Coordinates : IEquatable<Coordinates>
{

    protected override int Distance(NodeType A, NodeType B)
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