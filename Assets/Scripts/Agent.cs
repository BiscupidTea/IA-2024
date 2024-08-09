using System;
using UnityEngine;
public enum Behaviour
{
    Chase, Patrol, Explode
}

public enum Flags
{
    OnTargetReach, OnTargetLost, OnTargetNear
}

public class Agent : MonoBehaviour
{
    private FSM fsm;

    [SerializeField] private Transform target;

    [SerializeField] private Transform WayPoint1;
    [SerializeField] private Transform WayPoint2;

    [SerializeField] private float speed;
    [SerializeField] private float explodeDistance;
    [SerializeField] private float lostDistance;



    private void Start()
    {
        fsm = new FSM(Enum.GetValues(typeof(Behaviour)).Length, Enum.GetValues(typeof(Flags)).Length);

        //Add chase Behaviour
        fsm.AddBehaviour<ChaseState>((int)Behaviour.Chase,
            onTickParameters: () => { return new object[] { transform, target, speed, explodeDistance, lostDistance }; });

        fsm.AddBehaviour<PatrolState>((int)Behaviour.Patrol,
            onTickParameters: () => { return new object[] { transform, target, speed, explodeDistance, lostDistance }; });

        fsm.AddBehaviour<ExplodeState>((int)Behaviour.Explode);

        //

    }

    private void Update()
    {
        fsm.Tick(transform);
    }
}
