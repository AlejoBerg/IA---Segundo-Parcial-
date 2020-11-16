using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Victory : MonoBehaviour
{
   private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Bandide"))
        {
            SceneManager.LoadScene(3);
        }
    }
}