using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMController<T>
{
    private FSMState<T> _curretState;

    public void SetInitialState(FSMState<T> initialState)
    {
        _curretState = initialState;
        _curretState.Awake();
    }

    public void OnUpdate()
    {
        _curretState.Execute();
    }

    public void MakeTransition(T input)
    {
        FSMState<T> newState = _curretState.GetTransition(input);
        if (newState == null) return; //Si no hay transicion entonces freno la ejecucion
        _curretState.Sleep();
        _curretState = newState;
        _curretState.Awake();
    }
}
