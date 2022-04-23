using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  MAKE JUMP !!!
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private string Name;

    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;

    float horizontalMove = 0f;
    bool jump = false;
    // bool crouch = false;

    //game logic
    public Character[] PlayerCharacters; // list of characters

    //выбранный персонаж
    int selectedUserId = 0;
    
    public Character CurrentPlayer;

    //game logic


    // private bool isGrounded;
    //public Transform groundCheck;
    //public LayerMask GroundLayer;
    //const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded

    public PlayerMovement()
    {
        PlayerCharacters = new Character[] {
            new Viking2_Olaf(),
            new Viking1_Ulrick(),
            new Viking3_Baelog()
        };

        CurrentPlayer = PlayerCharacters[selectedUserId];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        //сменить игрока
        if (Input.GetKeyDown(KeyCode.Tab))
        {
           // var t = this.GetComponent("Hero-Olaf")
           // var oldName = CurrentPlayer.Name;

            selectedUserId =
                selectedUserId + 1  > PlayerCharacters.Length - 1 ?
                selectedUserId = 0 :
                selectedUserId + 1;

            CurrentPlayer = PlayerCharacters[selectedUserId];

          //  Debug.Log($"Player changed from {oldName} to {CurrentPlayer.Name}");

            //logic +
            //if (Name != CurrentPlayer.Name)
            //{
            //    var t = this.GetComponent<CharacterController2D>();
            //    t.enabled = false;
            //}
            //else
            //{
            //    var t = this.GetComponent<CharacterController2D>();
            //    t.enabled = true;
            //}
            //logic -
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
           // Debug.LogWarning($"Current user is {CurrentPlayer.Name}");

            //Only Ulrick can jump
            //if (CurrentPlayer is Viking1_Ulrick)
            //{
                jump = true;
                animator.SetBool("IsJumping", true);

            //    Debug.Log($"Player {CurrentPlayer.Name} => Jump");
            //}
            //else
            //{
            //    Debug.Log($"Sorry, but {CurrentPlayer.Name} can't jump");
            //}


        }
    }

    void FixedUpdate()
    {
        var crouch = false;

        // Move our character (go, jump)
        controller.Move(CurrentPlayer, horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;

        //jump = Physics2D.OverlapCircle(groundCheck.position, k_GroundedRadius, GroundLayer);

        //if (!jump)
        //{
        //     animator.SetBool("IsJumping", false);
        //}
    }

    /// <summary>
    /// How get it ??
    /// </summary>
    ////public void LandViking()
    ////{
    ////    animator.SetBool("IsJumping", false);
    ////}
}
