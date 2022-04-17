using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaSpriteMoves : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;

    [SerializeField] private Sprite Left;
    [SerializeField] private Sprite Right;
    [SerializeField] private Sprite Idle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         if (Input.GetKey(KeyCode.A))
        {
            GetComponent<SpriteRenderer>().sprite = Left;

            Movement(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            GetComponent<SpriteRenderer>().sprite = Right;

            Movement(Vector3.right);
        }
         else
        {
            GetComponent<SpriteRenderer>().sprite = Idle;
        }
    }

    private void Movement(Vector3 direction)
    {
        _target.position += direction * _speed;

    }

}
