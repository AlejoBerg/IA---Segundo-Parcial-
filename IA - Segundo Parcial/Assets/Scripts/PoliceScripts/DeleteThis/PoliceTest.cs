using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceTest : MonoBehaviour
{   
    private void Start()
    {
        GameManager.Instance.cops.Add(this.gameObject);
    }
}
