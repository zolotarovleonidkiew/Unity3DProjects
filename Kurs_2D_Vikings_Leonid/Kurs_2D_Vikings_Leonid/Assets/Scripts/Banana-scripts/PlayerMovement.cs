using System;
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


    bool _isDead = false;
    public void Kill()
    {
       // _isDead = true;
       // animator.SetBool("IsDead", true);
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

    private DateTime _finishAttackDatetime = new DateTime();
    private BoxCollider2D _baealogAttackCollider;

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
        if (controller.IsDead)
        {
            animator.SetBool("IsDead", true);
        }

        if (_finishAttackDatetime != new DateTime())
        {
            if (_finishAttackDatetime <= DateTime.Now)
            {
                //закончить атаку Баеалога
                _baealogAttackCollider.enabled = false;
                Debug.Log("Баеалог закончил махать мечом");
                _finishAttackDatetime = new DateTime();
            }
        }

        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        //TO DO: если нажата кнопка Идти (W)и потом нажать Атаку, анимация атаки не отображается

        //сменить игрока
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            selectedUserId =
                selectedUserId + 1 > PlayerCharacters.Length - 1 ?
                selectedUserId = 0 :
                selectedUserId + 1;

            CurrentPlayer = PlayerCharacters[selectedUserId];
        }                // смена персонажа
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            if (animator.GetBool("IsJumping") == false)
            {
                jump = true;
                animator.SetBool("IsJumping", true);
            }
        }         // прыжок
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
        }             // открыть дверь
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
        }       // на лифте - ехать вверх/вниз
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            //Только баеалог может мечом рубануть ког-ото
            if (transform.gameObject.name == "Hero-Baealog")
            {
                animator.SetBool("IsAttack", true);

                //активировать триггер боя
                var attackTrigger = transform.GetChild(1);

                _finishAttackDatetime = DateTime.Now.AddSeconds(3);
                Debug.Log("Баеалог начал махать мечом");

                if (attackTrigger != null)
                {
                    _baealogAttackCollider = attackTrigger.GetComponent<BoxCollider2D>();

                    _baealogAttackCollider.enabled = true;
                }
            }
        }   // атаковать мечом
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            controller.ChangeindexSelectedInventoryItem();
        }             // сменить активную ячейку в инвентаре
        else if (Input.GetKeyDown(KeyCode.R))
        {
            var selectedItem = controller.GetInventoryItemByIndex();

            if (selectedItem != null)
            {
                if (selectedItem.Type == ItemTypes.Food)
                {
                    if (controller.CurrentHealth <= CharacterController2D.MaxHealthPoint)
                    {
                        controller.CurrentHealth += (selectedItem as FoodItem).RestoreHealthPoints;

                        controller.RemoveItemFromInventory();
                    }
                }
                else if (selectedItem.Type == ItemTypes.Bomb)
                {
                    //use
                    GameObject plantedBomb = new GameObject($"{this.name}_plantedBomb");
                    plantedBomb.AddComponent<SpriteRenderer>();
                    plantedBomb.AddComponent<BoxCollider2D>();
                    plantedBomb.AddComponent<Transform>();
                    plantedBomb.AddComponent<Rigidbody2D>();

                    var script = plantedBomb.AddComponent<PlantedBombTimer>();
                    script.Explotion = controller._explotionSprite;

                    var tc = plantedBomb.GetComponent<Transform>();
                    tc.position = transform.position + transform.right;

                    var sr = plantedBomb.GetComponent<SpriteRenderer>();
                    sr.sprite = controller._bombHasBeenPlanted;
                    sr.size = new Vector2(300, 300);
                    sr.sortingOrder = 5;

                    var bc2 = plantedBomb.AddComponent<BoxCollider2D>();
                    bc2.isTrigger = false;
                    bc2.size = new Vector2(2, 2);

                    var bc = plantedBomb.GetComponent<BoxCollider2D>();
                    bc.isTrigger = true;
                    bc.size = new Vector2(10, 10);



                    controller.RemoveItemFromInventory();
                }
            }
        }             // применить активный элемент в инвентаре
    }

    /// <summary>
    /// Прекратить анимацию атаки после первого отображения
    /// </summary>
    public void StopBaealogAttackAnimation()
    {
        if (animator.GetBool("IsAttack"))
        {
            animator.SetBool("IsAttack", false);
        }
    }

    void FixedUpdate()
    {
        //try
        ////if (controller.isGrounded)
        ////{
        ////    animator.SetBool("IsJumping", false);
        ////}

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
    ///
    public void DestoyPlayerObjectAfterDeathAnimFinished()
    {
        Destroy(this.gameObject);

        //Удалить этого викинга из всех списков, чтоб не упоминался в списках
    }
}
