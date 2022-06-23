using UnityEngine;

/// <summary>
/// Rotates item, when it's on the land
/// </summary>
public class ItemRotator : MonoBehaviour
{
    [SerializeField] private float _rotationSpeed = 5;

    [SerializeField] private Vector3 direction;

    void Update()
    {
        transform.Rotate(direction * Time.deltaTime * _rotationSpeed);
    }
}
