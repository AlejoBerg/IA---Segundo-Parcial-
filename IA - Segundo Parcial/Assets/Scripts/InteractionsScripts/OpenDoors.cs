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
    Quaternion currentRotation1;
    Quaternion wantedRotation1;
    Quaternion currentRotation2;
    Quaternion wantedRotation2;
    
    private void Awake()
    {
        audio = GetComponent<AudioSource>();
        currentRotation1 = door1.transform.rotation;
        wantedRotation1 = Quaternion.Euler(0,-90,0);
        currentRotation2 = door2.transform.rotation;
        wantedRotation2 = Quaternion.Euler(0,90,-180);
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
        if (other.gameObject.tag.Equals("Bandit"))
        {
            audio.Play();
            canOpenDoors = true;
        }
    }
    
    IEnumerator OpenTheDoors()
    {
        yield return new WaitForSeconds(1);
        door1.transform.rotation = Quaternion.Lerp(currentRotation1,wantedRotation1, 5);//Quaternion.Euler(new Vector3(0,-90,0));
        door2.transform.rotation = Quaternion.Lerp(currentRotation2,wantedRotation2, 5);//Quaternion.Euler(new Vector3(0,90,-180));
        particles.SetActive(true);
        canOpenDoors = false;
        this.gameObject.SetActive(false);
        yield return null;
    }
}