using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField] public int Health = 1;

    /// <summary>
    /// ������ ��������������
    /// </summary>
    [SerializeField] public Vector3 StartPointPatrolling { get; set; }
    /// <summary>
    /// ����� ��������������
    /// </summary>
    [SerializeField] public Vector3 EndPointPatrolling { get; set; }
    /// <summary>
    /// ��������
    /// </summary>
    [SerializeField] public int Speed = 5;

    [SerializeField] public Sprite EnemySprite { get; set; }

    /// <summary>
    /// ���� ���� -> ��������� * 5;
    /// </summary>
    [SerializeField] public bool IsBoss = false;

    public Enemy()
    {
        // ��� ��������� ???
        if (IsBoss)
        {
            Health *= 5;
        }
    }


    /// <summary>
    /// EVENT - �������
    /// </summary>
    public void OnAttack()
    {

    }

    /// <summary>
    /// EVENT - ��������������� �� StartPointPatrolling �� EndPointPatrolling � �������
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
