using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillState<T> : FSMState<T>
{
    private IAttack _attacker;

    public KillState(IAttack attacker)
    {
        _attacker = attacker;
    }

    public override void Execute() 
    {
        _attacker.KillTarget();
    }
}
