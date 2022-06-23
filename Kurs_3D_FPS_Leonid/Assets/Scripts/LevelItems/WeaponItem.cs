using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : MonoBehaviour
{
    [SerializeField] private PickableItemTypes _slotType;
    [SerializeField] private int _ammoOnStart;
    [SerializeField] private int _respawnSeconds = 30;

    public PickableItemTypes PickableItemTypes => _slotType;
    public int AmmoOnStart => _ammoOnStart;

    private DateTime? _respawnDt;
    private Collider _triggerCollider;

    private void Awake()
    {
        _triggerCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (_respawnDt.HasValue)
        {
            if (DateTime.Now >= _respawnDt.Value)
            {
                enabled = true;
                _triggerCollider.enabled = true;
            }
        }
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

            _respawnDt = DateTime.Now.AddSeconds(_respawnSeconds);

            _triggerCollider.enabled = false;

            enabled = false;
        }
    }
}
