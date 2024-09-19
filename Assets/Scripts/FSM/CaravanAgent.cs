using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaravanAgent : Agent
{
    public float speed;

    public override void StartAgent(Grapf<Node<CoordinateType>, CoordinateType> grapfh, Node<CoordinateType> CU,
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

        fsm.SetTransition(Behaviours.Move, Flags.OnGoMine, Behaviours.Mining, () => { Debug.Log("Mining"); });
        fsm.SetTransition(Behaviours.Mining, Flags.OnInventoryFull, Behaviours.Move, () =>
        {
            Target = CU;
            StartPoint = Mine;
        });

        fsm.SetTransition(Behaviours.Move, Flags.OnGoTownCenter, Behaviours.Deposit,
            () => { Debug.Log("Depositing"); });
        fsm.SetTransition(Behaviours.Deposit, Flags.OnInventoryEmpty, Behaviours.Move, () =>
        {
            Target = Mine;
            StartPoint = CU;
        });
    }
}