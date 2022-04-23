using System.Collections;
using System.Collections.Generic;
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
    //  public Animator animator;
    public AudioSource audioSource;
    public SpriteRenderer spriteRenderer;
    public Sprite OpenedDoor;
    public Sprite ClosedDoor;

    public bool RequireBlueKey = false;
    public bool RequireRedKey = false;
    public bool RequireYellowKey = false;

    public bool RequireExplosion = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision hit detected");

        //animator.SetBool("doorToOpen", true);
        spriteRenderer.sprite = OpenedDoor;

        PlayAudio();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Collision EXIT hit detected");

        //animator.SetBool("doorToOpen", false);
        spriteRenderer.sprite = ClosedDoor;

      //  PlayAudio();
    }

    void PlayAudio()
    {
        audioSource.Play();
    }
}
