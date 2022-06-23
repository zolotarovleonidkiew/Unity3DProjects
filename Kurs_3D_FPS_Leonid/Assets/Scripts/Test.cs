using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete]
public class Test : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [SerializeField] private float _currentSpeed;
    [SerializeField] private float _maxSpeed = 25;

    private float _mouseSensitvity = 5;

    private float topAngleView = 60;
    public float bottomAngleView = -45;
    private float wantedYRotation;
    private float wantedCameraXRotation;

    void Start()
    {
        
    }

    void Update()
    {
        MouseInputMovement();
    }

    private void FixedUpdate()
    {
        PlayerMovementLogic();
    }

    void PlayerMovementLogic()
    {

    }

    void MouseInputMovement()
    {
        wantedYRotation += Input.GetAxis("Mouse X") * _mouseSensitvity;
        wantedCameraXRotation -= Input.GetAxis("Mouse Y") * _mouseSensitvity;

        wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);

        _camera.transform.rotation = Quaternion.Euler(wantedCameraXRotation, wantedYRotation, 0);
    }
}
