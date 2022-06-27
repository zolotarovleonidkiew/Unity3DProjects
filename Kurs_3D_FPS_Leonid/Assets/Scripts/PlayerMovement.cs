using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[SerializeField] public float currentSpeed;
    [SerializeField] public float maxSpeed;

    [SerializeField] private Transform cameraMain;

    Rigidbody rb;
    private LayerMask ignoreLayer;//to ignore player layer
	private Vector3 slowdownV;
	private Vector2 horizontalMovement;
	bool grounded;
	public float deaccelerationSpeed = 15.0f;
    public float accelerationSpeed = 50000.0f * 100000;//00

	private void Awake()
    {
        rb = GetComponent<Rigidbody>();
       // cameraMain = transform.Find("Main Camera").transform;
       // bulletSpawn = cameraMain.Find("BulletSpawn").transform;
        ignoreLayer = 1 << LayerMask.NameToLayer("Player");
		var g = 5;
    }

    private void Start()
    {
		grounded = true;
	}


    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
		PlayerMovementLogic();
	}

    //void FixedUpdate()
    //{
    //    //RaycastForMeleeAttacks();

    //    PlayerMovementLogic();
    //}

    public void PlayerMovementLogic(Vector3? joystickPosition = null)
	{
        float k = 8000;//800

        //rb.AddRelativeForce(
        //    Input.GetAxis("Horizontal") * accelerationSpeed / 2 * Time.deltaTime * k,
        //    0,
        //    Input.GetAxis("Vertical") * accelerationSpeed / 2 * Time.deltaTime * k);

        currentSpeed = rb.velocity.magnitude;

        horizontalMovement = new Vector2(rb.velocity.x, rb.velocity.z);

        if (horizontalMovement.magnitude > maxSpeed)
        {
            horizontalMovement = horizontalMovement.normalized;
            horizontalMovement *= maxSpeed;
        }

        rb.velocity = new Vector3(
            horizontalMovement.x,
            rb.velocity.y,
            horizontalMovement.y
        );

        if (grounded)
        {
            rb.velocity = Vector3.SmoothDamp(rb.velocity,
                new Vector3(0, rb.velocity.y, 0),
                ref slowdownV,
                deaccelerationSpeed);
        }

        var axeHorizontal = Input.GetAxis("Horizontal");
        var axeVertical = Input.GetAxis("Vertical");

        if (axeHorizontal > 0 || axeVertical > 0)
        {
            int g = 5;
        }

        if (joystickPosition != null)
        {
            axeHorizontal = joystickPosition.Value.x;
            axeVertical = joystickPosition.Value.y;
        }

        if (grounded)
        {
            rb.AddRelativeForce(axeHorizontal//Input.GetAxis("Horizontal")
                * accelerationSpeed
                * Time.deltaTime 
                * k,
                0,
                axeVertical //Input.GetAxis("Vertical")
                * accelerationSpeed * Time.deltaTime * k);
        }
        else
        {
            rb.AddRelativeForce(
                axeHorizontal * accelerationSpeed / 2 * Time.deltaTime * k, //Input.GetAxis("Horizontal")
                0,
                axeVertical * accelerationSpeed / 2 * Time.deltaTime * k); //Input.GetAxis("Vertical")

        }

        /*
         * Slippery issues fixed here
         */
        if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
        {
            deaccelerationSpeed = 0.5f;
        }
        else
        {
            deaccelerationSpeed = 0.1f;
        }
    }
}
