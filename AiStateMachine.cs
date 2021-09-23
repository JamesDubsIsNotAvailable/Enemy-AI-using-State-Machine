using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Object = System.Object;

public class AiStateMachine
{
    private AiState currentState;
    public AiState[] states;
    public AiAgent agent;

    private Dictionary<Type, List<Transition>> _transitions = new Dictionary<Type, List<Transition>>();
    private List<Transition> _currentTransitions = new List<Transition>();
    private List<Transition> _anyTransitions = new List<Transition>();

    private static List<Transition> EmptyTransitions = new List<Transition>(0);

    public void Update()
    {
        var transition = GetTransition();
        if (transition != null)
            SetState(transition.To);

        currentState?.Update();
    }

    public void SetState(AiState state)
    {
        if (state == currentState)
            return;

        currentState?.Exit();
        currentState = state;

        _transitions.TryGetValue(currentState.GetType(), out _currentTransitions);
        if (_currentTransitions == null)
            _currentTransitions = EmptyTransitions;

        currentState.Enter();
    }

    public void AddTransition(AiState from, AiState to, Func<bool> predicate)
    {
        if (_transitions.TryGetValue(from.GetType(), out var transitions) == false)
        {
            transitions = new List<Transition>();
            _transitions[from.GetType()] = transitions;
        }

        transitions.Add(new Transition(to, predicate));
    }

    public void AddAnyTransition(AiState state, Func<bool> predicate)
    {
        _anyTransitions.Add(new Transition(state, predicate));
    }

    private class Transition
    {
        public Func<bool> Condition { get; }
        public AiState To { get; }

        public Transition(AiState to, Func<bool> condition)
        {
            To = to;
            Condition = condition;
        }
    }

    private Transition GetTransition()
    {
        foreach (var transition in _anyTransitions)
            if (transition.Condition())
                return transition;

        foreach (var transition in _currentTransitions)
            if (transition.Condition())
                return transition;

        return null;
    }
}
