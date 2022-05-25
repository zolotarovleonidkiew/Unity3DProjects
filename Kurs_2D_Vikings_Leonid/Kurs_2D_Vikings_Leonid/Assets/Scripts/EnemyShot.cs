using UnityEngine;

/// <summary>
/// Vikings take damage from distance weapon(plasma shots)
/// </summary>
public class EnemyShot : MonoBehaviour
{
    [SerializeField] private bool _playerTakeDamageOnCollision = true;
    [SerializeField] public int Damage { get; internal set; }
    [SerializeField] public GameObject Bullet { get; internal set; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_playerTakeDamageOnCollision)
        {
            var pmController = collision.GetComponent<CharacterController2D>();

            if (pmController != null)
            {
                var damage = Damage;

                if (pmController.VikingName == "Olaf")
                {
                    if (pmController.transform.position.x < this.transform.position.x && pmController.PlayerIconDirectionToRight)
                    {
                        //shield blocks attack from right
                        damage = 0;
                    }
                    else if (pmController.transform.position.x > this.transform.position.x && !pmController.PlayerIconDirectionToRight)
                    {
                        //shield blocks attack from left
                        damage = 0;
                    }
                }
             
                pmController.CurrentHealth -= damage;
            }
        }

        Destroy(Bullet.GetComponent<BoxCollider2D>());
        Destroy(Bullet.GetComponent<SpriteRenderer>());
    }
}
