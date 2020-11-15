using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineOfSight : MonoBehaviour
{
    [SerializeField] private float _fieldOfView = 0; //Distancia
    [SerializeField] private float _angleOfVision = 0; //Angulo
    [SerializeField] private LayerMask _obstacleMast;
    private GameObject _gameObjectRef;

    private float _distance = 0;
    private Vector3 _difference = Vector3.zero;

    public bool IsTargetInSight(List<GameObject> targets)
    {
        for (int i = 0; i < targets.Count; i++)
        {
            var _diff = targets[i].transform.position - transform.position;
            var dist = _diff.magnitude;

            if (dist <= _fieldOfView)
            {
                var angleToTarget = Vector3.Angle(transform.forward, _difference.normalized); // Uso diff y no distance porque el primero es un vector3, el segundo es solamente un float
                if (angleToTarget <= _angleOfVision)
                {
                    _distance = dist;
                    _difference = _diff;
                    _gameObjectRef = targets[i];
                }
            }
            _distance = dist;
            _difference = _diff;
        }

        if (_distance <= _fieldOfView) 
        {
            var angleToTarget = Vector3.Angle(transform.forward, _difference.normalized); // Uso diff y no distance porque el primero es un vector3, el segundo es solamente un float
            if(angleToTarget <= _angleOfVision)
            {
                if(Physics.Raycast(transform.position, _difference.normalized, _distance, _obstacleMast))
                {
                    return false; //Colision obstaculo
                }
                //_hit = hit;
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

    public GameObject GetTargetReference()
    {
        //var gameObjectRef = _hit.collider.GetComponent<GameObject>();
        if(_gameObjectRef != null)
        {
            return _gameObjectRef;
        }
        else
        {
            return null;
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
