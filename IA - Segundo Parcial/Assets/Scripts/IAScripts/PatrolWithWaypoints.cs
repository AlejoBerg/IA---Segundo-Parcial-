using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolWithWaypoints : MonoBehaviour
{
    [SerializeField] private Transform[] targets;
    private int currentTargetIndex;
    private float speed = 10f;     

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, targets[currentTargetIndex].position, speed * Time.deltaTime);
        float dist = Vector3.Distance(transform.position, targets[currentTargetIndex].position);
        if (dist <= 0.1)
        {
            currentTargetIndex++;
            if (currentTargetIndex >= targets.Length)
            {
                currentTargetIndex = 0;
            }
        }
    }
}
