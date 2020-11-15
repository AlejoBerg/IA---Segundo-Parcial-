using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleAvoidance : MonoBehaviour
{
    [SerializeField]
    private float targetVelocity = 1.5f;
    private int numberOfRays = 17;
    private float angle = 90;
    public float rayRange = 2;
    public Transform player;
    [SerializeField] private LayerMask _obstacleLayer;
    

    public bool IsObstacleNear(float _minDistance = 2)
    {
        if (Physics.Raycast(transform.position, transform.forward, _minDistance, _obstacleLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Vector3 RunObstacleAvoidance()
    {
        var deltaPosition = Vector3.zero;
        for (int i = 0; i < numberOfRays; i++)
        {
            var rotation = this.transform.rotation;
            var rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle * 2 - angle, this.transform.up);
            var direction = rotation * rotationMod * Vector3.forward;

            var ray = new Ray(this.transform.position, direction);
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, rayRange))
            {
                deltaPosition -= (1.0f / numberOfRays) * direction;
            }
            else
            {
                deltaPosition += (1.0f / numberOfRays) * direction;
            }

            var newPos = this.transform.position + deltaPosition * Time.deltaTime;
            return newPos;
        }

        return Vector3.zero;
    }
    private void OnDrawGizmos()
    {
        for (int i = 0; i < numberOfRays; i++) 
        { 
        var rotation = this.transform.rotation;
        var rotationMod = Quaternion.AngleAxis((i / ((float)numberOfRays - 1)) * angle * 2 - angle, this.transform.up);
        var direction = rotation * rotationMod * Vector3.forward;
        Gizmos.DrawRay(this.transform.position, direction);
        }
    }
}
