using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ��������, ������� ����� ��������� ������ � ���������
/// </summary>
public enum ItemTypes
{
    Food,
    Bomb,
    Drinks
}

/// <summary>
/// ������� ��������� / �������� �� ����
/// </summary>
public abstract partial class Item : MonoBehaviour
{
    public ItemTypes Type { get; set; }
    public Sprite ItemSprite { get; set; }

    /// <summary>
    /// ��������, ������� ����� ��������� ������ � ���� ��������� ��� ��� �������������
    /// </summary>
    abstract public void ApplyItem(Character player);
}

/// <summary>
/// ��������  Unity
/// </summary>
public partial class Item : MonoBehaviour
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
/// ��� - ��������������� ��������
/// </summary>
public class FoodItem : Item
{
    public FoodItem()
    {
        Type = ItemTypes.Food;
    }

    public const int RestoreHealthpoints = 3;

    public override void ApplyItem(Character player)
    {
        //�������
        player.CurrentHealth += RestoreHealthpoints;
    }
}

/// <summary>
/// ����� - ���������� ��� � ������� 3 ������
/// </summary>
public class BombItem : Item
{
    /// <summary>
    /// ���� ���������� �����
    /// </summary>
    public Vector3 BombPlantedPosition { get; set; }

    public BombItem()
    {
        Type = ItemTypes.Bomb;
    }

    public const int DestroyRadius = 3;

    public override void ApplyItem(Character player)
    {
        //����� ����� ������ ���������, � ��� ����� 3 ������� ������ ���������� (��������)
        OnBombPlant(player.CharacterPosition);
    }

    /// <summary>
    /// EVENT - ������ ������� �����
    /// </summary>
    /// <param name="position"></param>
    public void OnBombPlant(Vector3 position)
    {
        BombPlantedPosition = position;

        //������ + ����� + ��������
        //���������� ���
    }
}

/// <summary>
/// ����� - ��������� ������� �������-����������
/// </summary>
public class VodkaItem : Item
{
    public VodkaItem()
    {
        Type = ItemTypes.Drinks;
    }

    public const int DecreaseHealthPoint = 3;

    public override void ApplyItem(Character player)
    {
        //��������� �������� (�����)
        player.CurrentHealth -= DecreaseHealthPoint;
    }
}