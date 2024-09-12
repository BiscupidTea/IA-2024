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

public sealed class MoveState<NodeType> : State where NodeType : INode<Vector2Int>
{
    private int currentTargetPoint;
    private Transform[] nodesPath;
    private NodeType target;

    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        throw new NotImplementedException();
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        float speed = Convert.ToSingle(parameters[0]);
        Transform ownerTransform = parameters[1] as Transform;
        nodesPath = parameters[2] as Transform[];
        

        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMainThreadBehaviour(0, () =>
        {
            if (Vector3.Distance(ownerTransform.position, nodesPath[currentTargetPoint].position) < 0.2f)
            {
                currentTargetPoint++;
            }

            ownerTransform.position +=
                (nodesPath[currentTargetPoint].position - ownerTransform.position).normalized * speed * Time.deltaTime;
        });

        behaviour.SetTransitionBehaviour(() =>
        {
            if (currentTargetPoint >= nodesPath.Length)
            {
                if (target.GetNodeType() == NodeTypeCost.TownCenter)
                {
                    OnFlag?.Invoke(Flags.OnDepositGold);
                }
                else
                {
                    OnFlag?.Invoke(Flags.OnMining);
                }
            }
        });

        return behaviour;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        throw new NotImplementedException();
    }
}

public sealed class MiningState : State
{
    private int totalGold;
    private int goldForFood;
    private int totalFood;

    private float timer;

    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMultitreadableBehaviours(0, () =>
        {
            timer = 0;
            goldForFood = 0;
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
                totalGold += 1;
                goldForFood += 1;
                timer = 0;
            }

            if (goldForFood >= goldBeforeFood)
            {
                goldForFood = 0;
                totalFood -= 1;
            }
        });

        behaviour.SetTransitionBehaviour(() =>
        {
            if (totalGold >= 15)
            {
                OnFlag?.Invoke(Flags.OnDepositGold);
            }
        });

        behaviour.SetTransitionBehaviour(() =>
        {
            if (totalGold >= 15)
            {
                OnFlag?.Invoke(Flags.OnRequiresFood);
            }
        });

        return behaviour;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        throw new NotImplementedException();
    }
}

public sealed class DepositState : State
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