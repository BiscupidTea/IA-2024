using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public struct BehavioursActions
{
    private Dictionary<int, List<Action>> mainThreadBehaviour;
    private ConcurrentDictionary<int, ConcurrentBag<Action>> multithreadablesBehaviour;
    private Action transitionBehaviour;

    public void AddMainThreadBehaviour(int executionOrder, Action behaviour)
    {
        if (mainThreadBehaviour == null)
        {
            mainThreadBehaviour = new Dictionary<int, List<Action>>();
        }

        if (!mainThreadBehaviour.ContainsKey(executionOrder))
        {
            mainThreadBehaviour.Add(executionOrder, new List<Action>());
        }

        mainThreadBehaviour[executionOrder].Add(behaviour);
    }

    public void AddMultitreadableBehaviours(int executionOrder, Action behaviour)
    {
        if (multithreadablesBehaviour == null)
        {
            multithreadablesBehaviour = new ConcurrentDictionary<int, ConcurrentBag<Action>>();
        }

        if (!multithreadablesBehaviour.ContainsKey(executionOrder))
        {
            multithreadablesBehaviour.TryAdd(executionOrder, new ConcurrentBag<Action>());
        }

        multithreadablesBehaviour[executionOrder].Add(behaviour);
    }

    public void SetTransitionBehaviour(Action behaviour)
    {
        transitionBehaviour = behaviour;
    }

    public Dictionary<int, List<Action>> MainThreadBehaviour => mainThreadBehaviour;
    public ConcurrentDictionary<int, ConcurrentBag<Action>> MultithreadablesBehaviour => multithreadablesBehaviour;
    public Action TransitionBehaviour => transitionBehaviour;
}

public abstract class State
{
    public Action<Enum> OnFlag;
    public abstract BehavioursActions GetOnEnterbehaviour(params object[] parameters);
    public abstract BehavioursActions GetTickbehaviour(params object[] parameters);
    public abstract BehavioursActions GetOnExitbehaviour(params object[] parameters);
}

public sealed class MoveState<NodeType, CoordType> : State
    where NodeType : INode<CoordType>, new()
    where CoordType : IEquatable<CoordType>, ICoordType<int>, new()
{
    private int currentTargetPoint;
    private List<NodeType> nodesPath;
    private NodeType target;
    private NodeType FirstNode;
    private NodeType CurrentNode;
    private Flags flagToRaise;
    private Traveler traveler;

    private Pathfinder<NodeType, CoordType> Pathfinder = new AStarPathfinder<NodeType, CoordType>();
    private Grapf<NodeType, CoordType> grapfh;

    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMultitreadableBehaviours(0, () =>
        {
            List<CoordType> a = new List<CoordType>();
            grapfh = parameters[0] as Grapf<NodeType, CoordType>;
            FirstNode = (NodeType)parameters[1];
            target = (NodeType)parameters[2];
            currentTargetPoint = 0;
            flagToRaise = (Flags)parameters[3];
            traveler = (Traveler)parameters[4];
            CurrentNode = FirstNode;
            nodesPath = Pathfinder.FindPath(FirstNode, target, this.grapfh, traveler);
        });

        return behaviour;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        float speed = Convert.ToSingle(parameters[0]);
        Transform ownerTransform = parameters[1] as Transform;

        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMainThreadBehaviour(0, () =>
        {
            try
            {
                if (Vector2.Distance(ownerTransform.position,
                        new Vector2(nodesPath[currentTargetPoint].GetCoordinate().GetXY()[0],
                            nodesPath[currentTargetPoint].GetCoordinate().GetXY()[1])) < 0.1)
                {
                    CurrentNode = nodesPath[currentTargetPoint];
                    currentTargetPoint++;
                }
                else
                {
                    ownerTransform.position +=
                        (new Vector3(nodesPath[currentTargetPoint].GetCoordinate().GetXY()[0],
                            nodesPath[currentTargetPoint].GetCoordinate().GetXY()[1], 0) - ownerTransform.position)
                        .normalized *
                        speed * Time.deltaTime;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });

        behaviour.SetTransitionBehaviour(() =>
        {
            if (currentTargetPoint >= nodesPath.Count)
            {
                OnFlag?.Invoke(flagToRaise);
            }
        });

        return behaviour;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        return default;
    }
}

public sealed class IdleState : State
{
    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        return default;
    }
}

//Miner

