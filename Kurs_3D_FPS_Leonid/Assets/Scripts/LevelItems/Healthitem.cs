using DG.Tweening;
using System;
using UnityEngine;

public class Healthitem : MonoBehaviour
{
    [SerializeField] private AllItemsController _itemsController;

    [SerializeField] private int _healPoints = 20;

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

                _itemsController.DisableItem(gameObject);

                float _timeCounter = 0;
                float _duration = _itemsController.MedKitRespawnSecons;
                
                DOTween.To(() => _timeCounter, x => _timeCounter = x, _duration, _duration)
                 .OnComplete(() =>
                 {
                     _itemsController.EnbleItem(gameObject);
                 })
                 .SetEase(Ease.Linear);
            }
        }
    }
}
