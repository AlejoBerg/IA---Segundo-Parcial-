using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditBandidePositions : MonoBehaviour
{
    [SerializeField] private GameObject[] bandits; 
    [SerializeField] private GameObject[] newTargets; 

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bandit")
        {
            for (int i = 0; i < bandits.Length; i++)
            {
                var obstacleAvoidance = bandits[i].GetComponent<ObstacleAvoidance2>();
                var banditController = bandits[i].GetComponent<BanditController>();

                obstacleAvoidance.ChangeTarget(newTargets[i].transform);
                banditController.ChangeTarget(newTargets[i]);
            }
        }
        this.gameObject.SetActive(false);
    }
}
