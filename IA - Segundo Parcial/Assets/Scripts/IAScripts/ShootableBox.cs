using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableBox : MonoBehaviour, IShoot
{
    public float currentHealth = 3;

    public event Action OnShoot;

    public void GetDamage(float _damage)
    {
        currentHealth -= _damage;
        if (currentHealth <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
