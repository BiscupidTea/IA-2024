using UnityEngine;

public class CaravanAgent : Agent
{
    public float speed;
    private CaravanInventory caravanInventory = new CaravanInventory();

    public override void StartAgent(Grapf<Node<CoordinateType>, CoordinateType> grapfh, Node<CoordinateType> CU,
        Node<CoordinateType> Mine)
    {
        this.CU = CU;
        this.Mine = Mine;

        Target = this.Mine;
        StartPoint = this.CU;

        flagToRaise = Flags.OnInventoryFull;

        fsm = new FSM<Behaviours, Flags>();

        fsm.AddBehaviour<MoveState<Node<CoordinateType>, CoordinateType>>(Behaviours.Move,
            onEnterParameters: () => { return new object[] { grapfh, StartPoint, Target, flagToRaise }; },
            onTickParameters: () => { return new object[] { speed, transform }; });

        fsm.AddBehaviour<DepositFoodState<Node<CoordinateType>, CoordinateType>>(Behaviours.Deposit,
            onTickParameters: () => { return new object[] { Target, caravanInventory }; });

        fsm.AddBehaviour<RepositFoodState>(Behaviours.Refill,
            onTickParameters: () => { return new object[] { caravanInventory }; });

        fsm.SetTransition(Behaviours.Move, Flags.OnInventoryFull, Behaviours.Deposit,
            () => { Debug.Log("Depositiong Food"); });
        fsm.SetTransition(Behaviours.Deposit, Flags.OnInventoryEmpty, Behaviours.Move, () =>
        {
            flagToRaise = Flags.OnInventoryEmpty;
            Target = CU;
            StartPoint = Mine;
            Debug.Log("Go to town center with " + caravanInventory.totalFood + " of food");
        });

        fsm.SetTransition(Behaviours.Move, Flags.OnInventoryEmpty, Behaviours.Refill,
            () => { Debug.Log("Reffil Inventory"); });
        fsm.SetTransition(Behaviours.Refill, Flags.OnInventoryFull, Behaviours.Move, () =>
        {
            flagToRaise = Flags.OnInventoryFull;
            Target = Mine;
            StartPoint = CU;
            Debug.Log("Go to mine with " + caravanInventory.totalFood + " of food");
        });

        fsm.ForcedState(Behaviours.Move);
    }

    private void Update()
    {
        fsm.Tick();
    }
}