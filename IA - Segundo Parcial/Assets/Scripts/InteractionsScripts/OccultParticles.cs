using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OccultParticles : MonoBehaviour
{
    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject[] money;
    [SerializeField] private GameObject scoreDisplay;
    [SerializeField] private GameObject textToDisplay;
    [SerializeField] private AudioSource audio;
    private bool stop = false;

    private void Awake()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            particles.SetActive(false);
            scoreDisplay.GetComponent<TextFader>().Fade();
            textToDisplay.GetComponent<TextFader>().Fade();
            audio.Stop();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Player"))
        {
            StartCoroutine(CollectMoney());
           
        }
    }

    IEnumerator CollectMoney()
    {
        for (int i = 0; i < money.Length; i++)
        {
            if(!stop){scoreDisplay.GetComponent<TextFader>().Fade();}
            money[i].SetActive(false);
            GameManager.AddPoints(500);
            stop = true;
            if (i == 16)
            {
                textToDisplay.GetComponent<TextFader>().Fade();
            }
            yield return new  WaitForSeconds(0.6f);
            audio.Play();
        }
        yield return null;
    }
}