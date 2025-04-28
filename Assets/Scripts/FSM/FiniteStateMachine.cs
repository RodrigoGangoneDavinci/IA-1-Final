using System;
using System.Collections.Generic;
using UnityEngine;

public class FiniteStateMachine : MonoBehaviour
{
    State _currentState;
    Dictionary<Enum, State> _allStates = new();

    public void Update()
    {
        _currentState?.OnUpdate();
    }

    public void AddState(Enum name, State state)
    {
        if (!_allStates.ContainsKey(name))
        {
            _allStates.Add(name, state);
            state.fsm = this;
        }
        else
        {
            _allStates[name] = state;
        }
    }

    public void ChangeState(Enum name) 
    {
        _currentState?.OnExit();
        if (_allStates.ContainsKey(name)) _currentState = _allStates[name];
        _currentState?.OnEnter();
    }
}
