using UnityEngine;

/// <summary>
/// Elevator kills hero if he's under elevator
/// 
/// Not used
/// </summary>
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
