using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BandideTesting : MonoBehaviour
{
    private void Start()
    {
        GameManager.Instance.bandides.Add(this.gameObject);
    }
}
