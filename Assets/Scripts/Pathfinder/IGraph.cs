using System.Collections.Generic;

public interface IGraph<NodeType> : ICollection<NodeType>
{
    public ICollection<NodeType> GetNeighborsNodes(int nodeId);
}