using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    [SerializeField] private float _speed = 5;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Movements
    /// </summary>
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.D)) // W
        {
            Movement(transform, Vector3.forward, _speed);
        }
        else if (Input.GetKey(KeyCode.A)) //S
        {
            Movement(transform, Vector3.back, _speed);
        }
        else if (Input.GetKey(KeyCode.W)) //A
        {
            Movement(transform, Vector3.left, _speed);
        }
        else if (Input.GetKey(KeyCode.S)) //D
        {
            Movement(transform, Vector3.right, _speed);
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            Movement(transform, Vector3.up, _speed);
        }
    }

    private void Movement(Transform transform, Vector3 direction, float speed)
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
