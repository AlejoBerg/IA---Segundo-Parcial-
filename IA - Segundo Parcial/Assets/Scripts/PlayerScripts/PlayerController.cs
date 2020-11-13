using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]private float jumpForce = 4f;
    [SerializeField]private float playerSpeed = 4f;
    private Camera mainCamera;
    private bool jumpActive = false;
    private float horizontalMove;
    private float verticalMove;
    private Vector3 playerInput;
    private Vector3 camForward;
    private Vector3 camRight;
    private bool isGrounded = true;
    private Animator playerAnimator;
    private float playerSpeedForAnimation;
    private Rigidbody rb;
    private float coordsY;
    
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerAnimator = GetComponent<Animator>();
        mainCamera = Camera.main;
    }
    
    void Update()
    {
        Movement();
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector3(playerInput.x, coordsY, playerInput.z); ;
    }

    void Movement()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        coordsY = rb.velocity.y;
        playerInput = new Vector3(horizontalMove, 0,verticalMove).normalized * playerSpeed;
        
        if (horizontalMove != 0 || verticalMove != 0)
        {
            //playerSpeedForAnimation = 0.2f; 
            //playerAnimator.SetFloat("Speed",Math.Abs(playerSpeedForAnimation));
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerSpeed = 5f;
               // playerSpeedForAnimation = 1.2f;
                //playerAnimator.SetFloat("Speed",Math.Abs(playerSpeedForAnimation ));
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                playerSpeed = 4f;
               // playerSpeedForAnimation = 0.2f; 
            }
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                playerSpeed = 4f;
                //playerSpeedForAnimation = 0.2f; 
            }
            //playerSpeedForAnimation = 0;
            //playerAnimator.SetFloat("Speed",Math.Abs(playerSpeedForAnimation));
        }
        
        CamDirection();

        playerInput = playerInput.x * camRight + playerInput.z * camForward;
        transform.LookAt(transform.position + playerInput);
       
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce((Vector3.up)* jumpForce,ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void CamDirection()
    {
        camForward = mainCamera.transform.forward;
        camRight = mainCamera.transform.right;

        camForward.y = 0;
        camRight.y = 0;

        camForward = camForward.normalized;
        camRight = camRight.normalized;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag.Equals("Floor"))
        {
            //playerAnimator.SetBool("IsGrounded", true);
            isGrounded = true;
        }
    }
}