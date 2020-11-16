using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IShoot 
{
    void GetDamage(float _damage);

    event Action OnShoot;
}
