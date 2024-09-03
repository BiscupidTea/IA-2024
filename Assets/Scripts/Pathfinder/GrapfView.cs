using UnityEngine;

public class GrapfView : MonoBehaviour
{
    private Vector2IntGrapf<Node<Vector2Int>> grapf;

    void Start(Vector2IntGrapf<Node<Vector2Int>> grapf)
    {
        this.grapf = grapf;
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;
        foreach (Node<Vector2Int> node in grapf.nodes.Values)
        {
            if (node.GetBloqued())
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;
            
            Gizmos.DrawWireSphere(new Vector3(node.GetCoordinate().x, node.GetCoordinate().y), 0.1f);
        }
    }
}
