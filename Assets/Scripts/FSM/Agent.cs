using System;
using UnityEditor.Compilation;
using UnityEngine;

public enum Behaviours
{
    Move,
    Mining,
    Strike,
    Deposit,
    Alarm,
}

public enum Flags
{
    OnGoTownCenter,
    OnStartMine,

    OnInventoryFull,
    OnInventoryEmpty,

    OnRequiresFood,
    OnEndStrike,
}

public class Agent : MonoBehaviour
{
    protected FSM<Behaviours, Flags> fsm;

    protected Node<CoordinateType> Target;
    protected Node<CoordinateType> StartPoint;

    protected Node<CoordinateType> CU;
    protected Node<CoordinateType> Mine;

    protected Flags flagToRaise;

    public virtual void StartAgent(Grapf<Node<CoordinateType>, CoordinateType> grapfh, Node<CoordinateType> CU,
        Node<CoordinateType> Mine)
    {
    }

    public void AlarmSound()
    {
        if (fsm.currentState != (int)Behaviours.Alarm)
        {
            fsm.ForcedState(Behaviours.Alarm);
        }
    }
}