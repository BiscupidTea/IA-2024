using System.Collections.Generic;

public class Traveler
{
    public Dictionary<NodeTypeCost, int> NodeTypesAditionalCost { get; set; }
    public Dictionary<NodeTypeCost, bool> NodeTypesBloqued { get; set; }

    public Traveler()
    {
        NodeTypesAditionalCost = new Dictionary<NodeTypeCost, int>();
        NodeTypesBloqued = new Dictionary<NodeTypeCost, bool>();
    }

    public int GetNodeTypesAditionalCost(NodeTypeCost node)
    {
        return NodeTypesAditionalCost[node];
    }

    public bool GetNodeTypesBloqued(NodeTypeCost node)
    {
        return NodeTypesBloqued[node];
    }
}