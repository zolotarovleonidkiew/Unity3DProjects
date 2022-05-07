using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  MAKE JUMP !!!
///  
/// https://www.youtube.com/watch?v=64ka8KsUnQc
/// </summary>

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private string Name;

    public CharacterController2D controller;
    public Animator animator;
    public float runSpeed = 40f;

    public void Kill()
    {
        throw new System.NotImplementedException();
    }

    float horizontalMove = 0f;
    bool jump = false;
    // bool crouch = false;

    //game logic
    public Character[] PlayerCharacters; // list of characters

    //выбранный персонаж
    int selectedUserId = 0;
    
    public Character CurrentPlayer;

    //game logic

    #region DoorRegion

    /// <summary>
    /// Персонаж в коллайдере двери, ждет нажатия Е чтобы ключом открыть дверь
    /// </summary>
    public bool TryToOpenDoorFlag;
    public ItemTypes NeededItemToOpenDoorType;
    public DoorController GameObjectDoorReference;

    #endregion

    #region elevator
    public bool playerOnLiftPlatforfmFlag;
    public LiftMovement GameObjectLiftReference;
    #endregion

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
            selectedUserId =
                selectedUserId + 1 > PlayerCharacters.Length - 1 ?
                selectedUserId = 0 :
                selectedUserId + 1;

            CurrentPlayer = PlayerCharacters[selectedUserId];
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
        else if (Input.GetKeyDown(KeyCode.E))
        {
            //применить текущий элемент в инвентаре
            if (TryToOpenDoorFlag)
            {
                var selectedItem = controller.GetInventoryItemByIndex();

                if (selectedItem != null)
                {
                    if (selectedItem.Type == NeededItemToOpenDoorType)
                    {
                        var referenceDoorController = GameObjectDoorReference.GetComponent<DoorController>();

                        if (referenceDoorController == null)
                        {
                            //Debug

                            //"звук Доступ запрещен"
                        }
                        else
                        {
                            //юзер, имея правильный ключ, нажал кнопку открытия дверей
                            referenceDoorController.DoorReadyToOpenFlag = true;

                            //удалить использованный ключ
                            controller.RemoveItemFromInventory();
                        }
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))//вызвать лифт вверх/вниз 
        {
            if (playerOnLiftPlatforfmFlag)
            {
                if (GameObjectLiftReference != null)
                {
                    GameObjectLiftReference._playerLaunchedLiftMovement = true;
                }
                else
                {
                    Debug.LogError("GameObjectLiftReference is null");
                }
            }
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
