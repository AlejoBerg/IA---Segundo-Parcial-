using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private float _rangeOfVision = 0; //Distancia
    [SerializeField] private float _fieldOfView = 0; //Angulo
    [SerializeField] private LayerMask _obstacleMast;


    public bool IsTargetInSight(GameObject target)
    {
        var diff = target.transform.position - transform.position;
        var distance = diff.magnitude;

        if (distance <= _rangeOfVision) 
        {
            var angleToTarget = Vector3.Angle(transform.forward, diff.normalized); // Uso diff y no distance porque el primero es un vector3, el segundo es solamente un float
            if(angleToTarget <= _fieldOfView)
            {
                if(Physics.Raycast(transform.position, diff.normalized, distance, _obstacleMast))
                {
                    return false; //Colision obstaculo
                }
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * _fieldOfView);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, _fieldOfView / 2, 0) * transform.forward * _fieldOfView);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -_fieldOfView / 2, 0) * transform.forward * _fieldOfView);
        Gizmos.DrawWireSphere(transform.position, _fieldOfView);
    }
}
