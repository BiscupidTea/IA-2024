using System;
using System.Collections.Generic;
using System.Linq;

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
        ICollection<NodeType> reverseNodes = new List<NodeType>();

        foreach (NodeType currentNode in graph.GetNeighborsNodes(node.GetId()).Reverse())
        {
            reverseNodes.Add(currentNode);
        }
        
        return reverseNodes;
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