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

public class Agent : MonoBehaviour
{
    private FSM<Behaviours, Flags> fsm;

    public Transform[] PathPoints;

    public INode<Vector2Int> Target;
    
    public INode<Vector2Int> CU;
    public INode<Vector2Int> Mine;

    [SerializeField] private float speed;

    public void StartMiner( Grapf<Node<CoordinateType>,CoordinateType> grapfh)
    {
        fsm = new FSM<Behaviours, Flags>();

        fsm.AddBehaviour<MoveState<Node<CoordinateType>,CoordinateType>>(Behaviours.Move,
            onEnterParameters: () => { return new object[] { Target, grapfh }; },
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
}
