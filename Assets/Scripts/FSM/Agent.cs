using System;
using UnityEngine;
public enum Behaviours
{
    Move, Mining, Strike, Deposit
}

public enum Flags
{
    OnDepositGold, OnMining, OnInventoryFull, OnInventoryEmpty, OnRequiresFood
}

public class Agent<NodeType> : MonoBehaviour where NodeType : INode<Vector2Int>
{
    private FSM<Behaviours, Flags> fsm;

    public Transform[] PathPoints;

    public NodeType Target;
    
    public NodeType CU;
    public NodeType Mine;

    [SerializeField] private float speed;
    [SerializeField] private float explodeDistance;
    [SerializeField] private float lostDistance;
    [SerializeField] private float chaseDistance;

    private void Start()
    {
        fsm = new FSM<Behaviours, Flags>();

        fsm.AddBehaviour<MoveState<NodeType>>(Behaviours.Move,
            onEnterParameters: () => { return new object[] { Target }; },
            onTickParameters: () => { return new object[] { speed, transform, PathPoints}; });
        
        fsm.AddBehaviour<MiningState>(Behaviours.Mining,
            onTickParameters: () => { return new object[] { 0.5f, 3}; });
        
        fsm.AddBehaviour<DepositState>(Behaviours.Deposit,
            onTickParameters: () => { return new object[] { 0.5f, 3}; });

        fsm.SetTransition(Behaviours.Move, Flags.OnMining, Behaviours.Mining);
        fsm.SetTransition(Behaviours.Mining, Flags.OnInventoryFull, Behaviours.Move, () => { Target = CU;});
        
        fsm.SetTransition(Behaviours.Move, Flags.OnDepositGold, Behaviours.Deposit);
        fsm.SetTransition(Behaviours.Deposit, Flags.OnInventoryEmpty, Behaviours.Move, () => { Target = Mine;});
        
        fsm.SetTransition(Behaviours.Mining, Flags.OnRequiresFood, Behaviours.Strike);
        fsm.SetTransition(Behaviours.Strike, Flags.OnRequiresFood, Behaviours.Mining);

        fsm.ForcedState(Behaviours.Move);
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
