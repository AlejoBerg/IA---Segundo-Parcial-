using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeIn : MonoBehaviour
{
    [SerializeField] private GameObject itemToFade;
    
    private void Awake()
    {
        StartCoroutine(DoFade());
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator DoFade()
    {
        yield return new WaitForSeconds(5);
        itemToFade.GetComponent<TextFader>().Fade();
        yield return null;
    }
}