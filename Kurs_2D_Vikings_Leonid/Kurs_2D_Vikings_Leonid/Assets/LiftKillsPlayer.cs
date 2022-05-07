using UnityEngine;

public class LiftKillsPlayer : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision hit detected");

        var pmController = collision.GetComponent<PlayerMovement>();

        if (pmController != null)
        {
            pmController.Kill();
        }

    }
}
