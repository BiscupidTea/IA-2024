using System;
using System.Collections.Generic;

public enum Algorithm
{
    DepthFirstPathfinder = 0,
    BreadthPathfinder,
    DijstraPathfinder,
    AStarPathfinder,
}

public abstract class Pathfinder<NodeType, CoordType>
    where NodeType : INode<CoordType>
    where CoordType : IEquatable<CoordType>, ICoordType<int>, new()
{
    public List<NodeType> FindPath(NodeType startNode, NodeType destinationNode, IGraph<NodeType> graph, Traveler traveler)
    {
        Dictionary<NodeType, (NodeType Parent, int AcumulativeCost, float Heuristic)> nodes =
            new Dictionary<NodeType, (NodeType Parent, int AcumulativeCost, float Heuristic)>();

        foreach (NodeType node in graph)
        {
            nodes.Add(node, (default, 0, 0));
        }

        List<NodeType> openList = new List<NodeType>();
        List<NodeType> closedList = new List<NodeType>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            NodeType currentNode = openList[0];
            int currentIndex = 0;

            for (int i = 1; i < openList.Count; i++)
            {
                if (nodes[openList[i]].AcumulativeCost + nodes[openList[i]].Heuristic <
                nodes[currentNode].AcumulativeCost + nodes[currentNode].Heuristic)
                {
                    currentNode = openList[i];
                    currentIndex = i;
                }
            }

            openList.RemoveAt(currentIndex);
            closedList.Add(currentNode);

            if (NodesEquals(currentNode, destinationNode))
            {
                return GeneratePath(startNode, destinationNode);
            }

            foreach (NodeType neighbor in GetNeighbors(currentNode, graph))
            {
                if (!nodes.ContainsKey(neighbor) ||
                IsBloqued(neighbor, traveler) ||
                closedList.Contains(neighbor))
                {
                    continue;
                }

                int tentativeNewAcumulatedCost = 0;
                tentativeNewAcumulatedCost += nodes[currentNode].AcumulativeCost;
                tentativeNewAcumulatedCost += MoveToNeighborCost(currentNode, neighbor, traveler);

                if (!openList.Contains(neighbor) || tentativeNewAcumulatedCost < nodes[currentNode].AcumulativeCost)
                {
                    nodes[neighbor] = (currentNode, tentativeNewAcumulatedCost, Distance(neighbor, destinationNode, graph, traveler));

                    if (!openList.Contains(neighbor))
                    {
                        openList.Add(neighbor);
                    }
                }
            }
        }

        return null;

        List<NodeType> GeneratePath(NodeType startNode, NodeType goalNode)
        {
            List<NodeType> path = new List<NodeType>();
            NodeType currentNode = goalNode;
            
            while (!NodesEquals(currentNode, startNode))
            {
                path.Add(currentNode);
                currentNode = nodes[currentNode].Parent;
            }

            path.Reverse();
            
            return path;
        }
    }

    protected abstract ICollection<NodeType> GetNeighbors(NodeType node, IGraph<NodeType> graph);

    protected abstract float Distance(NodeType A, NodeType B, IGraph<NodeType> graph, Traveler traveler);

    protected abstract bool NodesEquals(NodeType A, NodeType B);

    protected abstract int MoveToNeighborCost(NodeType A, NodeType b, Traveler traveler);

    protected abstract bool IsBloqued(NodeType node, Traveler traveler);
}