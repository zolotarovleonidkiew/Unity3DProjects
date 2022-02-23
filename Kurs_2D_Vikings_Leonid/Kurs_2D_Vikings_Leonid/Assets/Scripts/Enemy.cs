using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /// <summary>
    /// Здоровье
    /// </summary>
    [SerializeField] public int Health = 1;

    /// <summary>
    /// Начало патрулирования
    /// </summary>
    [SerializeField] public Vector3 StartPointPatrolling { get; set; }
    /// <summary>
    /// Конец патрулирования
    /// </summary>
    [SerializeField] public Vector3 EndPointPatrolling { get; set; }
    /// <summary>
    /// Скрорсть
    /// </summary>
    [SerializeField] public int Speed = 5;

    [SerializeField] public Sprite EnemySprite { get; set; }

    /// <summary>
    /// Если БОСС -> живучесть * 5;
    /// </summary>
    [SerializeField] public bool IsBoss = false;

    public Enemy()
    {
        // Это сработает ???
        if (IsBoss)
        {
            Health *= 5;
        }
    }


    /// <summary>
    /// EVENT - атакует
    /// </summary>
    public void OnAttack()
    {

    }

    /// <summary>
    /// EVENT - патрулированиек от StartPointPatrolling до EndPointPatrolling и обратно
    /// </summary>
    public void OnPatrolling()
    {

    }

    //2D

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
