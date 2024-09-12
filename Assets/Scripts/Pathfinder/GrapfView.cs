using System;
using UnityEngine;

public class GrapfView : MonoBehaviour
{
    private Vector2IntGrapf<Node<Vector2Int>> grapfh;

    [SerializeField] private GameObject prefabTownCenter;
    [SerializeField] private GameObject prefabGoldMine;
    [SerializeField] private GameObject prefabPlain;
    [SerializeField] private GameObject prefabMountain;
    
    public void SetGrapfView(Vector2IntGrapf<Node<Vector2Int>> grapfh)
    {
        this.grapfh = grapfh;
        
        SpawnVisibleGrid();
    }

    private void SpawnVisibleGrid()
    {
        foreach (Node<Vector2Int> node in grapfh)
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
                case NodeTypeCost.Plain:
                    prefab = prefabPlain;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Instantiate(prefab, new Vector3(node.GetCoordinate().x, node.GetCoordinate().y, 0), Quaternion.identity, transform);
        }
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        foreach (Node<Vector2Int> node in grapfh.nodes.Values)
        {
            if (node.GetBloqued())
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;
            
            Gizmos.DrawWireSphere(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y), 0.1f);

            foreach (Node<Vector2Int> neighborNode in grapfh.GetNeighborsNodes(node.GetId()))
            {
                Gizmos.color = Color.black;
                Gizmos.DrawLine(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y), new Vector3(neighborNode.GetCoordinate().x, neighborNode.GetCoordinate().y));
            }
        }
    }
}
