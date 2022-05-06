using UnityEngine;

/// <summary>
/// Bot logic
/// </summary>
public class EnemyMoves : MonoBehaviour
{
    const float Xmin = 2.4f;
    const float Xmax = 5f;

    private CharacterMovement _cm;

    private float _speed = 1;

    private bool _movingToSecondPoint = true;

    void Start()
    {
        _cm = new CharacterMovement();

        _speed /= 2;
    }

    void FixedUpdate()
    {
        if (movingToSecondPoint)
        {
            cm.Movement(transform, Vector3.right, _speed);

            if (transform.position.x > Xmax)
            {
                movingToSecondPoint = false;
                //come back to point 1
            }
        }

        if (!movingToSecondPoint)
        {
            _cm.Movement(transform, Vector3.left, _speed);

            if (transform.position.x <= Xmin)
            {
                movingToSecondPoint = true;
                //come back to point 1
            }
        }
    }
}