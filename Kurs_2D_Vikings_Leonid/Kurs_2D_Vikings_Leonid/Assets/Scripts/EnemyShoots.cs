using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plasma-Gun controller
/// 
/// Плазма-ган стреляет каждые {_delay} секунд/ы
/// </summary>
public class EnemyShoots : MonoBehaviour
{
    [SerializeField] private int _delay = 1;
    [SerializeField] private float speed = 20;
    [SerializeField] private Sprite _bulletSprite;
    private DateTime _lastShotTime;

    private List<GameObject> _bullets = new List<GameObject>();

    private Vector3 _target;

    private void Start()
    {
        _target = this.transform.GetChild(0).position;
    }

    void FixedUpdate()
    {
        foreach (var b in _bullets)
        {
            if (b.transform.position.x > _target.x)
            {
                b.transform.position += Vector3.left * speed * Time.deltaTime;
            }
            else
            {
                Destroy(b.GetComponent<SpriteRenderer>());
                Destroy(b.GetComponent<BoxCollider2D>());
            }
            
        }
    }

    void Update()
    {
        if (_lastShotTime == default(DateTime) || DateTime.Now >= _lastShotTime.AddSeconds(_delay))
        {
           // _createdBullet = null;

            //Добавить звук

            Shoot();

            _lastShotTime = DateTime.Now;
        }
    }

    private void Shoot()
    {
        var _createdBullet = new GameObject($"bullet_{DateTime.Now}");

        SpriteRenderer sr = _createdBullet.AddComponent<SpriteRenderer>();

        sr.sprite = _bulletSprite;
        sr.sortingOrder = 5;

        _createdBullet.AddComponent<Transform>();

        _createdBullet.transform.position = this.transform.position - Vector3.left - Vector3.left;

        var killCollider = _createdBullet.AddComponent<BoxCollider2D>();
        killCollider.size = new Vector2(2, 1);
        killCollider.isTrigger = true;

        var script = _createdBullet.AddComponent<EnemyShot>();
        script.Damage = 1;
        script.Bullet = _createdBullet;

        _bullets.Add(_createdBullet);
    }
}
