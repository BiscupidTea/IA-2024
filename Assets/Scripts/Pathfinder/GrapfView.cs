using System;
using UnityEngine;

public class GrapfView : MonoBehaviour
{
    private Grapf<Node<CoordinateType>, CoordinateType> grapfh;

    [SerializeField] private bool SeeCells;
    [SerializeField] private bool SeeMap;

    [SerializeField] private GameObject prefabTownCenter;
    [SerializeField] private GameObject prefabGoldMine;
    [SerializeField] private GameObject prefabPlain;
    [SerializeField] private GameObject prefabPlateau;
    [SerializeField] private GameObject prefabMountain;

    [SerializeField] private Mesh baseMesh;
    [SerializeField] private Material MaterialTownCenter;
    [SerializeField] private Material MaterialGoldMine;
    [SerializeField] private Material MaterialPlain;
    [SerializeField] private Material MaterialPlateau;
    [SerializeField] private Material MaterialMountain;

    public void SetGrapfView(Grapf<Node<CoordinateType>, CoordinateType> grapfh)
    {
        this.grapfh = grapfh;

        SeeMap = true;
    }

    private void LateUpdate()
    {
        if (SeeMap)
        {
            Quaternion rotation = Quaternion.Euler(-90, 0, 0);
            
            foreach (Node<CoordinateType> node in grapfh)
            {
                Matrix4x4 matrix =
                    Matrix4x4.TRS(new Vector3(node.GetCoordinate().GetXY()[0], node.GetCoordinate().GetXY()[1], 1),
                        rotation, new Vector3(0.1f, 0.1f, 0.1f));

                Mesh mesh = baseMesh;
                Material material = MaterialPlain;

                switch (node.GetNodeType())
                {
                    case NodeTypeCost.None:
                        break;
                    case NodeTypeCost.GoldMine:
                        material = MaterialGoldMine;
                        break;
                    case NodeTypeCost.TownCenter:
                        material = MaterialTownCenter;
                        break;
                    case NodeTypeCost.Mountain:
                        material = MaterialMountain;
                        break;
                    case NodeTypeCost.Plateau:
                        material = MaterialPlateau;
                        break;
                    case NodeTypeCost.Plain:
                        material = MaterialPlain;
                        break;
                }

                Graphics.DrawMesh(mesh, matrix, material, 0);
            }
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