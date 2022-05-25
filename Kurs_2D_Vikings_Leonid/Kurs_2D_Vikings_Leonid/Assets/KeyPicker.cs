using UnityEngine;

/// <summary>
/// Represents any item to be picked
/// 
/// ��������� �����/����� � ��. � ���� ����� � ���������
/// 
/// https://www.youtube.com/watch?v=12AbcoLrYKM
/// </summary>
public class KeyPicker : MonoBehaviour
{
    /// <summary>
    /// ��� ������������ item'�
    /// </summary>
    [SerializeField] public ItemTypes currentTypeOfItem;

    /// <summary>
    /// ����������� �������� item'�, ���� ����������� �� ������
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
            //���� ���-�� ����-�� ������� - �� ���� ���-�� ������ ����� ����, � ��� - ��� ���  ������ ��� �������
            
            //TODO: ���� ����� ����� �� ������� ����, �� ����������� ���, �� ������� ��� ��������
            // FIX !!!!
            if (controller.Inventory == null)
            {
                controller.CreateEmptyInventory();
            }

            if (controller.Inventory.Length <= 4) // ��������
            {
                var factory = new ItemFactory();

                var item = factory.CreateItemByType(currentTypeOfItem);

                if (controller.AddItemToInventory(item, _small_icon_for_panel))
                {
                    Debug.Log($"Picked up {item.Type}");

                    //��������� �������� ������, �.�. ��� ���������
                    var renderer = this.GetComponent<SpriteRenderer>();
                    renderer.enabled = false;

                    var collissionBox = this.GetComponent<Collider2D>();
                    collissionBox.enabled = false;

                    //�������� �������� ����� � ��������� ���������
                }
            }
            else
            {
                Debug.LogWarning("���������� �����");
            }
        }

    }
}