public sealed class MiningState<NodeType, CoordType> : State
    where NodeType : class, INode<CoordType>, new()
    where CoordType : IEquatable<CoordType>, ICoordType<int>, new()
{
    private int totalGold;
    private int goldForFood;
    private int totalFood;

    private float timer;
    bool noGold = false;

    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMultitreadableBehaviours(0, () =>
        {
            noGold = false;
            timer = 0;
            goldForFood = 0;
            totalGold = (parameters[0] as MinerInventory).totalGold;
            totalFood = (parameters[0] as MinerInventory).totalFood;

            TileClass newtileclass = (parameters[1] as NodeType).GetTileClass();

            if (newtileclass is MineInventory mineInventory)
            {
                mineInventory.haveMiner = true;
                Debug.Log(mineInventory.haveMiner);
            }
        });

        return behaviour;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        float MineTime = Convert.ToSingle(parameters[0]);
        int goldBeforeFood = Convert.ToInt32(parameters[1]);

        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMainThreadBehaviour(0, () =>
        {
            timer += Time.deltaTime;

            if (timer >= MineTime)
            {
                TileClass newtileclass = (parameters[3] as NodeType).GetTileClass();

                if (newtileclass is MineInventory mineInventory)
                {
                    if (mineInventory.totalGold <= 0)
                    {
                        noGold = true;
                    }
                    else
                    {
                        mineInventory.totalGold -= 1;
                        totalGold += 1;
                        goldForFood += 1;
                        timer = 0;
                    }
                }
            }

            if (goldForFood >= goldBeforeFood)
            {
                goldForFood = 0;
                totalFood -= 1;
            }
        });

        behaviour.SetTransitionBehaviour(() =>
        {
            if (totalGold >= 15 || noGold)
            {
                (parameters[2] as MinerInventory).totalGold = totalGold;
                (parameters[2] as MinerInventory).totalFood = totalFood;

                OnFlag?.Invoke(Flags.OnInventoryFull);
            }

            if (totalFood <= 0)
            {
                (parameters[2] as MinerInventory).totalGold = totalGold;
                (parameters[2] as MinerInventory).totalFood = totalFood;
                OnFlag?.Invoke(Flags.OnRequiresFood);
            }
        });

        return behaviour;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMultitreadableBehaviours(0, () =>
        {
            TileClass newtileclass = (parameters[0] as NodeType).GetTileClass();

            if (newtileclass is MineInventory mineInventory)
            {
                mineInventory.haveMiner = false;
                Debug.Log(mineInventory.haveMiner);
            }
        });

        return behaviour;
    }
}

public sealed class DepositGoldState : State
{
    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMultitreadableBehaviours(0, () => { (parameters[0] as MinerInventory).totalGold = 0; });

        behaviour.SetTransitionBehaviour(() =>
        {
            if ((parameters[0] as MinerInventory).totalGold == 0)
            {
                OnFlag?.Invoke(Flags.OnInventoryEmpty);
            }
        });

        return behaviour;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        return default;
    }
}

public sealed class StrikeState<NodeType, CoordType> : State
    where NodeType : class, INode<CoordType>, new()
    where CoordType : IEquatable<CoordType>, ICoordType<int>, new()
{
    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();


        behaviour.AddMultitreadableBehaviours(0, () =>
        {
            TileClass newtileclass = (parameters[0] as NodeType).GetTileClass();

            if (newtileclass is MineInventory mineInventory)
            {
                if (mineInventory.totalFood >= 1)
                {
                    mineInventory.totalFood -= 1;
                    (parameters[1] as MinerInventory).totalFood += 1;
                }
            }
        });

        behaviour.SetTransitionBehaviour(() =>
        {
            if ((parameters[1] as MinerInventory).totalFood > 0)
            {
                OnFlag?.Invoke(Flags.OnEndStrike);
            }
        });

        return behaviour;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        return default;
    }
}

//Caravan

public sealed class DepositFoodState<NodeType, CoordType> : State
    where NodeType : class, INode<CoordType>, new()
    where CoordType : IEquatable<CoordType>, ICoordType<int>, new()
{
    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMultitreadableBehaviours(0, () =>
        {
            TileClass newtileclass = (parameters[0] as NodeType).GetTileClass();

            if (newtileclass is MineInventory mineInventory)
            {
                mineInventory.totalFood = (parameters[1] as CaravanInventory).totalFood;
                (parameters[1] as CaravanInventory).totalFood = 0;
            }
        });

        behaviour.SetTransitionBehaviour(() =>
        {
            if ((parameters[1] as CaravanInventory).totalFood <= 0)
            {
                OnFlag?.Invoke(Flags.OnInventoryEmpty);
            }
        });

        return behaviour;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        return default;
    }
}

public sealed class RepositFoodState : State
{
    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMultitreadableBehaviours(0, () => { (parameters[0] as CaravanInventory).totalFood = 15; });

        behaviour.SetTransitionBehaviour(() =>
        {
            if ((parameters[0] as CaravanInventory).totalFood >= 0)
            {
                OnFlag?.Invoke(Flags.OnInventoryFull);
            }
        });

        return behaviour;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        return default;
    }
}

public sealed class WaitToCallFoodState<NodeType, CoordType> : State
    where NodeType : class, INode<CoordType>, new()
    where CoordType : IEquatable<CoordType>, ICoordType<int>, new()
{
    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        Debug.Log("Wait for call");
        return default;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.SetTransitionBehaviour(() =>
        {
            TileClass newtileclass = (parameters[0] as NodeType).GetTileClass();

            if (newtileclass is MineInventory mineInventory)
            {
                if (mineInventory.haveMiner)
                {
                    OnFlag?.Invoke(Flags.OnInventoryFull);
                }
            }
        });

        return behaviour;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        return default;
    }
}