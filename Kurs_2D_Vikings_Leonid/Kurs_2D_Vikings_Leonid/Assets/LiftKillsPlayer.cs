using UnityEngine;

public class LiftKillsPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("[LiftKillsPlayer] Collision hit detected");

        var pmController = collision.GetComponent<CharacterController2D>();

        if (pmController != null)
        {
            pmController.CurrentHealth = 0;
        }

    }
}
