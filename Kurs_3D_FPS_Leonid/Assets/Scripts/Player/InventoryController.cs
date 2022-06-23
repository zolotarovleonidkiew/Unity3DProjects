using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private bool _isBot;
    public bool IsBot => _isBot;

    /// <summary>
    /// Место, откуда у игрока растут руки
    /// </summary>
    [SerializeField] private GameObject _playerWeaponPlaceHolder;
    [SerializeField] private GameObject _targetPosition;

    /// <summary>
    /// Prefab - руки с револьвером, ТГ и гранатой
    /// </summary>
    [SerializeField] private GameObject _handsWithPistole;
    [SerializeField] private GameObject _handsWithTommyGun;
    [SerializeField] private GameObject _handsWithGrenade;

    public delegate void PickupItem(PickableItemTypes type, int ammo);
    public event PickupItem OnPickupItem;

    /// <summary>
    /// Weapon slots
    /// </summary>
    public Slot Slot1 { get; set; }
    public Slot Slot2 { get; set; }
    public Slot Slot3 { get; set; }

   // int? _currentWeaponIndex = null;
    bool _handsWithWeaponDisplayed = false;

    private void Awake()
    {
        Slot1 = new Slot(0);
        Slot2 = new Slot(1);
        Slot3 = new Slot(2);
    }

    void Start()
    {
        OnPickupItem += OnPickupItemHandler;
    }

    void Update()
    {
        //Select weapon
        if (Input.GetKey(KeyCode.Alpha1) && Slot1.isActive)
        {
            ChangeSelectedWeapon(_handsWithGrenade);
        }
        else if (Input.GetKey(KeyCode.Alpha2) && Slot2.isActive)
        {
            ChangeSelectedWeapon(_handsWithPistole);
        }
        else if (Input.GetKey(KeyCode.Alpha3) && Slot3.isActive)
        {
            ChangeSelectedWeapon(_handsWithTommyGun);
        }
    }

    /// <summary>
    /// Игрок поднимает пушку
    /// </summary>
    public void OnPickupItemHandler(PickableItemTypes type, int ammo)
    {
        Slot slot;

        if (type == PickableItemTypes.WeaponSlot1_Grenade && !Slot1.isActive)
        {
            slot = Slot1;

            DisplayHands(_handsWithGrenade);

            if (_isBot)
                Debug.LogWarning("[AI]: Grenade hands appeared");
            else
                Debug.Log("Grenade hands appeared");

        }
        else if (type == PickableItemTypes.WeaponSlop2_Pistol && !Slot2.isActive)
        {
            slot = Slot2;

            DisplayHands(_handsWithPistole);

            if (_isBot)
                Debug.LogWarning("[AI]: Revolver hands appeared");
            else
                Debug.Log("Revolver hands appeared");
        }
        else //Tommy Gun
        {
            slot = Slot3;

            if (!Slot3.isActive)
            {
                DisplayHands(_handsWithTommyGun);

                if (_isBot)
                    Debug.LogWarning("[AI]: TommyGun hands appeared");
                else
                    Debug.Log("TommyGun hands appeared");
            }
        }

        slot.SetSlotActive();
        slot.AddAmmo(ammo);
    }

    private void DisplayHands(GameObject handObject)
    {
        if (!_handsWithWeaponDisplayed)
        {
            _handsWithWeaponDisplayed = true;

            var lookPos = transform.position - _targetPosition.transform.position;
            var rotation = Quaternion.LookRotation(lookPos);

           // _playerWeaponPlaceHolder =
           Instantiate(handObject, _playerWeaponPlaceHolder.transform.position, rotation,
                parent: _playerWeaponPlaceHolder.transform);
        }
    }

    public void ChangeSelectedWeapon(GameObject handsWithWeaponObject)
    {
        var goToDestroy = _playerWeaponPlaceHolder.transform.GetChild(1).gameObject;

        Destroy(goToDestroy);

        _handsWithWeaponDisplayed = false;

        DisplayHands(handsWithWeaponObject);
    }
}
