using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 10000;							// Amount of force added when the player jumps. 800
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement

    private Vector3 m_Velocity = Vector3.zero;

    private Rigidbody2D m_Rigidbody2D;

    private bool playerIconDirectionToRight = true;
    private bool playerOnTheFloor = true;

    //land
  //  public UnityEvent OnLandEvent;
    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool isGrounded;
    public Transform groundCheck;
    public LayerMask GroundLayer;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        //if (OnLandEvent == null)
        //    OnLandEvent = new UnityEvent();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //bool wasGrounded = isGrounded;
        //isGrounded = false;

        //// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        //// This can be done using layers instead but Sample Assets will not overwrite your project settings.
        //Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        //for (int i = 0; i < colliders.Length; i++)
        //{
        //    if (colliders[i].gameObject != gameObject)
        //    {
        //        isGrounded = true;
        //        if (!wasGrounded)
        //            OnLandEvent.Invoke();
        //    }
        //}

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, k_GroundedRadius, GroundLayer);

        playerOnTheFloor = isGrounded;

        if (!isGrounded)
        {
           // animator.SetBool("IsJumping", false);
        }
    }

    public void Move(Character currentPlayer, float move, bool crouch, bool jump)
    {
        if (playerOnTheFloor && !jump)
        {
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);

            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            if (move > 0 && !playerIconDirectionToRight)
            {
                Flip();
            }
            else if (move < 0 && playerIconDirectionToRight)
            {
                Flip();
            }
        }
        else if (jump)
        {
            if (isGrounded)
            {
                playerOnTheFloor = false;
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            }
            else
            {
                playerOnTheFloor = true;
            }

        }
    }

    /// <summary>
    /// Change direction to dprite on X
    /// </summary>
    private void Flip()
    {
        playerIconDirectionToRight = !playerIconDirectionToRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
