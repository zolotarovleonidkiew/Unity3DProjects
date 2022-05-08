using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
    #region Planting Bombs
    [SerializeField] public Sprite _bombHasBeenPlanted;
    [SerializeField] public Sprite _explotionSprite;
    #endregion

    #region Health
    private int _currentHealth;
    private bool _isDead;

    [SerializeField] private Sprite _health_3_points;
    [SerializeField] private Sprite _health_2_points;
    [SerializeField] private Sprite _health_1_points;
    [SerializeField] private Sprite _health_0_points;

    //МАКС здоровья
    public const int MaxHealthPoint = 3;

    /// <summary>
    /// Отслеживание здоровья
    /// </summary>
    public int CurrentHealth
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;

            UpdateUIHealth(_currentHealth);

            if (_currentHealth > MaxHealthPoint)
            {
                _currentHealth = MaxHealthPoint;      
            }
            else if (_currentHealth <= 0)
            {
                IsDead = true;
            }
        }
    }

    /// <summary>
    /// Отслеживание состояния жизни
    /// </summary>
    public bool IsDead
    {
        get { return _isDead; }
        set
        {
            _isDead = value;
        }
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth -= damage;
    }

    private void UpdateUIHealth(int health)
    {
        var taggedObjects = GameObject.FindGameObjectsWithTag("UpperPanelTag");

        var nameToSearh = "";

        if (this.name == "Hero-Baealog")
            nameToSearh = "BaleogStatusBar";
        else if (this.name == "Hero-Olaf")
            nameToSearh = "OlafStatusBar";
        else
            nameToSearh = "UlrichStatusBar";

        GameObject heroObject = taggedObjects.Where(go => go.name == nameToSearh).First();

        GameObject healthObject = heroObject.transform.GetChild(4).gameObject;

        var sr = healthObject.GetComponent<SpriteRenderer>();

        if (health == 3)
            sr.sprite = _health_3_points;
        else if (health == 2)
            sr.sprite = _health_2_points;
        else if (health == 1)
            sr.sprite = _health_1_points;
        else
            sr.sprite = _health_0_points;
    }

    #endregion

    #region Inventory
    [SerializeField] private Sprite _default_inv_sprite;

    [SerializeField] private Sprite _inventoryCellSelected;
    [SerializeField] private Sprite _inventoryCellSelectionDismissed;

    const int InventoryItemsMax = 4;
    public Item[] Inventory;

    private int indexSelectedInventoryItem = 0;

    #endregion

    #region For Exit Level
    [SerializeField] public string VikingName;
    #endregion

    [SerializeField] private float m_JumpForce = 10000 * 500;						// Amount of force added when the player jumps. 800
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
        Inventory = new Item[4] { null, null, null, null };

        CurrentHealth = 3;

        UpdateUISelectedItem();
    }

    private void Update()
    {
        if (_isDead)
        {
            Destroy(this.gameObject);
        }
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


    #region Inventory operations

    /// <summary>
    /// Получить итем по indexSelectedInventoryItem
    /// </summary>
    public Item GetInventoryItemByIndex()
    {
        return Inventory[indexSelectedInventoryItem];
    }

    /// <summary>
    /// Управление инвентарем по инексу
    /// </summary>
    public void ChangeindexSelectedInventoryItem()
    {
        indexSelectedInventoryItem ++;

        if (indexSelectedInventoryItem == InventoryItemsMax)
        {
            indexSelectedInventoryItem = 0;
        }

        UpdateUISelectedItem();
    }

    public void UpdateUISelectedItem()
    {
        var taggedObjects = GameObject.FindGameObjectsWithTag("UpperPanelTag");

        var nameToSearh = "";

        if (this.name == "Hero-Baealog")
            nameToSearh = "BaleogStatusBar";
        else if (this.name == "Hero-Olaf")
            nameToSearh = "OlafStatusBar";
        else
            nameToSearh = "UlrichStatusBar";

        GameObject hero = taggedObjects.Where(go => go.name == nameToSearh).First();

        var listItems = new List<Transform>();

        listItems.Add(hero.transform.GetChild(0));
        listItems.Add(hero.transform.GetChild(1));
        listItems.Add(hero.transform.GetChild(2));
        listItems.Add(hero.transform.GetChild(3));

        for (int i = 0; i < listItems.Count(); i++)
        {
            if (i == indexSelectedInventoryItem)
            {
                var transform = listItems[i].GetChild(0);
                var sr = transform.GetComponent<SpriteRenderer>();
                sr.sprite = _inventoryCellSelected;
            }
            else
            {
                var transform = listItems[i].GetChild(0);
                var sr = transform.GetComponent<SpriteRenderer>();
                sr.sprite = _inventoryCellSelectionDismissed;
            }
        }


    }

    /// <summary>
    /// Добавить итем винвентарь
    /// </summary>
    /// <param name="i">Item</param>
    /// <returns>успешно/рюкзак полон</returns>
    public bool AddItemToInventory(Item i, Sprite _small_icon_for_panel)
    {
        if (Inventory[0] == null)
        {
            Inventory[0] = i;

            UpdateUIInventory(0, _small_icon_for_panel);

            return true;
        }
        else if (Inventory[1] == null)
        {
            Inventory[1] = i;

            UpdateUIInventory(1, _small_icon_for_panel);

            return true;
        }
        else if (Inventory[2] == null)
        {
            Inventory[2] = i;

            UpdateUIInventory(2, _small_icon_for_panel);

            return true;
        }
        else if (Inventory[3] == null)
        {
            Inventory[3] = i;

            UpdateUIInventory(3, _small_icon_for_panel);

            return true;
        }
        else
        {
            Debug.LogError("Рюкзак полон");

            return false;
        }
    }

    private void UpdateUIInventory(int inventoryIndex, Sprite sprite, bool ClearItem = false)
    {
        var taggedObjects = GameObject.FindGameObjectsWithTag("UpperPanelTag");

        var nameToSearh = "";

        if (this.name == "Hero-Baealog")
            nameToSearh = "BaleogStatusBar";
        else if (this.name == "Hero-Olaf")
            nameToSearh = "OlafStatusBar";
        else
            nameToSearh = "UlrichStatusBar";

        GameObject hero = taggedObjects.Where(go => go.name == nameToSearh).First();

        var inventoryCell1 = hero.transform.GetChild(inventoryIndex);
        var sr = inventoryCell1.GetComponent<SpriteRenderer>();
        sr.sprite = sprite;
    }

    /// <summary>
    /// Удалить текущий (выбранный) в инваентаре элемент
    /// </summary>
    /// <returns> успешно (итем исчез) / не успешно (итем не исчез)</returns>
    public void RemoveItemFromInventory()
    {
        Inventory[indexSelectedInventoryItem] = null;

        //удалить картинку из инвентаря

        UpdateUIInventory(indexSelectedInventoryItem, _default_inv_sprite, true);
    }
    #endregion
}
