using UnityEngine;

/// <summary>
/// Represents any item to be picked
/// 
/// Подбираем ключи/бомбу и пр. с пола прямо в инвентарь
/// 
/// https://www.youtube.com/watch?v=12AbcoLrYKM
/// </summary>
public class KeyPicker : MonoBehaviour
{
    /// <summary>
    /// Тип подбираемого item'а
    /// </summary>
    [SerializeField] public ItemTypes currentTypeOfItem;

    /// <summary>
    /// Уменьшенная картинка item'а, дляч отображения на панели
    /// </summary>
    [SerializeField] private Sprite _small_icon_for_panel;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterController2D controller = collision.GetComponent<CharacterController2D>();

        if (controller == null)
        {
           // Debug.LogError("KeyPicker => OnTriggerEnter2D => controller is null");
        }
        else
        {
            //если кто-то кого-то толкнул - то этот кто-то должен взять итем, а так - тот кто  ТОЛКАЛ его получил
            
            //TODO: если после этого он возьмет итем, то перезапишет тот, на который его толкнули
            // FIX !!!!
            if (controller.Inventory == null)
            {
                controller.CreateEmptyInventory();
            }

            if (controller.Inventory.Length <= 4) // дубликат
            {
                var factory = new ItemFactory();

                var item = factory.CreateItemByType(currentTypeOfItem);

                if (controller.AddItemToInventory(item, _small_icon_for_panel))
                {
                    Debug.Log($"Picked up {item.Type}");

                    //выключить картинку айтема, т.к. его подобрали
                    var renderer = this.GetComponent<SpriteRenderer>();
                    renderer.enabled = false;

                    var collissionBox = this.GetComponent<Collider2D>();
                    collissionBox.enabled = false;

                    //добавить картинку Итема в инвентарь персонажа
                }
            }
            else
            {
                Debug.LogWarning("Инрвентарь полон");
            }
        }

    }
}
