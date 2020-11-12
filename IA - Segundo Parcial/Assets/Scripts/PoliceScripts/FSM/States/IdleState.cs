using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState<T> : FSMState<T>
{
    private IIdle _idleEntity;

    public IdleState(IIdle idleEntity)
    {
        _idleEntity = idleEntity;
    }

    public override void Execute() 
    {
        //Debug.Log("Ejecutando IdleState - DoIdle");
        _idleEntity.DoIdle();
    }
}
