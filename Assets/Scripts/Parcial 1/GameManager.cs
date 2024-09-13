using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Grapf<Node<CoordinateType>, CoordinateType> grapf;

    [SerializeField] private Vector2Int grid;
    [SerializeField] private GrapfView grapfView;
    private Pathfinder<Node<CoordinateType>, CoordinateType> Pathfinder;

    [SerializeField] private int cellGap;

    public int goldMineCuantity;
    public int minersCuantity;

    [SerializeField] private GameObject minerPrefab;
    private Node<Vector2Int> townCenter;
    private Node<Vector2Int> mine;

    private List<Agent> miners;

    private void Start()
    {
        Pathfinder = new AStarPathfinder<Node<CoordinateType>, CoordinateType>();

        grapf = new Grapf<Node<CoordinateType>, CoordinateType>(grid.x, grid.y, cellGap, Algorithm.AStarPathfinder);

        SetNodesData(grapf);

        grapfView.SetGrapfView(grapf);

        SetMainers(grapf);
    }


    private void SetNodesData(Grapf<Node<CoordinateType>, CoordinateType> grapfh)
    {
        Node<CoordinateType> currentNode;

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
        foreach ( Node<CoordinateType> node in grapfh)
        {
            if (node.GetNodeType() == NodeTypeCost.None)
            {
                node.SetNodeType(NodeTypeCost.Plain);
            }
        }
    }

    private void SetMainers( Grapf<Node<CoordinateType>,CoordinateType> grapfh)
    {
        for (int i = 0; i < minersCuantity; i++)
        {
            GameObject currentMiner = Instantiate(minerPrefab.gameObject,
                new Vector3(townCenter.GetCoordinate().x, townCenter.GetCoordinate().y, 0),
                Quaternion.identity, transform);

            miners.Add(currentMiner.GetComponent<Agent>());
        }

        foreach (Agent currentMiner in miners)
        {
            currentMiner.StartMiner(grapf);
        }
    }
}