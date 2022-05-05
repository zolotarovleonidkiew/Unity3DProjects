using UnityEngine;

/// <summary>
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterController2D controller = collision.GetComponent<CharacterController2D>();

        if (controller == null)
        {
            Debug.LogError("KeyPicker => OnTriggerEnter2D => controller is null");
        }
        else
        {

            if (controller.Inventory.Length <= 4) // дубликат
            {
                var factory = new ItemFactory();

                var item = factory.CreateItemByType(currentTypeOfItem);

                if (controller.AddItemToInventory(item))
                {
                    Debug.Log($"Picked up {item.Type}");

                    //выключить картинку айтема, т.к. его подобрали
                    var renderer = this.GetComponent<SpriteRenderer>();
                    renderer.enabled = false;

                    var collissionBox = this.GetComponent<Collider2D>();
                    collissionBox.enabled = false;
                }
            }
            else
            {
                Debug.LogWarning("Инрвентарь полон");
            }
        }

    }
}
