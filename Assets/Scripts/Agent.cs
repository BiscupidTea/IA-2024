using System;
using UnityEngine;
public enum Behaviours
{
    Chase, Patrol, Explode
}

public enum Flags
{
    OnTargetReach, OnTargetLost, OnTargetNear
}

public class Agent : MonoBehaviour
{
    private FSM<Behaviours, Flags> fsm;
    [SerializeField] private Transform target;

    [SerializeField] private Transform patrolPoint1;
    [SerializeField] private Transform patrolPoint2;

    [SerializeField] private float speed;
    [SerializeField] private float explodeDistance;
    [SerializeField] private float lostDistance;
    [SerializeField] private float chaseDistance;

    private void Start()
    {
        fsm = new FSM<Behaviours, Flags>();

        //Add states and transitions
        fsm.AddBehaviour<ChaseState>(Behaviours.Chase,
            onTickParameters: () => { return new object[] { transform, target, speed, explodeDistance, lostDistance }; });

        fsm.AddBehaviour<PatrolState>(Behaviours.Patrol,
            onTickParameters: () => { return new object[] { transform, patrolPoint1, patrolPoint2, target, speed, chaseDistance }; });

        fsm.AddBehaviour<ExplodeState>(Behaviours.Explode);

        fsm.SetTransition(Behaviours.Patrol, Flags.OnTargetNear, Behaviours.Chase, () => { Debug.Log("Te Vi!"); });
        fsm.SetTransition(Behaviours.Chase, Flags.OnTargetReach, Behaviours.Explode);
        fsm.SetTransition(Behaviours.Chase, Flags.OnTargetLost, Behaviours.Patrol);


        fsm.ForcedState(Behaviours.Patrol);
    }

    private void Update()
    {
        fsm.Tick();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, lostDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explodeDistance);
    }
}
