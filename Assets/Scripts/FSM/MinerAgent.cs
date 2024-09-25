using System.Collections.Generic;
using UnityEngine;

public class MinerAgent : Agent
{
    public float speed;
    public int GoldPickedEat;
    private MinerInventory minerInventory = new MinerInventory();

    public override void StartAgent(Grapf<Node<CoordinateType>, CoordinateType> grapfh, Node<CoordinateType> CU,
        Node<CoordinateType> Mine)
    {
        base.StartAgent(grapfh, base.CU, base.Mine);
        
        this.CU = CU;
        this.Mine = Mine;

        Target = this.Mine;
        StartPoint = this.CU;

        flagToRaise = Flags.OnStartMine;
        this.grapfh = grapfh;

        Dictionary<NodeTypeCost, int> NodeTypesAditionalCost;
        Dictionary<NodeTypeCost, bool> NodeTypesBloqued;
        
        traveler = new Traveler();
        
        traveler.NodeTypesAditionalCost.Add(NodeTypeCost.GoldMine, 0);
        traveler.NodeTypesAditionalCost.Add(NodeTypeCost.TownCenter, 0);
        traveler.NodeTypesAditionalCost.Add(NodeTypeCost.Mountain, 0);
        traveler.NodeTypesAditionalCost.Add(NodeTypeCost.Plateau, 0);
        traveler.NodeTypesAditionalCost.Add(NodeTypeCost.Plain, 0);
        
        traveler.NodeTypesBloqued.Add(NodeTypeCost.GoldMine, false);
        traveler.NodeTypesBloqued.Add(NodeTypeCost.TownCenter, false);
        traveler.NodeTypesBloqued.Add(NodeTypeCost.Mountain, true);
        traveler.NodeTypesBloqued.Add(NodeTypeCost.Plateau, false);
        traveler.NodeTypesBloqued.Add(NodeTypeCost.Plain, false);
        
        fsm.AddBehaviour<MoveState<Node<CoordinateType>, CoordinateType>>(Behaviours.Move,
            onEnterParameters: () => { return new object[] { grapfh, StartPoint, Target, flagToRaise, traveler }; },
            onTickParameters: () => { return new object[] { speed, transform }; });

        fsm.AddBehaviour<MiningState<Node<CoordinateType>, CoordinateType>>(Behaviours.Mining,
            onEnterParameters: () => { return new object[] { minerInventory }; },
            onTickParameters: () => { return new object[] { 0.5f, GoldPickedEat, minerInventory, Target }; });

        fsm.AddBehaviour<DepositGoldState>(Behaviours.Deposit,
            onTickParameters: () => { return new object[] { minerInventory }; });

        fsm.AddBehaviour<StrikeState<Node<CoordinateType>, CoordinateType>>(Behaviours.Strike,
            onTickParameters: () => { return new object[] { Target, minerInventory }; });
        
        fsm.AddBehaviour<IdleState>(Behaviours.Alarm);

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
            
            flagToRaise = Flags.OnStartMine;
            
            fsm.ForcedState(Behaviours.Move);
        }
    }

    private void Update()
    {
        fsm.Tick();
    }
}