using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTrain : MonoBehaviour
{
    [SerializeField] private GameObject train;
    [SerializeField] private GameObject particles;
    [SerializeField] private float speedTrain;
    private bool active = false;
    
    private void Update()
    {
        if(active){train.transform.position = new Vector3(train.transform.position.x + speedTrain,train.transform.position.y,train.transform.position.z);}
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Bandit"))
        {
            particles.SetActive(true);
            active = true;
        }
    }
}