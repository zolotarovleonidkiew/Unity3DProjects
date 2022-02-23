using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Тип элемента, который может поместить викинг в инвентарь
/// </summary>
public enum ItemTypes
{
    Food,
    Bomb,
    Drinks
}

/// <summary>
/// Элемент инвентаря / валяется на полу
/// </summary>
public abstract partial class Item : MonoBehaviour
{
    public ItemTypes Type { get; set; }
    public Sprite ItemSprite { get; set; }

    /// <summary>
    /// Действие, которое может совершить викинг с этим предметом при его ИСПОЛЬЗОВАНИИ
    /// </summary>
    abstract public void ApplyItem(Character player);
}

/// <summary>
/// Сообытия  Unity
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
/// Еда - восстанавливает здоровье
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
        //лечение
        player.CurrentHealth += RestoreHealthpoints;
    }
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

    public override void ApplyItem(Character player)
    {
        //бомбу можно только поместить, и она через 3 секунды должна взорваться (корутина)
        OnBombPlant(player.CharacterPosition);
    }

    /// <summary>
    /// EVENT - викинг положил бомбу
    /// </summary>
    /// <param name="position"></param>
    public void OnBombPlant(Vector3 position)
    {
        BombPlantedPosition = position;

        //таймер + взырв + корутина
        //нарисовать БУМ
    }
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

    public override void ApplyItem(Character player)
    {
        //уменьшить здоровье (убить)
        player.CurrentHealth -= DecreaseHealthPoint;
    }
}