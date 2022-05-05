using UnityEngine;

/// <summary>
/// Тип элемента, который может поместить викинг в инвентарь
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
/// Элемент инвентаря / валяется на полу
/// </summary>
public partial class Item
{
    public ItemTypes Type { get; set; }



/// <summary>
/// Еда - восстанавливает здоровье
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
/// Бомба - уничтожает все в радиусе 3 клеток
/// </summary>
public class BombItem : Item
{
    /// <summary>
    /// Куда установили бомбу
    /// </summary>
    public Vector3 BombPlantedPosition { get; set; }

    public BombItem()
    {
        Type = ItemTypes.Bomb;
    }

    public const int DestroyRadius = 3;

}

/// <summary>
/// Водка - мгновенно убивает викинга-алкоголика
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
/// Синий ключ
/// </summary>
public class BlueKey : Item
{
    public BlueKey()
    {
        Type = ItemTypes.BlueKey;
    }

}

/// <summary>
/// Красный ключ
/// </summary>
public class RedKey : Item
{
    public RedKey()
    {
        Type = ItemTypes.RedKey;
    }

}

/// <summary>
/// Желтый ключ
/// </summary>
public class YellowKey : Item
{
    public YellowKey()
    {
        Type = ItemTypes.YellowKey;
    }

}