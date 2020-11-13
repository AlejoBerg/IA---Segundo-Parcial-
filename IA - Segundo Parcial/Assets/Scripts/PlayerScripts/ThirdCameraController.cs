using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThirdCameraController : MonoBehaviour
{
    private GameObject playerRef = null;
    [SerializeField] private float distanceBetweenPlayer = 3;
    [SerializeField] private float sensitivity;
    [SerializeField] private Camera camera = null;
    private Transform cameraTransform = null;
    private const float minXAngle = 12f;
    private const float maxXAngle = 50f;
    private float currentX = 0f;
    private float currentY = 0f;
    private float minFov = 20f;
    private float maxFov = 50f;
 
    void Start()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");
        cameraTransform = this.transform;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        RelativePositionFromMouse();
        CalculateCameraPosition();
        CameraZoom();
    }

    void RelativePositionFromMouse()
    {
        currentX -= Input.GetAxis("Mouse Y");
        currentY += Input.GetAxis("Mouse X");
        currentX = Mathf.Clamp(currentX, minXAngle, maxXAngle);
    }

    void CalculateCameraPosition()
    {      
        Vector3 direction = new Vector3(0, 0, -distanceBetweenPlayer);
        Quaternion rotation = Quaternion.Euler(currentX,currentY ,0 );
        cameraTransform.position = playerRef.transform.position + rotation * direction;
        cameraTransform.LookAt(playerRef.transform.position + new Vector3(0,1f,0));
    }
    
    void CameraZoom()
    {
        var fov = camera.fieldOfView;
        fov += Input.GetAxis("Mouse ScrollWheel") * sensitivity; 
        fov = Mathf.Clamp(fov, minFov, maxFov);
        camera.fieldOfView = fov;
    }

    public void ChangeDistanceToPlayer(float distance)
    {
        distanceBetweenPlayer = distance;
    }

    public void ChangeCameraPos(float mouseY, float mouseX)
    {
        currentX = mouseY;
        currentY = mouseX;
    }
}