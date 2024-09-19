using UnityEngine;

public class MinerAgent : Agent
{
    public float speed;
    public int GoldPickedEat;
    private MinerInventory minerInventory = new MinerInventory();

    public override void StartAgent(Grapf<Node<CoordinateType>, CoordinateType> grapfh, Node<CoordinateType> CU,
        Node<CoordinateType> Mine)
    {
        this.CU = CU;
        this.Mine = Mine;

        Target = this.Mine;
        StartPoint = this.CU;

        flagToRaise = Flags.OnStartMine;

        fsm = new FSM<Behaviours, Flags>();

        fsm.AddBehaviour<MoveState<Node<CoordinateType>, CoordinateType>>(Behaviours.Move,
            onEnterParameters: () => { return new object[] { grapfh, StartPoint, Target, flagToRaise }; },
            onTickParameters: () => { return new object[] { speed, transform }; });

        fsm.AddBehaviour<MiningState<Node<CoordinateType>, CoordinateType>>(Behaviours.Mining,
            onEnterParameters: () => { return new object[] { minerInventory }; },
            onTickParameters: () => { return new object[] { 0.5f, GoldPickedEat, minerInventory, Target }; });

        fsm.AddBehaviour<DepositGoldState>(Behaviours.Deposit,
            onTickParameters: () => { return new object[] { minerInventory }; });

        fsm.AddBehaviour<StrikeState<Node<CoordinateType>, CoordinateType>>(Behaviours.Strike,
            onTickParameters: () => { return new object[] { Target, minerInventory }; });

        fsm.SetTransition(Behaviours.Move, Flags.OnStartMine, Behaviours.Mining, () =>
        {
            Target = Mine;
            //Debug.Log("Mining");
        });
        fsm.SetTransition(Behaviours.Mining, Flags.OnInventoryFull, Behaviours.Move, () =>
        {
            Target = CU;
            StartPoint = Mine;
            flagToRaise = Flags.OnGoTownCenter;
           // Debug.Log("Go TownCenter, with: " + minerInventory.totalGold + "$ - and: " + minerInventory.totalFood +
           //           " of food");
        });

        fsm.SetTransition(Behaviours.Move, Flags.OnGoTownCenter, Behaviours.Deposit,
            () => { Debug.Log("Depositing"); });
        fsm.SetTransition(Behaviours.Deposit, Flags.OnInventoryEmpty, Behaviours.Move, () =>
        {
            Target = Mine;
            StartPoint = CU;
            flagToRaise = Flags.OnStartMine;
           // Debug.Log("Go Mining, with: " + minerInventory.totalGold + "$ - and: " + minerInventory.totalFood +
            //          " of food");
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