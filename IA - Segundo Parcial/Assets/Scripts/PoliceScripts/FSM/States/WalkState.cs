using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState<T> : FSMState<T> 
{
    private IMove _objectToMoveRef;
    public WalkState(IMove objectToMoveRef)
    {
        _objectToMoveRef = objectToMoveRef;
    }

    public override void Execute() 
    {
        _objectToMoveRef.Move();
    }
}
