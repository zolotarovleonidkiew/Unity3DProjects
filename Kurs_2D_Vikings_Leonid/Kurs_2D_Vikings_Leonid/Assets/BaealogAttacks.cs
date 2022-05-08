using UnityEngine;

/// <summary>
/// Баелог кого-то рубанул мечом
/// </summary>
public class BaealogAttacks : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Baealog attacks collision hit detected");

        var enemyController = collision.GetComponent<Enemy>(); // WHY null ???

        if (enemyController != null)
        {
            enemyController.TakeDamage(1);
        }

    }
}
