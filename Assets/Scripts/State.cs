using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    public Action<int> OnFlag;
    public abstract List<Action> GetOnEnterbehaviour(params object[] parameters);
    public abstract List<Action> GetTickbehaviour(params object[] parameters);
    public abstract List<Action> GetOnExitbehaviour(params object[] parameters);
}

public sealed class ChaseState : State
{
    public override List<Action> GetOnEnterbehaviour(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetOnExitbehaviour(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickbehaviour(params object[] parameters)
    {
        Transform OwnerTransform = parameters[0] as Transform;
        Transform TargetTransform = parameters[1] as Transform;
        float speed = Convert.ToSingle(parameters[2]);
        float explodeDistance = Convert.ToSingle(parameters[3]);
        float lostDistance = Convert.ToSingle(parameters[4]);

        List<Action> behaviour = new List<Action>();

        behaviour.Add(() =>
        {
            OwnerTransform.position += (TargetTransform.position - OwnerTransform.position).normalized * speed * Time.deltaTime;
        });

        behaviour.Add(() =>
        {
            Debug.Log("Whistle!");
        });

        behaviour.Add(() =>
        {
            if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) < explodeDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetReach);
            }
            else if (Vector3.Distance(TargetTransform.position, OwnerTransform.position) > lostDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetLost);
            }
        });

        return behaviour;
    }
}

public sealed class PatrolState : State
{
    private Transform actualTarget;
    public override List<Action> GetOnEnterbehaviour(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetOnExitbehaviour(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickbehaviour(params object[] parameters)
    {
        Transform ownerTransform = parameters[0] as Transform;
        Transform wayPoint1 = parameters[1] as Transform;
        Transform wayPoint2 = parameters[2] as Transform;
        Transform ChaseTarget = parameters[3] as Transform;
        float speed = Convert.ToSingle(parameters[4]);
        float chaseDistance = Convert.ToSingle(parameters[5]);

        List<Action> behaviour = new List<Action>();

        behaviour.Add(() =>
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

        behaviour.Add(() =>
        {
            if (Vector3.Distance(ownerTransform.position, ChaseTarget.position) < chaseDistance)
            {
                OnFlag?.Invoke((int)Flags.OnTargetNear);
            }
        });

        return behaviour;
    }
}

public sealed class ExplodeState : State
{
    public override List<Action> GetOnEnterbehaviour(params object[] parameters)
    {
        List<Action> behaviour = new List<Action>();

        behaviour.Add(() =>
        {
            Debug.Log("Explode!");
        });
        return behaviour;
    }

    public override List<Action> GetOnExitbehaviour(params object[] parameters)
    {
        return new List<Action>();
    }

    public override List<Action> GetTickbehaviour(params object[] parameters)
    {
        List<Action> behaviour = new List<Action>();

        behaviour.Add(() =>
        {
            Debug.Log("F");
        });

        return behaviour;
    }
}