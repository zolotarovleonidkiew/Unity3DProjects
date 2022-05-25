using UnityEngine;

/// <summary>
/// Inventory item's type
/// 
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
/// Element of viking's inventory
/// 
/// Элемент инвентаря / валяется на полу
/// </summary>
public class Item
{
    public ItemTypes Type { get; set; }
}


/// <summary>
/// Еда - восстанавливает здоровье
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
/// Бомба - уничтожает все в радиусе {DestroyRadius} клеток
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