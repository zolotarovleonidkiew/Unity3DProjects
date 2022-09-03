using UnityEngine;

/// <summary>
/// Двигает вверх/вниз объект с определнной скоростью
/// </summary>
public class UpDownMover : MonoBehaviour
{
    [SerializeField] private float GoUpValue;
    [SerializeField] private float GoDownValue;
    [SerializeField] private float Speed;

    private Vector3 _startposition;
    private Vector3 _maxPosition, _minPosition;
    private bool _goUp;

    private void Start()
    {
        _goUp = true;

        _startposition = transform.position;

        _maxPosition = new Vector3
        {
            x = _startposition.x,
            y = _startposition.y + GoUpValue,
            z = _startposition.z
        };
        _minPosition = new Vector3
        {
            x = _startposition.x,
            y = _startposition.y - GoDownValue,
            z = _startposition.z
        };
    }

    private void FixedUpdate()
    {
        CheckDirection();

        var targetPosition = _goUp ? _maxPosition : _minPosition;

        var direction = targetPosition - transform.position;
        direction = direction.normalized;

        transform.position += direction * Speed * Time.deltaTime;
    }

    private void CheckDirection()
    {
        float currY = transform.position.y;
        float maxY = _maxPosition.y;
        float minY = _minPosition.y;

        if (Mathf.Approximately(currY, maxY) && _goUp)
        {
            _goUp = false;
        }
        else if (Mathf.Approximately(currY, minY) && !_goUp)
        {
            _goUp = true;
        }
    }
}
