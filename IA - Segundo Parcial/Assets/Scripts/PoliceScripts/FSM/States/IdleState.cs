using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState<T> : FSMState<T>
{
    public override void Awake() 
    {
        Debug.Log("Awake IdleState");
    }
    public override void Execute() 
    {
        Debug.Log("Execute IdleState");
    }
}
