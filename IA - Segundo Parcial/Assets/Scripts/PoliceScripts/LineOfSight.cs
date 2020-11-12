using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private float _fieldOfView = 0; //Distancia
    [SerializeField] private float _angleOfVision = 0; //Angulo
    [SerializeField] private LayerMask _obstacleMast;

    private float _distance = 0;

    public bool IsTargetInSight(GameObject target)
    {
        var _diff = target.transform.position - transform.position;
        _distance = _diff.magnitude;

        if (_distance <= _fieldOfView) 
        {
            var angleToTarget = Vector3.Angle(transform.forward, _diff.normalized); // Uso diff y no distance porque el primero es un vector3, el segundo es solamente un float
            if(angleToTarget <= _angleOfVision)
            {
                if(Physics.Raycast(transform.position, _diff.normalized, _distance, _obstacleMast))
                {
                    return false; //Colision obstaculo
                }
                return true;
            }
        }
        return false;
    }

    public bool GetDistanceToTarget(float distance)
    {
        if (_distance < distance) 
        {
            return true;
        }
        else 
        {
            return false;
        }   
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * _fieldOfView);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, _angleOfVision / 2, 0) * transform.forward * _fieldOfView);
        Gizmos.DrawRay(transform.position, Quaternion.Euler(0, -_angleOfVision / 2, 0) * transform.forward * _fieldOfView);
        Gizmos.DrawWireSphere(transform.position, _fieldOfView);
    }
}
