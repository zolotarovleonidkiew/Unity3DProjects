using UnityEngine;

/// <summary>
/// Baealog attacks melee weapon
/// 
/// Баелог кого-то рубанул мечом
/// </summary>
public class BaealogAttacks : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemyController = collision.GetComponent<Enemy>();

        if (enemyController != null)
        {
            enemyController.TakeDamage(1);
        }

    }
}
