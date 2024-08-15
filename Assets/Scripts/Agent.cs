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
    private FSM fsm;

    [SerializeField] private Transform target;

    [SerializeField] private Transform patrolPoint1;
    [SerializeField] private Transform patrolPoint2;

    [SerializeField] private float speed;
    [SerializeField] private float explodeDistance;
    [SerializeField] private float lostDistance;
    [SerializeField] private float chaseDistance;

    private void Start()
    {
        fsm = new FSM(Enum.GetValues(typeof(Behaviours)).Length, Enum.GetValues(typeof(Flags)).Length);

        //Add states and transitions
        fsm.AddBehaviour<ChaseState>((int)Behaviours.Chase,
            onTickParameters: () => { return new object[] { transform, target, speed, explodeDistance, lostDistance }; });
        
        fsm.SetTransition((int)Behaviours.Chase, (int)Flags.OnTargetReach, (int)Behaviours.Explode);
        fsm.SetTransition((int)Behaviours.Chase, (int)Flags.OnTargetLost, (int)Behaviours.Patrol);

        fsm.AddBehaviour<PatrolState>((int)Behaviours.Patrol,
            onTickParameters: () => { return new object[] { transform, patrolPoint1, patrolPoint2, target, speed, chaseDistance }; });
        
        fsm.SetTransition((int)Behaviours.Patrol, (int)Flags.OnTargetNear, (int)Behaviours.Chase);

        fsm.AddBehaviour<ExplodeState>((int)Behaviours.Explode);
        
        fsm.ForcedState((int)Behaviours.Patrol);
    }

    private void Update()
    {
        fsm.Tick(transform);
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
