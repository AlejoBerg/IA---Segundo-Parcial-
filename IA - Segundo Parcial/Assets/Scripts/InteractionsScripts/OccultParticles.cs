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
        if (other.gameObject.tag.Equals("Bandit"))
        {
            particles.SetActive(false);
            scoreDisplay.GetComponent<TextFader>().Fade();
            textToDisplay.GetComponent<TextFader>().Fade();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals("Bandit"))
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
            GameManager.AddPoints(1000);
            stop = true;
            if (i == 16)
            {
                textToDisplay.GetComponent<TextFader>().Fade();
                audio.Play();
            }
            yield return new  WaitForSeconds(2f);
        }
        yield return null;
    }
}