using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadState<T> : FSMState<T>
{
    private IAttack _recharger;

    public ReloadState(IAttack recharger)
    {
        _recharger = recharger;
    }

    public override void Execute()
    {
        //Debug.Log("Ejecutando ReloadState - ReloadAmmo");
        _recharger.ReloadAmmo();
    }
}
