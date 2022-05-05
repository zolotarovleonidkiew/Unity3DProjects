using UnityEngine;

/// <summary>
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
/// ������� ��������� / �������� �� ����
/// </summary>
public partial class Item
{
    public ItemTypes Type { get; set; }



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