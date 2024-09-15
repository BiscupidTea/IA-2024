using System;
using UnityEditor.Compilation;
using UnityEngine;

public enum Behaviours
{
    Move,
    Mining,
    Strike,
    Deposit
}

public enum Flags
{
    OnDepositGold,
    OnMining,
    
    OnInventoryFull,
    OnInventoryEmpty,
    
    OnRequiresFood,
    OnEndStrike
}

public class Agent : MonoBehaviour
{
    private FSM<Behaviours, Flags> fsm;

    public Node<CoordinateType> Target;
    public Node<CoordinateType> StartPoint;

    public Node<CoordinateType> CU;
    public Node<CoordinateType> Mine;

    [SerializeField] private float speed;
    [SerializeField] private int GoldPickedEat;
    private MinerInventory minerInventory = new MinerInventory();
    
    public void StartMiner(Grapf<Node<CoordinateType>, CoordinateType> grapfh, Node<CoordinateType> CU,
        Node<CoordinateType> Mine)
    {
        this.CU = CU;
        this.Mine = Mine;

        Target = this.Mine;
        StartPoint = this.CU;

        fsm = new FSM<Behaviours, Flags>();

        fsm.AddBehaviour<MoveState<Node<CoordinateType>, CoordinateType>>(Behaviours.Move,
            onEnterParameters: () => { return new object[] { grapfh, StartPoint, Target }; },
            onTickParameters: () => { return new object[] { speed, transform }; });

        fsm.AddBehaviour<MiningState>(Behaviours.Mining,
            onEnterParameters: () => { return new object[] { minerInventory }; },
            onTickParameters: () => { return new object[] { 0.5f, GoldPickedEat, minerInventory }; });

        fsm.AddBehaviour<DepositState>(Behaviours.Deposit,
            onTickParameters: () => { return new object[] { minerInventory }; });
        
        fsm.AddBehaviour<StrikeState>(Behaviours.Strike,
            onTickParameters: () => { return new object[] { minerInventory }; });

        fsm.SetTransition(Behaviours.Move, Flags.OnMining, Behaviours.Mining, () => { Debug.Log("Mining"); });
        fsm.SetTransition(Behaviours.Mining, Flags.OnInventoryFull, Behaviours.Move, () =>
        {
            Target = CU;
            StartPoint = Mine;
            Debug.Log("Go TownCenter, with: " + minerInventory.totalGold + "$ - and: " + minerInventory.totalFood + " of food");
        });

        fsm.SetTransition(Behaviours.Move, Flags.OnDepositGold, Behaviours.Deposit, () => { Debug.Log("Depositing"); });
        fsm.SetTransition(Behaviours.Deposit, Flags.OnInventoryEmpty, Behaviours.Move, () =>
        {
            Target = Mine;
            StartPoint = CU;
            Debug.Log("Go Mining, with: " + minerInventory.totalGold + "$ - and: " + minerInventory.totalFood + " of food");
        });

        fsm.SetTransition(Behaviours.Mining, Flags.OnRequiresFood, Behaviours.Strike, () => { Debug.Log("Strike!"); });
        fsm.SetTransition(Behaviours.Strike, Flags.OnEndStrike, Behaviours.Mining, () => { Debug.Log("Mining"); });

        fsm.ForcedState(Behaviours.Move);
    }

    private void Update()
    {
        fsm.Tick();
    }
}