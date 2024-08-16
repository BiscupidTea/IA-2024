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

public sealed class ChaseState : State
{
    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform TargetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);

        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMainThreadBehaviour(0, () =>
        {
            OwnerTransform.position += (TargetTransform.position - OwnerTransform.position).normalized * speed * Time.deltaTime;
        });

        behaviour.AddMultitreadableBehaviours(0, () =>
        {
            Debug.Log("Whistle!");
        });

        behaviour.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) < explodeDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetReach);
            }
            else if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) > lostDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetLost);
            }
        });

        return behaviour;
    }
}

public sealed class PatrolState : State
{
    private Transform actualTarget;
    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        Transform ownerTransform = parameters[0] as Transform;
        Transform wayPoint1 = parameters[1] as Transform;
        Transform wayPoint2 = parameters[2] as Transform;
        Transform ChaseTarget = parameters[3] as Transform;
        float speed = Convert.ToSingle(parameters[4]);
        float chaseDistance = Convert.ToSingle(parameters[5]);

        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMainThreadBehaviour(0, () =>
        {
            if (actualTarget == null)
            {
                actualTarget = wayPoint1;
            }

            if (Vector3.Distance(ownerTransform.position, actualTarget.position) < 0.2f)
            {
                if (actualTarget == wayPoint1)
                    actualTarget = wayPoint2;
                else
                    actualTarget = wayPoint1;
            }
            ownerTransform.position += (actualTarget.position - ownerTransform.position).normalized * speed * Time.deltaTime;
        });

        behaviour.SetTransitionBehaviour(() =>
        {
            if (Vector3.Distance(ownerTransform.position, ChaseTarget.position) < chaseDistance)
            {
                OnFlag?.Invoke(Flags.OnTargetNear);
            }
        });

        return behaviour;
    }
}

public sealed class ExplodeState : State
{
    public override BehavioursActions GetOnEnterbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMultitreadableBehaviours(0,() =>
        {
            Debug.Log("Explode!");
        });
        return behaviour;
    }

    public override BehavioursActions GetOnExitbehaviour(params object[] parameters)
    {
        return default;
    }

    public override BehavioursActions GetTickbehaviour(params object[] parameters)
    {
        BehavioursActions behaviour = new BehavioursActions();

        behaviour.AddMultitreadableBehaviours(0,() =>
        {
            Debug.Log("F");
        });

        return behaviour;
    }
}