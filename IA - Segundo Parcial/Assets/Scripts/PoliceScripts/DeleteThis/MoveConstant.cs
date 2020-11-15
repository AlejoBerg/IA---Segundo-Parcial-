using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveConstant : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 velocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        rb.velocity = velocity;
    }
}
