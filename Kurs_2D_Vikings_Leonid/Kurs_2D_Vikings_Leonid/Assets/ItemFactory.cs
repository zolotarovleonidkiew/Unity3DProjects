public class ItemFactory
{
    /// <summary>
    /// Создаем предмет инвентаря по его типу
    /// </summary>
    public Item CreateItemByType(ItemTypes type)
    {
        if (type == ItemTypes.BlueKey)
        {
            return new BlueKey();
        }
        else if (type == ItemTypes.RedKey)
        {
            return new RedKey();
        }
        else if (type == ItemTypes.Drinks)
        {
            return new VodkaItem();
        }
        else if (type == ItemTypes.Food)
        {
            return new FoodItem();
        }
        else if (type == ItemTypes.YellowKey)
        {
            return new YellowKey();
        }
        else
        {
            throw new ItemDoesNotExistsException(type.ToString());
        }
    }
}