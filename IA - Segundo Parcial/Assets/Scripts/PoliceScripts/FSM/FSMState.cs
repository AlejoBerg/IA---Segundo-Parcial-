using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMState<T> : INode
{
    public virtual void Awake() { }
    public virtual void Execute() { }
    public virtual void Sleep() { }

    public Dictionary<T, FSMState<T>> myTransitions = new Dictionary<T, FSMState<T>>();

    public void AddTransitionToState(T input, FSMState<T> value) 
    {
        if (!myTransitions.ContainsKey(input))
        {
            myTransitions.Add(input, value);
        }
    }

    public void RemoveTransitionToState(T input, FSMState<T> value)
    {
        if (myTransitions.ContainsKey(input))
        {
            myTransitions.Remove(input);
        }
    }

    public FSMState<T> GetTransition(T input) //En base a un input te retorna la transicion posible de ese input
    {
        if (myTransitions.ContainsKey(input))
        {
            return myTransitions[input];
        }
        return null;
    }
}
