using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public void Movement(Transform transform, Vector3 direction, float speed)
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
