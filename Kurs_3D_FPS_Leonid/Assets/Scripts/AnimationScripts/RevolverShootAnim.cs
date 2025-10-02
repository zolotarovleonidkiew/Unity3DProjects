using UnityEngine;

/// <summary>
/// Отдача рвольвера
/// </summary>
public class RevolverShootAnim : MonoBehaviour
{
    public WeaponToCenterScreen reference_wct;

    [SerializeField] private float _speed = 90;
    [SerializeField] private int _limitRotationGradus = 6;

    private Vector3 directionUp = new Vector3(1, 0, 0);
    private Vector3 directionDown = new Vector3(-1, 0, 0);

    private bool _startAnimation;
    private bool _startAnimationStep2;

    private int _currentGradus = 0;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            _startAnimation = true;
        }

        if (_startAnimation)
        {
            if (_currentGradus >= _limitRotationGradus && !_startAnimationStep2)
            {
                _startAnimationStep2 = true;
                _currentGradus = 0;
            }
            else if (_currentGradus >= _limitRotationGradus && _startAnimationStep2)
            {
                _startAnimation = false;
                _startAnimationStep2 = false;
                _currentGradus = 0;

                reference_wct = transform.GetComponent<WeaponToCenterScreen>();

                reference_wct.StopPointingToCenter = false;
            }
        }
    }

    private void FixedUpdate()
    {
        if (_startAnimation)
        {
            Vector3 direction = directionUp;

            if (_startAnimationStep2)
            {
                direction = directionDown;
            }

            _currentGradus++;

            transform.Rotate(direction * Time.deltaTime * _speed);
        }
    }
}