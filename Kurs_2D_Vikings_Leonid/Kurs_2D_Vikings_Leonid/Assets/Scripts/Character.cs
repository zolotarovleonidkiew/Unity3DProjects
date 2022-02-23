using Assets.Scripts;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public partial class Character : MonoBehaviour
{
    /// <summary>
    /// ��� ������� (��� ����)
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// ������� �������
    /// </summary>
    public Vector3 CharacterPosition { get; set; }

    //2D
    public float MotionSpeed { get; set; }
    public float JumpForce { get; set; }
    public Sprite CharacterSprite { get; set; }

    /// <summary>
    /// ���: ������ 1, 2, 3 ��� ����
    /// </summary>
    public CharacterType Type {get;set;}

    //���� ��������
    public const int MaxHealthPoint = 3;
    //���� ��������� � ���������
    public const int MaxItemsCount = 2;
    //���������: ARR[0][1]
    public Item[] InventoryArr;
    //��������� ������ ���������
    public int SelectedItem = 0;

    // ���������� ������� �� ������������
    protected bool CanJump { get; set; }
    protected bool CanShoot { get; set; }
    protected bool CanDefendByShield { get; set; }

    //������ / ��������
    private int _currentHealth;
    private bool _isDead;

    /// <summary>
    /// ������������ ��������
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
    /// ������������ ��������� �����
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

                //���������� ����� ���������
            }
        }
    }

    /// <summary>
    /// ����������� �������� - ����/�����, ����� ��� ��������
    /// </summary>
    public MovementDirections Direction { get; set; }

    /// <summary>
    /// .ctor
    /// </summary>
    /// <param name="name">��� ���������</param>
    /// <param name="type">��� �������</param>
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
    /// ��������� ��� ������� ������� ���� ������ (��������, ����������, ������� ���)
    /// </summary>
    public virtual void UseSpecificAction()
    {
        Debug.Log("UseSpecificAction executed");
    }


    /// <summary>
    /// EVENTS *********************************************************** ������� ***********************************************************
    /// </summary>


    /// <summary>
    /// ������ ������ �������
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
            Debug.LogWarning($"[{Name}]: �������� �����");
        }
    }

    /// <summary>
    /// ��������� ��������� ������� ��������� (����� ������� ������)
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
    /// ������ �������
    /// </summary>
    public void OnDie()
    {
        //update sprite
    }

    //���������� � �������� ����� ������������ �������� ������� �������

    ///////// <summary>
    ///////// ������ ���������
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
    /// ����� �������� ������ ����������
    /// </summary>
    public void OnIndexChangedSelectInventory()
    {
        if (SelectedItem == 0)
            SelectedItem = 1;
        else
            SelectedItem = 0;
    }

    /// <summary>
    /// �������� ��������� ????
    /// </summary>
    public void OnMove()
    {
        
    }
}

/// <summary>
/// ������� Unity
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
/// ����������� �������� (���� ��� ��������)
/// </summary>
public enum MovementDirections
{
    left = 1,
    right = 2
}