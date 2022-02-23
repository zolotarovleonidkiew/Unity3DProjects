using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������, ������� ��������� ������ � ������������
/// 
/// ����� ������� � ������������/����� �����
/// </summary>
public class Arrow : MonoBehaviour
{
    /// <summary>
    /// ������ �������� ������ (������� �������)
    /// </summary>
    public Vector3 StartPosition { get; set; }

    /// <summary>
    /// ���� ����� ������ - �����/������ � ������������ ���� �� �������, �� ����� ������
    /// </summary>
    public Vector3 EndPosition { get; set; }

    /// <summary>
    /// �������� ������
    /// </summary>
    public float Speed = 10;

    public Sprite ArrowSprite { get; set; }

    /*
     * 
     [?] ��� ���������� ������� ������ ����� ��� ������? Viewport ?
     
     */

    public Arrow(Vector3 startPosition, MovementDirections direction)
    {
        StartPosition = startPosition;

        /*DEFINE EndPosition

        ������ ����� ���� ����� ��� ������, � ����������� �� ���� ������� ������

        */
        if (direction == MovementDirections.left)
        {
            EndPosition = new Vector3(-10, startPosition.y, startPosition.z);
        }
        else if (direction == MovementDirections.right)
        {
            EndPosition = new Vector3(10, startPosition.y, startPosition.z);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        /*
         * ������ �����
         */
    }

    /// <summary>
    /// EVENTS - ������ ����/�� ������ (�������� ��� ����)
    /// </summary>
    public void OnArrowShootSomething()
    {
        //������ ����-�� ������

        // ��� ???
    }
}

