using UnityEngine;

/// <summary>
/// Inventory item's type
/// 
/// ��� ��������, ������� ����� ��������� ������ � ���������
/// </summary>
public enum ItemTypes
{
    BlueKey,
    RedKey,
    YellowKey,
    Food,
    Bomb,
    Drinks
}

/// <summary>
/// Element of viking's inventory
/// 
/// ������� ��������� / �������� �� ����
/// </summary>
public class Item
{
    public ItemTypes Type { get; set; }
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

    public int RestoreHealthPoints = 1;

}

/// <summary>
/// ����� - ���������� ��� � ������� {DestroyRadius} ������
/// </summary>
public class BombItem : Item
{
    public BombItem()
    {
        Type = ItemTypes.Bomb;
    }

    public const int DestroyRadius = 3;

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
}

/// <summary>
/// ����� ����
/// </summary>
public class BlueKey : Item
{
    public BlueKey()
    {
        Type = ItemTypes.BlueKey;
    }

}

/// <summary>
/// ������� ����
/// </summary>
public class RedKey : Item
{
    public RedKey()
    {
        Type = ItemTypes.RedKey;
    }

}

/// <summary>
/// ������ ����
/// </summary>
public class YellowKey : Item
{
    public YellowKey()
    {
        Type = ItemTypes.YellowKey;
    }

}