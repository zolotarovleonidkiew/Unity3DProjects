using UnityEngine;

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
                pmController.CurrentHealth -= Damage;
            }
        }

        Destroy(Bullet.GetComponent<BoxCollider2D>());
        Destroy(Bullet.GetComponent<SpriteRenderer>());
    }
}
