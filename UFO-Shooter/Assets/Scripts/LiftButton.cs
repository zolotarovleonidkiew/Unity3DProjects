using UnityEngine;

public class LiftButton : MonoBehaviour
{
    public LiftPlatform lift;      // посилання на платформу
    public bool callToUpper;       // якщо true — виклик на верхній поверх, інакше вниз
    public KeyCode interactKey = KeyCode.E;

    private bool _playerInRange = false;

    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(interactKey))
        {
            if (callToUpper)
                lift.CallLiftUp();
            else
                lift.CallLiftDown();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = true;
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _playerInRange = false;
        }
            
    }
}