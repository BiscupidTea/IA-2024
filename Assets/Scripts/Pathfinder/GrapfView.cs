using System;
using UnityEngine;

public class GrapfView : MonoBehaviour
{
    private Grapf<Node<CoordinateType>, CoordinateType> grapfh;

    [SerializeField] private bool SeeCells;

    [SerializeField] private GameObject prefabTownCenter;
    [SerializeField] private GameObject prefabGoldMine;
    [SerializeField] private GameObject prefabPlain;
    [SerializeField] private GameObject prefabPlateau;
    [SerializeField] private GameObject prefabMountain;

    public void SetGrapfView(Grapf<Node<CoordinateType>, CoordinateType> grapfh)
    {
        this.grapfh = grapfh;

        SpawnVisibleGrid();
    }

    private void SpawnVisibleGrid()
    {
        foreach (Node<CoordinateType> node in grapfh)
        {
            GameObject prefab = prefabPlain;

            switch (node.GetNodeType())
            {
                case NodeTypeCost.None:
                    break;
                case NodeTypeCost.GoldMine:
                    prefab = prefabGoldMine;
                    break;
                case NodeTypeCost.TownCenter:
                    prefab = prefabTownCenter;
                    break;
                case NodeTypeCost.Mountain:
                    prefab = prefabMountain;
                    break;
                case NodeTypeCost.Plateau:
                    prefab = prefabPlateau;
                    break;
                case NodeTypeCost.Plain:
                    prefab = prefabPlain;
                    break;
            }

            Instantiate(prefab, new Vector3(node.GetCoordinate().GetXY()[0], node.GetCoordinate().GetXY()[1], 1),
                Quaternion.identity, transform);
        }
    }

    private void OnDrawGizmos()
    {
        if (SeeCells)
        {
            foreach (Node<CoordinateType> node in grapfh.nodes.Values)
            {
                if (node.GetBloqued())
                    Gizmos.color = Color.red;
                else if (node.GetNodeCost() > 0)
                    Gizmos.color = Color.yellow;
                else
                    Gizmos.color = Color.green;

                Gizmos.DrawWireSphere(new Vector3(node.GetCoordinate().GetXY()[0], node.GetCoordinate().GetXY()[1]),
                    0.1f);

                foreach (Node<CoordinateType> neighborNode in grapfh.GetNeighborsNodes(node.GetId()))
                {
                    Gizmos.color = Color.black;
                    Gizmos.DrawLine(new Vector3(node.GetCoordinate().GetXY()[0], node.GetCoordinate().GetXY()[1]),
                        new Vector3(neighborNode.GetCoordinate().GetXY()[0], neighborNode.GetCoordinate().GetXY()[1]));
                }
            }
        }
    }
}