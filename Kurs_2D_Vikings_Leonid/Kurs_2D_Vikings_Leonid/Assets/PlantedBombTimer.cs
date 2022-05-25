using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Planted bomb controller - timer andf explosion were  set here
/// 
/// TO DO: display explosion animation on bomb's sprite
/// </summary>
public class PlantedBombTimer : MonoBehaviour
{
    [SerializeField] private int _timer = 3;

    [SerializeField] public Sprite Explotion;

    private DateTime _bombShouldExplodeAt;

    private bool _bombExploded = false;
    private bool _bombShouldExplode = false;

    private bool _explotionShouldStop = false;

    private List<PlayerMovement> HeroesInExplodedZone = new List<PlayerMovement>();
    private List<Enemy> EnemiesInExplodedZone = new List<Enemy>();

    private DateTime _explotionShoulEndAt;

    private GameObject explotion;

    private void Start()
    {
        _bombShouldExplodeAt = DateTime.Now.AddSeconds(_timer);
    }

    private void Update()
    {
        if (DateTime.Now >= _bombShouldExplodeAt && !_bombExploded)
        {
            _bombShouldExplode = true;
        }

        if (DateTime.Now >= _explotionShoulEndAt && _explotionShoulEndAt != new DateTime())
        {
            _explotionShouldStop = true;
        }
    }

    private void FixedUpdate()
    {
        //Animation
        if (_bombShouldExplode)
        {
            _bombShouldExplode = false;

            CalculateDamages();

            _bombExploded = true;

            var sr = this.gameObject.GetComponent<SpriteRenderer>();
            sr.enabled = false;
            //Destroy(this.gameObject);

            CreateExplotionImage();
        }

        if (_explotionShouldStop)
        {
            Destroy(this.gameObject);
            Destroy(explotion);

            _explotionShouldStop = false;
        }
    }

    private void CalculateDamages()
    {
        int bombDamage = BombItem.DestroyRadius;

        foreach (PlayerMovement p in HeroesInExplodedZone)
        {
            p.controller.CurrentHealth -= bombDamage;
        }

        //fix collection currepted exception (can't use foreach here)
        for (int i = 0; i < EnemiesInExplodedZone.Count; i++)
        {
            var enemy = EnemiesInExplodedZone[i];
            enemy.TakeDamage(bombDamage);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var pmController = collision.GetComponent<PlayerMovement>();

        if (pmController != null)
        {
            HeroesInExplodedZone.Add(pmController);
        }

        var enemyController = collision.GetComponent<Enemy>();

        if (enemyController != null)
        {
            EnemiesInExplodedZone.Add(enemyController);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var pmController = collision.GetComponent<PlayerMovement>();

        if (pmController != null)
        {
            HeroesInExplodedZone.Remove(pmController);
        }

        var enemyController = collision.GetComponent<Enemy>();

        if (enemyController != null)
        {
            EnemiesInExplodedZone.Remove(enemyController);
        }
    }

    private void CreateExplotionImage()
    {
        if (explotion == null)
        {
            explotion = new GameObject($"{this.name}_explotion");
            explotion.AddComponent<SpriteRenderer>();
            explotion.AddComponent<BoxCollider2D>();
            explotion.AddComponent<Transform>();

            var tc = explotion.GetComponent<Transform>();
            tc.position = transform.position + Vector3.up * 3;

            var sr = explotion.GetComponent<SpriteRenderer>();
            sr.sprite = Explotion;
            sr.size = new Vector2(300, 300);
            sr.sortingOrder = 5;

            var bc = explotion.GetComponent<BoxCollider2D>();
            bc.size = new Vector2(1, 7);

            _explotionShoulEndAt = DateTime.Now.AddSeconds(2);
        }
    }
}
