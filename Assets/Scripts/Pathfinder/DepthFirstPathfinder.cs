using System;
using System.Collections.Generic;
using System.Linq;

public class DepthFirstPathfinder<NodeType, CoordType> : Pathfinder<NodeType, CoordType>
    where NodeType : INode<CoordType>
    where CoordType : IEquatable<CoordType>, ICoordType<int>, new()
{

    protected override float Distance(NodeType A, NodeType B,  IGraph<NodeType> graph, Traveler traveler)
    {
        return 0;
    }
    
    protected override ICollection<NodeType> GetNeighbors(NodeType node, IGraph<NodeType> graph)
    {
        ICollection<NodeType> reverseNodes = new List<NodeType>();

        foreach (NodeType currentNode in graph.GetNeighborsNodes(node.GetId()).Reverse())
        {
            reverseNodes.Add(currentNode);
        }
        
        return reverseNodes;
    }

    protected override bool IsBloqued(NodeType node, Traveler traveler)
    {
        return traveler.NodeTypesBloqued[node.GetNodeType()];
    }

    protected override int MoveToNeighborCost(NodeType A, NodeType b, Traveler traveler)
    {
        return 0;
    }

    protected override bool NodesEquals(NodeType A, NodeType B)
    {
        return Equals(A, B);
    }
}