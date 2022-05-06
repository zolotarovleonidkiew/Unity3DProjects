using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Actions : MonoBehaviour
{

    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            Movement(Vector3.forward);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Movement(Vector3.back);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Movement(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Movement(Vector3.right);
        }
    }

    private void Movement(Vector3 direction)
    {
        transform.position += direction * _speed;
    }
}
