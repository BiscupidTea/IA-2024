using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

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

    private VoronoidController<CoordinateType> voronoid;

    public void SetGrapfView(Grapf<Node<CoordinateType>, CoordinateType> grapfh, VoronoidController<CoordinateType> voronoid)
    {
        this.grapfh = grapfh;
        this.voronoid = voronoid;
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
        if (!Application.isPlaying)
            return;


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

        for (int i = 0; i < voronoid.mines.Count; i++)
        {
            Gizmos.DrawCube(new Vector3(voronoid.mines[i].position.GetXY()[0], voronoid.mines[i].position.GetXY()[1], 0), Vector3.one / 2);
        }

        Gizmos.color = Color.magenta;
        for (int i = 0; i < voronoid.vertex.Count; i++)
        {
            //Vector3 current = new Vector3(voronoid.vertex[i].GetXY()[0], voronoid.vertex[i].GetXY()[i+1], 0);
            //Vector3 next = new Vector3(voronoid.vertex[i+1 % voronoid.mines[i].poligon.vertices.Count].GetXY()[0], voronoid.vertex[i + 1% voronoid.mines[i].poligon.vertices.Count].GetXY()[i+2], 0);
            //Gizmos.DrawLine(current, next);
        }

        Gizmos.color = UnityEngine.Color.black;
        for (int j = 0; j < voronoid.mines.Count; j++)
        {
            for (int i = 0; i < voronoid.mines[j].poligon.vertices.Count; i++)
            {
               // Vector3 vertex1 = new Vector3(voronoid.mines[j].poligon.vertices[i].GetXY()[0], voronoid.mines[j].poligon.vertices[i].GetXY()[1]);
               // Vector3 vertex2 = new Vector3(voronoid.mines[j].poligon.vertices[i+1 % voronoid.mines[j].poligon.vertices.Count].GetXY()[0], voronoid.mines[j].poligon.vertices[i+1 % voronoid.mines[j].poligon.vertices.Count].GetXY()[1]);
               // Gizmos.DrawLine(vertex1, vertex2);
            }
        }
    }
}