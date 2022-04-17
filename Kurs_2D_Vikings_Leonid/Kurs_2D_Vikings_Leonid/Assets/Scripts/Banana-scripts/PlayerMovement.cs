using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;

    float horizontalMove = 0f;
    // bool jump = false;
    // bool crouch = false;

    //game logic
    Character[] PlayerCharacters; // list of characters

    //выбранный персонаж
    int selectedUserId = 0;
    Character CurrentPlayer;

    //game logic

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

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log($"Key 'Tab' pressed.");
            
            var oldName = CurrentPlayer.Name;

            selectedUserId =
                selectedUserId + 1  > PlayerCharacters.Length - 1 ?
                selectedUserId = 0 :
                selectedUserId + 1;

            CurrentPlayer = PlayerCharacters[selectedUserId];

            Debug.Log($"Player changed from {oldName} to {CurrentPlayer.Name}");
        }
    }

    void FixedUpdate()
    {
        var crouch = false;
        var jump = false;

        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump);
        jump = false;
    }
}
