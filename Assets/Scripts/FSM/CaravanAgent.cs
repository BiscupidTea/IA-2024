using UnityEngine;

public class CaravanAgent : Agent
{
    public float speed;
    private CaravanInventory caravanInventory = new CaravanInventory();

    public override void StartAgent(Grapf<Node<CoordinateType>, CoordinateType> grapfh, Node<CoordinateType> CU,
        Node<CoordinateType> Mine)
    {
        base.StartAgent(grapfh, base.CU, base.Mine);

        this.CU = CU;
        this.Mine = Mine;

        Target = this.Mine;
        StartPoint = this.CU;

        flagToRaise = Flags.OnInventoryFull;
        this.grapfh = grapfh;

        traveler = new Traveler();

        traveler.NodeTypesAditionalCost.Add(NodeTypeCost.GoldMine, 0);
        traveler.NodeTypesAditionalCost.Add(NodeTypeCost.TownCenter, 0);
        traveler.NodeTypesAditionalCost.Add(NodeTypeCost.Mountain, 0);
        traveler.NodeTypesAditionalCost.Add(NodeTypeCost.Plateau, 10);
        traveler.NodeTypesAditionalCost.Add(NodeTypeCost.Plain, 50);

        traveler.NodeTypesBloqued.Add(NodeTypeCost.GoldMine, false);
        traveler.NodeTypesBloqued.Add(NodeTypeCost.TownCenter, false);
        traveler.NodeTypesBloqued.Add(NodeTypeCost.Mountain, true);
        traveler.NodeTypesBloqued.Add(NodeTypeCost.Plateau, false);
        traveler.NodeTypesBloqued.Add(NodeTypeCost.Plain, false);

        fsm.AddBehaviour<MoveState<Node<CoordinateType>, CoordinateType>>(Behaviours.Move,
            onEnterParameters: () => { return new object[] { grapfh, StartPoint, Target, flagToRaise, traveler }; },
            onTickParameters: () => { return new object[] { speed, transform }; });

        fsm.AddBehaviour<DepositFoodState<Node<CoordinateType>, CoordinateType>>(Behaviours.Deposit,
            onTickParameters: () => { return new object[] { Target, caravanInventory }; });

        fsm.AddBehaviour<RepositFoodState>(Behaviours.Refill,
            onTickParameters: () => { return new object[] { caravanInventory }; });

        fsm.AddBehaviour<WaitToCallFoodState<Node<CoordinateType>, CoordinateType>>(Behaviours.WaitCall,
            onTickParameters: () => { return new object[] { Target }; });


        fsm.SetTransition(Behaviours.Move, Flags.OnInventoryFull, Behaviours.Deposit,
            () => { Debug.Log("Depositiong Food"); });
        fsm.SetTransition(Behaviours.Deposit, Flags.OnInventoryEmpty, Behaviours.Move, () =>
        {
            flagToRaise = Flags.OnInventoryEmpty;
            Target = CU;
            StartPoint = this.Mine;
            Debug.Log("Go to town center with " + caravanInventory.totalFood + " of food");
        });

        fsm.SetTransition(Behaviours.Move, Flags.OnInventoryEmpty, Behaviours.Refill,
            () => { Debug.Log("Reffil Inventory"); });

        fsm.SetTransition(Behaviours.Refill, Flags.OnInventoryFull, Behaviours.WaitCall,
            () =>
            {
                Debug.Log("go deposit new  Food");
                flagToRaise = Flags.OnInventoryFull;
                Target = this.Mine;
                StartPoint = CU;
            });
        fsm.SetTransition(Behaviours.WaitCall, Flags.OnInventoryFull, Behaviours.Move,
            () => { Debug.Log("Go to mine with " + caravanInventory.totalFood + " of food"); });

        fsm.ForcedState(Behaviours.WaitCall);
    }

    public override void AlarmSound()
    {
        if (fsm.currentState != (int)Behaviours.Alarm)
        {
            StartPoint = grapfh.SerchNearNode(transform.position.x, transform.position.y);
            Target = CU;

            flagToRaise = Flags.OnRefuge;

            fsm.ForcedState(Behaviours.Move);
        }
        else
        {
            StartPoint = grapfh.SerchNearNode(transform.position.x, transform.position.y);
            Target = Mine;

            flagToRaise = Flags.OnInventoryFull;

            fsm.ForcedState(Behaviours.Move);
        }
    }

    private void Update()
    {
        fsm.Tick();
    }
}