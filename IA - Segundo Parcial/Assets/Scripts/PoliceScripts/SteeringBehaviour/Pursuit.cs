using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pursuit : ISteering
{
    Transform _targetPosition;
    Transform _npcPosition;
    Rigidbody _targetRb;
    float _predictionTime;

    public Pursuit(Transform targetPos, Transform npcPos, Rigidbody targetRb, float predictionTime) 
    {
        _predictionTime = predictionTime;
        _targetRb = targetRb;
        _npcPosition = npcPos;
        _targetPosition = targetPos;
    }

    public Vector3 GetDirection()
    {
        var targetSpeed = _targetRb.velocity.magnitude;
        var newPosPrediction = _targetPosition.position + _targetPosition.forward * targetSpeed * _predictionTime;
        Vector3 newNPCDirection = (newPosPrediction - _npcPosition.position).normalized;
        return newNPCDirection;
    }
}
