using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    [SerializeField] float _speed;

    private Animator _animator;

    private CharacterMovement _cm;

    private float _sensitivity = 4;

    void Start()
    {
        _cm = new CharacterMovement();

        _animator = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }

    /// <summary>
    /// Animation
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)
            || Input.GetKeyDown(KeyCode.S)
            || Input.GetKeyDown(KeyCode.A) 
            || Input.GetKeyDown(KeyCode.D))
        {
            _animator.SetBool("ShouldRun", true);
        }

        if (Input.GetKeyUp(KeyCode.W)
            || Input.GetKeyUp(KeyCode.S)
            || Input.GetKeyUp(KeyCode.A)
            || Input.GetKeyUp(KeyCode.D))
        {
            _animator.SetBool("ShouldRun", false);
        }
    }

    /// <summary>
    /// Movements
    /// </summary>
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _cm.Movement(transform, Vector3.forward, _speed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            _cm.Movement(transform, Vector3.back, _speed);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            _cm.Movement(transform, Vector3.left, _speed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            _cm.Movement(transform, Vector3.right, _speed);
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            _cm.Movement(transform, Vector3.up, _speed);
        }
    }
}
