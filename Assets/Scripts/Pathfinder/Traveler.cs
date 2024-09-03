using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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
    private Pathfinder<Node<Vector2Int>, Vector2Int> Pathfinder;

    void Start()
    {
        grapf = new Vector2IntGrapf<Node<Vector2Int>>(10, 10);

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
            grapf.nodes[Random.Range(0, grapf.nodes.Count)],
            grapf.nodes[Random.Range(0, grapf.nodes.Count)], grapf);

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