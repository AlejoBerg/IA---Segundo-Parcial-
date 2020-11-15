using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : ISteering
{
    //Transform _targetPosition;
    Transform _npcPosition;
    //Rigidbody _targetRb;
    float _predictionTime;

    public Pursuit(Transform npcPos, float predictionTime) 
    {
        _predictionTime = predictionTime;
        //_targetRb = targetRb;
        _npcPosition = npcPos;
        //_targetPosition = targetPos;
    }

    public Vector3 GetDirection(GameObject target)
    {
        //Debug.Log("lista de bandidos.count =  " + GameManager.Instance.bandides.Count);
        //Debug.Log("lista de policias.count =  " + GameManager.Instance.cops.Count);
        var targetRB = target.GetComponent<Rigidbody>();
        var targetTransform = target.GetComponent<Transform>();
        var targetSpeed = targetRB.velocity.magnitude;
        var newPosPrediction = targetTransform.position + targetTransform.forward * targetSpeed * _predictionTime;
        Vector3 newNPCDirection = (newPosPrediction - _npcPosition.position).normalized;
        return newNPCDirection;
    }
}
