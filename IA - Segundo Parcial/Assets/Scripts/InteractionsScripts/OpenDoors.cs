using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoors : MonoBehaviour
{
    [SerializeField] private GameObject door1;
    [SerializeField] private GameObject door2;
    [SerializeField] private GameObject particles;
    private bool canOpenDoors = false;
    [SerializeField] private AudioSource audio;
    
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (canOpenDoors)
        {
            StartCoroutine(OpenTheDoors());
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            audio.Play();
            canOpenDoors = true;
        }
    }
    
    IEnumerator OpenTheDoors()
    {
        yield return new WaitForSeconds(1);
        door1.transform.rotation = Quaternion.Euler(new Vector3(0,-90,0));
        door2.transform.rotation = Quaternion.Euler(new Vector3(0,90,-180));
        particles.SetActive(true);
        canOpenDoors = false;
        this.gameObject.SetActive(false);
        yield return null;
    }
}