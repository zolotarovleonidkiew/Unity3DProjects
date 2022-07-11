using DG.Tweening;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    [SerializeField] private AllItemsController _itemsController;
    [SerializeField] private PickableItemTypes _slotType;
    [SerializeField] private int _ammoOnStart;

    public PickableItemTypes PickableItemTypes => _slotType;
    public int AmmoOnStart => _ammoOnStart;
    private Collider _triggerCollider;

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        var inventory = other.GetComponent<InventoryController>();

        if (inventory != null)
        {
            inventory.OnPickupItemHandler(_slotType, _ammoOnStart);

            if (inventory.IsBot)
                Debug.LogWarning($"[AI] Pick-uped {_slotType}");
            else
                Debug.Log($"Pick-uped {_slotType}");

            _triggerCollider.enabled = false;

            _itemsController.DisableItem(gameObject);

            float _timeCounter = 0;
            float _duration = _itemsController.WeaponRespawnSecons;

            DOTween.To(() => _timeCounter, x => _timeCounter = x, _duration, _duration)
             .OnComplete(() => 
             { 
                 _itemsController.EnbleItem(gameObject);
                 _triggerCollider.enabled = true;
             } )
             .SetEase(Ease.Linear);
        }
    }
}
