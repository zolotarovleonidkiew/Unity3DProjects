using UnityEngine;

public class LiftPlatform : MonoBehaviour
{
    [Header("Lift Settings")]
    public float targetFloorHeight = 4f; // висота другого поверху
    public float speed = 2f;             // швидкість руху
    public bool autoMove = false;// true;         // чи їздить автоматично
    
    public GameObject HeroesCollectionGUI;

    private Vector3 _startPos;
    private Vector3 _endPos;
    private Vector3 _targetPos;
    private bool _isMoving = false;

    void Start()
    {
        _startPos = transform.position;
        _endPos = new Vector3(_startPos.x, _startPos.y + targetFloorHeight, _startPos.z);
        _targetPos = _startPos;
    }

    void Update()
    {
        if (autoMove)
        {
            // якщо авто-режим, їздимо постійно туди-сюди
            if (!_isMoving)
            {
                if (Vector3.Distance(transform.position, _startPos) < 0.01f)
                    CallLiftUp();
                else if (Vector3.Distance(transform.position, _endPos) < 0.01f)
                    CallLiftDown();
            }
        }

        if (_isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPos,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, _targetPos) < 0.01f)
                _isMoving = false;
        }
    }

    /// <summary>
    /// Lift calling
    /// </summary>
    public void CallLiftUp()
    {
        _targetPos = _endPos;
        _isMoving = true;
    }

    /// <summary>
    /// Lift calling
    /// </summary>
    public void CallLiftDown()
    {
        _targetPos = _startPos;
        _isMoving = true;
    }

    /// <summary>
    /// Trigger
    /// </summary>   
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // герой стає дочірнім об'єктом, щоб рухатись разом із ліфтом
            other.transform.SetParent(transform);
        }
    }

    /// <summary>
    /// Trigger
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // відв’язуємо героя назад
            other.transform.SetParent(HeroesCollectionGUI.transform);
            //other.transform.SetParent(null);
        }
    }
}
