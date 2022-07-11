using UnityEngine;

public class PrefabWithAnimation : MonoBehaviour
{
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKey(KeyCode.Backspace))
        {
            _animator.SetBool("RevolverShooted", true);
        }

    }

    /// <summary>
    /// Используется в анимации префаба
    /// </summary>
    public void StopShootAnimation()
    {

        Debug.LogWarning("StopShootAnimation() called");
        _animator.SetBool("RevolverShooted", false);
    }
}
