using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Vector2IntGrapf<Node<Vector2Int>> grapf;

    [SerializeField] private Vector2Int grid;
    [SerializeField] private GrapfView grapfView;
    private Pathfinder<Node<Vector2Int>, Vector2Int> Pathfinder;
    
    [SerializeField] private int cellGap;

    public int goldMineCuantity;

    private void Start()
    {
        Pathfinder = new AStarPathfinder<Node<Vector2Int>, Vector2Int>();

        grapf = new Vector2IntGrapf<Node<Vector2Int>>(grid.x, grid.y, cellGap, Traveler.Algorithm.AStarPathfinder);
        
        SetNodesData(grapf);

        grapfView.SetGrapfView(grapf);
    }
    
    
    private void SetNodesData(Vector2IntGrapf<Node<Vector2Int>> grapfh)
    {
        Node<Vector2Int> currentNode;
        
        //Set Town Center
        currentNode = grapf.nodes[Random.Range(0, grapf.nodes.Count)];

        if (currentNode.GetNodeType() == NodeTypeCost.None)
        {
            currentNode.SetNodeType(NodeTypeCost.TownCenter);
        }
        
        //Set Gold Mine
        for (int i = 0; i < goldMineCuantity; i++)
        {
            currentNode = grapf.nodes[Random.Range(0, grapf.nodes.Count)];

            if (currentNode.GetNodeType() == NodeTypeCost.None)
            {
                currentNode.SetNodeType(NodeTypeCost.GoldMine);
            }
        }
        
        //Set Mountains

        //Set Default Nodes Type
        foreach (Node<Vector2Int> node in grapfh)
        {
            if (node.GetNodeType() == NodeTypeCost.None)
            {
                node.SetNodeType(NodeTypeCost.Plain);
            }
        }
    }
}
