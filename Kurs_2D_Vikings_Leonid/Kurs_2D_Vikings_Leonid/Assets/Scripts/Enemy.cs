using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /// <summary>
    /// Здоровье
    /// </summary>
    [SerializeField] private int Health = 1;

    /// <summary>
    /// Сила атаки
    /// </summary>
    [SerializeField] private int Attack = 1;

    [SerializeField] private float _patrulPathLength = 5;

    /// <summary>
    /// Скрорсть
    /// </summary>
    [SerializeField] private int Speed = 5;

    [SerializeField] private Sprite EnemySprite { get; set; }

    /// <summary>
    /// БОСС
    /// </summary>
    [SerializeField] private bool IsBoss = false;


    private Vector3 _startPosition;
    private float _XstartPatrul;
    private float _XendPatrul;

    private bool _pointAreached;
    private bool _pointBreached;

    private bool _playerIconDirectionToRight = true;

    bool shouldDie = false;

    //2D
    private void Start()
    {
        _startPosition = this.transform.position;

        _XstartPatrul = _startPosition.x - _patrulPathLength;
        _XendPatrul = _startPosition.x + _patrulPathLength;
    }

    private void Update()
    {
        if (shouldDie)
        {
            shouldDie = false;

            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        bool shoudAttack = AlienDecidedToAttack();

        if (!shoudAttack)
        {
            Patrul();
        }
    }

    private bool AlienDecidedToAttack()
    {
        return false;
    }

    private void Patrul()
    {
        var currentPosition = this.transform.position;

        if (currentPosition.x <= _XstartPatrul)
        {
            _pointAreached = true;
            _pointBreached = false;
        }
        if (currentPosition.x >= _XendPatrul)
        {
            _pointAreached = false;
            _pointBreached = true;
        }


        if (_pointAreached && !_pointBreached)
        {
            if (!_playerIconDirectionToRight)
            {
                Flip();
            }
            //move to B
            this.transform.position += Vector3.right * Speed * Time.deltaTime;
        }
        else if (!_pointAreached && _pointBreached)
        {
            if (_playerIconDirectionToRight)
            {
                Flip();
            }

            //move to a
            this.transform.position += Vector3.left * Speed * Time.deltaTime;
        }
        else if (!_pointAreached && !_pointBreached)
        {
            if (_playerIconDirectionToRight)
            {
                Flip();
            }
            //move to a
            this.transform.position += Vector3.left * Speed * Time.deltaTime;
        }

    }

    private void Flip()
    {
        _playerIconDirectionToRight = !_playerIconDirectionToRight;

        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Alien collision hit detected");

        var pmController = collision.GetComponent<CharacterController2D>();

        if (pmController != null)
        {
            pmController.TakeDamage(Attack);

            Debug.LogError($"Damage: {Attack}");
        }

    }

    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// TO DO: добавить анимацию смерти
    /// </summary>
    void Die()
    {
        shouldDie = true;
    }
}
