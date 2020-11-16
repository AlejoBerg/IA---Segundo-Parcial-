using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance2 : MonoBehaviour
{   
    [SerializeField] private Transform _target;
    [SerializeField] private float _radius;
    [SerializeField] private LayerMask _mask;
    [SerializeField] private float _avoidWeight;


    public Vector3 GetDir()
    {
        //Obtenemos los obstaculos
        Collider[] obstacles = Physics.OverlapSphere(transform.position, _radius, _mask);
        Transform obsSave = null;
        var count = obstacles.Length;

        //Recorremos los obstaculos y determinos cual es el mas cercano
        for (int i = 0; i < count; i++)
        {
            var currObs = obstacles[i].transform;
            if (obsSave == null)
            {
                obsSave = currObs;
            }
            else if (Vector3.Distance(transform.position, obsSave.position) > Vector3.Distance(transform.position, currObs.position))
            {
                obsSave = currObs;
            }
        }
        Vector3 dirToTarget = (_target.position - transform.position).normalized;

        //Si hay un obstaculo, le agregamos a nuestra direccion una direccion de esquive
        if (obsSave != null)
        {
            Vector3 dirObsToNpc = (transform.position - obsSave.position).normalized * _avoidWeight;
            dirToTarget += dirObsToNpc;
        }
        //retornamos la direccion final
        return dirToTarget.normalized;
    }

    public void ChangeTarget(Transform newTarget)
    {
        _target = newTarget;
    }
}
