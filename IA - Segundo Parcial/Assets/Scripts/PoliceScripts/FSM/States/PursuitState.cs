using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursuitState<T> : FSMState<T> 
{
    private IAttack _chaser;
    public PursuitState(IAttack chaser) 
    {
        _chaser = chaser;
    }

    public override void Execute()
    {
        //Debug.Log("Ejecutando PursuitState - PursuitTarget");
        _chaser.PursuitTarget();
    }
}
