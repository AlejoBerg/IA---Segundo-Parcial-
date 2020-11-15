using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccultInitialParticle : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    [SerializeField] private AudioSource audio;
    
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Bandit"))
        {
            audio.Play();
            StartCoroutine(Occult());
        }
    }

    IEnumerator Occult()
    {
        yield return new WaitForSeconds(1);
        particles.SetActive(false);
    }
}