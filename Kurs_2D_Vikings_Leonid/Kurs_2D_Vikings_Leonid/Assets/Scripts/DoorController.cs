using UnityEngine;

/// <summary>
/// При приближении одного из трех персонажей дверь открываются, и по удалению персонажа
/// автоматом закрываются через 3 секунды
/// 
/// https://www.youtube.com/watch?v=ZoZcBgRR9ns
/// https://gamedevbeginner.com/how-to-change-a-sprite-from-a-script-in-unity-with-examples/#:~:text=To%20change%20a%20Sprite%20from%20a%20script%20in%20Unity%2C%20create,match%20the%20new%2C%20replacement%20Sprite.
/// audio:
/// https://gamedevbeginner.com/how-to-play-audio-in-unity-with-examples/
/// </summary>
public class DoorController : MonoBehaviour
{
    private CharacterController2D controller;

    public AudioSource audioSource;
    public SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite OpenedDoor;
    [SerializeField] private Sprite ClosedDoor;
    [SerializeField] bool NoKeyNeeded;
    [SerializeField] private ItemTypes RequiredItemToOpenDoor;

    public bool DoorReadyToOpenFlag = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision hit detected");

        if (!NoKeyNeeded)
        {
            // nothing
            SetFlagPlayerToOpenDoor(collision);
        }
        else
        {
            OpenDoor();
        }

    }

    void OpenDoor()
    {
        //if open - remove child collider
        if (transform.childCount == 1)
        {
            var childTransform = transform.GetChild(0);
            var child = childTransform.gameObject;
            var collider = child.GetComponent<BoxCollider2D>();
            collider.enabled = false;
        }

        spriteRenderer.sprite = OpenedDoor;

        PlayAudio();
    }

    void PlayAudio()
    {
        audioSource.Play();
    }


    void SetFlagPlayerToOpenDoor(Collider2D collision)
    {
        var pmController = collision.GetComponent<PlayerMovement>();

        if (pmController != null)
        {
            pmController.TryToOpenDoorFlag = true;
            pmController.NeededItemToOpenDoorType = RequiredItemToOpenDoor;
            pmController.GameObjectDoorReference = this;
        }
    }

    //персонаж не стал открывать дверь и вышел из коллайдера
    private void OnTriggerExit2D(Collider2D collision)
    {

        if (!NoKeyNeeded)
        {
            var pmController = collision.GetComponent<PlayerMovement>();

            if (pmController != null)
            {
                pmController.TryToOpenDoorFlag = false;
                pmController.GameObjectDoorReference = null;
            }
        }
        else
        {
            Debug.Log("Collision EXIT hit detected");

            spriteRenderer.sprite = ClosedDoor;

            PlayAudio();
        }


    }

    void Update()
    {
        if (DoorReadyToOpenFlag)
        {
            OpenDoor();
            NoKeyNeeded = true;
            DoorReadyToOpenFlag = false;
        }
    }
}
