using Assets.Scripts;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : MonoBehaviour
{
    /// <summary>
    /// Имя викинга (для лога)
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Позиция викинга
    /// </summary>
    public Vector3 CharacterPosition { get; set; }

    //2D
    public float MotionSpeed { get; set; }
    public float JumpForce { get; set; }
    public Sprite CharacterSprite { get; set; }

    /// <summary>
    /// Тип: викинг 1, 2, 3 или враг
    /// </summary>
    public CharacterType Type {get;set;}

    //МАКС здоровья
    public const int MaxHealthPoint = 3;
    //МАКС элементов в инвентаре
    public const int MaxItemsCount = 2;
    //Инвентарь: ARR[0][1]
    public Item[] InventoryArr;
    //Выбранная ячейка инвентаря
    public int SelectedItem = 0;

    // Разделение викинга по способностям
    protected bool CanJump { get; set; }
    protected bool CanShoot { get; set; }
    protected bool CanDefendByShield { get; set; }

    //Статус / здоровье
    private int _currentHealth;
    private bool _isDead;

    /// <summary>
    /// Отслеживание здоровья
    /// </summary>
    public int CurrentHealth
    {
        get { return _currentHealth; }
        set 
        {
            _currentHealth += value;

            if (_currentHealth > MaxHealthPoint)
                _currentHealth = MaxHealthPoint;
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

            if (_isDead)
            {
                Debug.Log($"Viking {Name} dead");

                //Нарисовать взрыв персонажа
            }
        }
    }

    /// <summary>
    /// Направление движения - лево/право, нужно для стрельбы
    /// </summary>
    public MovementDirections Direction { get; set; }

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="name">Имя персонажа</param>
    /// <param name="type">Тип викинга</param>
    public Character(string name, CharacterType type)
    {
        Name = name;
        Type = type;

        InventoryArr = new Item[MaxItemsCount];

        _currentHealth = MaxHealthPoint;

        _isDead = false;

        Debug.Log($"Viking {name} created");
    }

    /// <summary>
    /// Выполнить для каждого Викинга свое умение (прыгнуть, выстрелить, поднять щит)
    /// </summary>
    public virtual void UseSpecificAction()
    {
        Debug.Log("UseSpecificAction executed");
    }


    /// <summary>
    /// EVENTS *********************************************************** СОБЫТИЯ ***********************************************************
    /// </summary>


    /// <summary>
    /// Викинг поднял предмет
    /// </summary>
    public void OnPickupItem(Item item)
    {
        if (InventoryArr[0] == null)
            InventoryArr[0] = item;
        else if (InventoryArr[1] == null)
            InventoryArr[1] = item;
        else
        {
            //nothing
            Debug.LogWarning($"[{Name}]: инентарь полон");
        }
    }

    /// <summary>
    /// Применить выбранный элемент инвентаря (можно выбрать пустой)
    /// </summary>
    public void OnApplyItem()
    {
        var item = InventoryArr[SelectedItem];

        if (item != null)
        {
            item.ApplyItem(this);

            InventoryArr[SelectedItem] = null;
        }
    }

    /// <summary>
    /// Викинг умирает
    /// </summary>
    public void OnDie()
    {
        //update sprite
    }

    //перенесено в оверрайд метод специального действия каждого викинга

    ///////// <summary>
    ///////// Викинг выстрелил
    ///////// </summary>
    ///////// <param name="item"></param>
    //////public void OnShoot(Item item)
    //////{
    //////    if (CanShoot)
    //////    {
    //////        //create arrow
    //////    }
    //////}

    /// <summary>
    /// Смена активной ячейки инвеентаря
    /// </summary>
    public void OnIndexChangedSelectInventory()
    {
        if (SelectedItem == 0)
            SelectedItem = 1;
        else
            SelectedItem = 0;
    }

    /// <summary>
    /// Персонаж ввигается ????
    /// </summary>
    public void OnMove()
    {
        
    }
}

/// <summary>
/// События Unity
/// </summary>
public partial class Character : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


/// <summary>
/// Направление движения (надо для стрельбы)
/// </summary>
public enum MovementDirections
{
    left = 1,
    right = 2
}