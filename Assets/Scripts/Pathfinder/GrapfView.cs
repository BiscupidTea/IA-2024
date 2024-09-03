using UnityEngine;

public class GrapfView : MonoBehaviour
{
    private Vector2IntGrapf<Node<Vector2Int>> grapfh;
    
    public void SetGrapfView(Vector2IntGrapf<Node<Vector2Int>> grapfh)
    {
        this.grapfh = grapfh;
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
