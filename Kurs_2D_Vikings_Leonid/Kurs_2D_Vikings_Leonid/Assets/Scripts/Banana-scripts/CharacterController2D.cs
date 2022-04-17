using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement

    private Vector3 m_Velocity = Vector3.zero;

    private Rigidbody2D m_Rigidbody2D;

    private bool playerIconDirectionToRight = true;



    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        //if (OnLandEvent == null)
        //    OnLandEvent = new UnityEvent();

        //if (OnCrouchEvent == null)
        //    OnCrouchEvent = new BoolEvent();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Move(float move, bool crouch, bool jump)
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
