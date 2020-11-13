using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccultInitialParticle : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            particles.SetActive(false);
        }
    }
}