using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private Transform _myCamera;
    [SerializeField] private Transform _weaponPlaceHolder;

    [Tooltip("Current mouse sensivity, changes in the weapon properties")]
    private float mouseSensitvity = 5;//0;
    [Tooltip("Top camera angle.")]
    private float topAngleView = 60;
    [Tooltip("Minimum camera angle.")]
    public float bottomAngleView = -45;

    /// <summary>
    /// Определяем, что оружие есть в руках
    /// </summary>
    private bool _weaponWasPicked => _weaponPlaceHolder?.childCount == 3;

    public float wantedYRotation;
    public float currentYRotation;
    public float wantedCameraXRotation;

    private float deltaTime = 0.0f;
    private float zRotation;
    private float timer;
    private float timeSpeed = 2;
    private int int_timer;
    private float wantedZ;
    private float timerToRotateZ;
    private float mouseSensitvity_notAiming =  1;// 300;
    private float mouseSensitvity_aiming = 10; //50;
    private float rotationYVelocity, cameraXVelocity;
    [SerializeField]  private float yRotationSpeed = 0;
    private float xCameraSpeed = 0;
    public float currentCameraXRotation;

    private void Awake()
    {
        //курсор по центру экрана заморозить
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        MouseInputMovement();

        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    private void FixedUpdate()
    {
        /*
         * Reduxing mouse sensitvity if we are aiming.
         */
        if (Input.GetAxis("Fire2") != 0)
        {
            mouseSensitvity = mouseSensitvity_aiming;
        }
        else if (GetComponent<PlayerMovement>().maxSpeed > 5)
        {
            mouseSensitvity = mouseSensitvity_notAiming;
        }
        else
        {
            mouseSensitvity = mouseSensitvity_notAiming;
        }

        //вращение головой
        HeadMovement();
    }

    private void MouseInputMovement()
    {
        wantedYRotation += Input.GetAxis("Mouse X") * mouseSensitvity;

        wantedCameraXRotation -= Input.GetAxis("Mouse Y") * mouseSensitvity;

        wantedCameraXRotation = Mathf.Clamp(wantedCameraXRotation, bottomAngleView, topAngleView);
    }

    private void HeadMovement()
    {
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotationYVelocity, yRotationSpeed);
        currentCameraXRotation = Mathf.SmoothDamp(currentCameraXRotation, wantedCameraXRotation, ref cameraXVelocity, xCameraSpeed);

        transform.rotation = Quaternion.Euler(0, currentYRotation, 0);
        _myCamera.transform.rotation = Quaternion.Euler(currentCameraXRotation, currentYRotation, 0);
    }
}
