using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack 
{
    void PursuitTarget();

    void KillTarget();

    void ReloadAmmo();
}
