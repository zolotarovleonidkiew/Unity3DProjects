using UnityEngine;

/// <summary>
/// Вращает объект вокруг своей оси
/// </summary>
public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 5;
    [SerializeField] private Vector3 direction;

    void Update()
    {
        transform.Rotate(direction * Time.deltaTime * _rotationSpeed);
    }
}
