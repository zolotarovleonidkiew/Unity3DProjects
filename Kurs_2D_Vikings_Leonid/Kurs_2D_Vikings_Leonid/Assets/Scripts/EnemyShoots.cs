using System;
using UnityEngine;

/// <summary>
/// ������-��� �������� ������ 3 �������
/// </summary>
public class EnemyShoots : MonoBehaviour
{
    [SerializeField] private int _delay = 3;
    [SerializeField] private float speed = 20;
    [SerializeField] private Sprite _bulletSprite;
    private DateTime _lastShotTime;

    private GameObject _createdBullet = null;

    void FixedUpdate()
    {
        if (_createdBullet != null)
        {
            _createdBullet.transform.position += Vector3.left * speed * Time.deltaTime;
        }
    }

    void Update()
    {
        if (_lastShotTime == default(DateTime) || DateTime.Now >= _lastShotTime.AddSeconds(_delay))
        {
            Destroy(_createdBullet);
            _createdBullet = null;

            //�������� ����

            Shoot();

            _lastShotTime = DateTime.Now;
        }
    }

    private void Shoot()
    {
        //Create bullet
        _createdBullet = new GameObject($"bullet_{DateTime.Now}");

        SpriteRenderer sr = _createdBullet.AddComponent<SpriteRenderer>();

        sr.sprite = _bulletSprite;
        sr.sortingOrder = 5;

        BoxCollider2D collider = _createdBullet.AddComponent<BoxCollider2D>(); //TO DO add attack !!!! ��� ������������ � ��������� �������
        collider.size = new Vector2(1, 1);

        _createdBullet.AddComponent<Transform>();

        _createdBullet.transform.position = this.transform.position - Vector3.left - Vector3.left;
    }
}
