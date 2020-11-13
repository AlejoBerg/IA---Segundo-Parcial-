using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveAlarmAndIndicator : MonoBehaviour
{
    [SerializeField] private GameObject indicator;
    private bool playedSound = false;
    private AudioSource audio;


    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player") && !playedSound)
        {
           indicator.SetActive(true);
           playedSound = true;
           audio.Play();
        }
    }
}
