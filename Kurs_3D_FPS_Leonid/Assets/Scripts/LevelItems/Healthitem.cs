using System;
using UnityEngine;

public class Healthitem : MonoBehaviour
{
    [SerializeField] private int _healPoints = 20;
    [SerializeField] private int _respawnSeconds = 30;
    
    private DateTime? _respawnDt;

    void Update()
    {
        if (_respawnDt.HasValue)
        {
            if (DateTime.Now >= _respawnDt.Value)
            {
                enabled = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var playerController = other.GetComponent<HeroController>();

        if (playerController != null)
        {
            if (playerController.Health < 100)
            {
                playerController.OnTakeDamageHandler(_healPoints * -1);

                _respawnDt = DateTime.Now.AddSeconds(_respawnSeconds);

                enabled = false;
            }
        }
    }
}
