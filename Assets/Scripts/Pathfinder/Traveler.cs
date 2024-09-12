using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traveler : MonoBehaviour
{
    public enum Algorithm
    {
        DepthFirstPathfinder = 0,
        BreadthPathfinder,
        DijstraPathfinder,
        AStarPathfinder,
    }

    public Algorithm AlgorithmType;

    private Vector2IntGrapf<Node<Vector2Int>> grapf;
    [SerializeField] private GrapfView grapfView;
    private Pathfinder<Node<Vector2Int>, Vector2Int> Pathfinder;

    [SerializeField] private Vector2Int grid;
    [SerializeField] private int cellGap;
    [SerializeField] private int startNode;
    [SerializeField] private int endNode;

    void Start()
    {
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(grid.x, grid.y, cellGap, AlgorithmType);
        
        grapfView.SetGrapfView(grapf);
        
        switch (AlgorithmType)
        {
            case Algorithm.DepthFirstPathfinder:
                Pathfinder = new DepthFirstPathfinder<Node<Vector2Int>, Vector2Int>();
                break;
            case Algorithm.BreadthPathfinder:
                Pathfinder = new BreadthPathfinder<Node<Vector2Int>, Vector2Int>();
                break;
            case Algorithm.DijstraPathfinder:
                Pathfinder = new DijkstraPathfinder<Node<Vector2Int>, Vector2Int>();
                break;
            case Algorithm.AStarPathfinder:
                Pathfinder = new AStarPathfinder<Node<Vector2Int>, Vector2Int>();
                break;
        }
        
        List<Node<Vector2Int>> path = Pathfinder.FindPath(
            grapf.nodes[startNode],
            grapf.nodes[endNode], grapf);

        StartCoroutine(Move(path));
    }

    public IEnumerator Move(List<Node<Vector2Int>> path)
    {
        foreach (Node<Vector2Int> node in path)
        {
            transform.position = new Vector3(node.GetCoordinate().x, node.GetCoordinate().y);
            yield return new WaitForSeconds(1.0f);
        }
    }
}