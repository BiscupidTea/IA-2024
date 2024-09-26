using UnityEngine;

public enum Behaviours
{
    Move,
    Mining,
    Strike,
    Deposit,
    Refill,
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
    
    OnRefuge,
}

public class Agent : MonoBehaviour
{
    protected FSM<Behaviours, Flags> fsm;

    protected Node<CoordinateType> Target;
    protected Node<CoordinateType> StartPoint;

    protected Node<CoordinateType> CU;
    protected Node<CoordinateType> Mine;

    protected Flags flagToRaise;
    protected Grapf<Node<CoordinateType>, CoordinateType> grapfh;

    protected Traveler traveler;

    public virtual void StartAgent(Grapf<Node<CoordinateType>, CoordinateType> grapfh, Node<CoordinateType> CU,
        Node<CoordinateType> Mine)
    {
        fsm = new FSM<Behaviours, Flags>();

        fsm.AddBehaviour<IdleState>(Behaviours.Alarm);
        
        fsm.SetTransition(Behaviours.Move, Flags.OnRefuge, Behaviours.Alarm, () => { Debug.Log("Refuged"); });
    }
    
    public void SetNewMine(Node<CoordinateType> newMine)
    {
        Mine = newMine;
    }

    public virtual void AlarmSound()
    {

    }
}