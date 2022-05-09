using UnityEngine;

/// <summary>
/// Отслежтивание приземления Улрика
/// </summary>
public class UserGroundCheck : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _animator.SetBool("IsJumping", false);
    }
}