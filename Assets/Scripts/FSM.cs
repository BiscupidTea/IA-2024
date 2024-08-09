using System;
using System.Collections.Generic;
using UnityEditor;

public class FSM
{
    private const int UNNASSSIGNED_TRANSITION = -1;

    public int currentState = 0;
    private Dictionary<int, State> behaviours;
    private Dictionary<int, Func<object[]>> behaviourTickParameters;
    private Dictionary<int, Func<object[]>> behaviourOnEnterParameters;
    private Dictionary<int, Func<object[]>> behaviourOnExitParameters;
    private int[,] transitions;

    public FSM(int states, int flags)
    {
        behaviours = new Dictionary<int, State>();
        transitions = new int[states, flags];
        for (int i = 0; i < states; i++)
        {
            for (int j = 0; j < flags; j++)
            {
                transitions[i, j] = UNNASSSIGNED_TRANSITION;
            }
        }

        behaviourTickParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnEnterParameters = new Dictionary<int, Func<object[]>>();
        behaviourOnExitParameters = new Dictionary<int, Func<object[]>>();
    }

    public void AddBehaviour<T>(int stateIndex, Func<object[]> onTickParameters = null, Func<object[]> onEnterParameters = null, Func<object[]> onExitParameters = null) where T : State, new()
    {
        if (!behaviours.ContainsKey(stateIndex))
        {
            State newbehaviour = new T();
            newbehaviour.OnFlag += Transition;
            behaviours.Add(stateIndex, newbehaviour);
            behaviourTickParameters.Add(stateIndex, onTickParameters);
            behaviourOnEnterParameters.Add(stateIndex, onEnterParameters);
            behaviourOnExitParameters.Add(stateIndex, onExitParameters);
        }
    }

    public void ForcedState(int state)
    {
        currentState = state;
    }

    public void SetTransition(int originState, int flag, int destinationState)
    {
        transitions[originState, flag] = destinationState;
    }

    public void Transition(int flag)
    {
        if (transitions[currentState, flag] != UNNASSSIGNED_TRANSITION)
        {
            foreach (Action behaviour in behaviours[currentState].GetOnExitbehaviour(behaviourTickParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }

            currentState = transitions[currentState, flag];

            foreach (Action behaviour in behaviours[currentState].GetOnEnterbehaviour(behaviourTickParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }
        }
    }

    public void Tick(params object[] parameters)
    {
        if (behaviours.ContainsKey(currentState))
        {
            foreach (Action behaviour in behaviours[currentState].GetTickbehaviour(behaviourTickParameters[currentState]?.Invoke()))
            {
                behaviour?.Invoke();
            }
        }
    }
}
