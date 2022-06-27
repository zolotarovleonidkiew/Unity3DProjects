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
                bool needToUpdateHUD = false;
                if (playerController.gameObject.tag == "Player")
                {
                    needToUpdateHUD = true;
                }

                playerController.OnTakeDamageHandler(_healPoints * -1, needToUpdateHUD);

                _respawnDt = DateTime.Now.AddSeconds(_respawnSeconds);

                enabled = false;
            }
        }
    }
}
