using UnityEngine;

public class InventoryController : MonoBehaviour
{
    /// <summary>
    /// ссылка на  HUD
    /// </summary>
    [SerializeField] private HUDMenu _hud;

    [SerializeField] private bool _isBot;
    public bool IsBot => _isBot;

    /// <summary>
    /// Место, откуда у игрока растут руки
    /// </summary>
    [SerializeField] private GameObject _playerWeaponPlaceHolder;
    [SerializeField] private GameObject _targetPosition;

    public GameObject PlayerWeaponPlaceHolder => _playerWeaponPlaceHolder;

    /// <summary>
    /// Prefab - руки с револьвером, ТГ и гранатой
    /// </summary>
    [SerializeField] private GameObject _handsWithPistole;
    [SerializeField] private GameObject _handsWithTommyGun;
    [SerializeField] private GameObject _handsWithGrenade; // TODO: сделать руку с гранатой

    public delegate void PickupItem(PickableItemTypes type, int ammo);
    public event PickupItem OnPickupItem;

    /// <summary>
    /// Weapon slots
    /// </summary>
    public Slot Slot1 { get; set; }
    public Slot Slot2 { get; set; }
    public Slot Slot3 { get; set; }

    int? _currentWeaponIndex = null;
    bool _handsWithWeaponDisplayed = false;

    private void Awake()
    {
        Slot1 = new Slot(0, PickableItemTypes.WeaponSlot1_Grenade);
        Slot2 = new Slot(1, PickableItemTypes.WeaponSlop2_Revolver);
        Slot3 = new Slot(2, PickableItemTypes.WeaponSlot3_TommyGun);
    }

    void Start()
    {
        OnPickupItem += OnPickupItemHandler;
    }

    /// <summary>
    /// Выбор оружия
    /// </summary>
    void Update()
    {
        //Select weapon
        if (Input.GetKey(KeyCode.Alpha1) && Slot1 != null && Slot1.isActive)
        {
            ChangeSelectedWeapon(_handsWithGrenade);
            _currentWeaponIndex = 0;

            UpdateHUD(Slot1);
        }
        else if (Input.GetKey(KeyCode.Alpha2) && Slot2 != null && Slot2.isActive)
        {
            ChangeSelectedWeapon(_handsWithPistole);
            _currentWeaponIndex = 1;

            UpdateHUD(Slot2);
        }
        else if (Input.GetKey(KeyCode.Alpha3) && Slot3 != null && Slot3.isActive)
        {
            ChangeSelectedWeapon(_handsWithTommyGun);
            _currentWeaponIndex = 2;

            UpdateHUD(Slot3);
        }
    }

    /// <summary>
    /// Игрок поднимает пушку
    /// </summary>
    public void OnPickupItemHandler(PickableItemTypes type, int ammo)
    {
        Slot slot = default;

        if (type == PickableItemTypes.WeaponSlot1_Grenade && !Slot1.isActive)
        {
            slot = Slot1;

            if (_currentWeaponIndex == null)
            {
                _currentWeaponIndex = 0;
            }

            DisplayHands(_handsWithGrenade);

            if (_isBot)
                Debug.LogWarning("[AI]: Grenade hands appeared");
            else
                Debug.Log("Grenade hands appeared");

        }
        else if (type == PickableItemTypes.WeaponSlop2_Revolver && !Slot2.isActive)
        {
            slot = Slot2;

            if (_currentWeaponIndex == null)
            {
                _currentWeaponIndex = 1;
            }

            DisplayHands(_handsWithPistole);

            if (_isBot)
                Debug.LogWarning("[AI]: Revolver hands appeared");
            else
                Debug.Log("Revolver hands appeared");
        }
        else if (type == PickableItemTypes.WeaponSlot3_TommyGun && !Slot3.isActive)
        {
            slot = Slot3;

            if (_currentWeaponIndex == null)
            {
                _currentWeaponIndex = 2;
            }

            if (!Slot3.isActive)
            {
                DisplayHands(_handsWithTommyGun);

                if (_isBot)
                    Debug.LogWarning("[AI]: TommyGun hands appeared");
                else
                    Debug.Log("TommyGun hands appeared");
            }
        }
        else
        {
            // слот уже активирован, просто добавить патроны

            //update ammo
            if (type == PickableItemTypes.WeaponSlot1_Grenade)
            {
                Slot1?.AddAmmo(1);
            }
            else if (type == PickableItemTypes.WeaponSlop2_Revolver)
            {
                Slot2?.AddAmmo(6);
            }
            else
            {
                Slot3?.AddAmmo(30);
            }
        }
        
        slot?.SetSlotActive();
        slot?.AddAmmo(ammo);

        //update strategy if it's a BOT
        if (_isBot)
        {
            var aiNavigation = GetComponent<AI_Navigation>();

            aiNavigation.NeedCheckStrategy = true;
        }
        else
        {
            UpdateHUD();
        }

    }

    private void DisplayHands(GameObject handObject)
    {
        if (!_handsWithWeaponDisplayed)
        {
            _handsWithWeaponDisplayed = true;

            var lookPos = transform.position - _targetPosition.transform.position;
            var rotation = Quaternion.LookRotation(lookPos);

            var hands = Instantiate(handObject, _playerWeaponPlaceHolder.transform.position, rotation,
                 parent: _playerWeaponPlaceHolder.transform);

            hands.AddComponent<Animator>(); //!!!

            hands.AddComponent<WeaponToCenterScreen>();
        }
    }

    public void ChangeSelectedWeapon(GameObject handsWithWeaponObject)
    {
        var goToDestroy = _playerWeaponPlaceHolder.transform.GetChild(1).gameObject;

        Destroy(goToDestroy);

        _handsWithWeaponDisplayed = false;

        DisplayHands(handsWithWeaponObject);
    }

    public void ChangeSelectedWeapon(int index)
    {
        if (_currentWeaponIndex != null)
        {
            GameObject go = default;

            if (index == 0)
            {
                go = _handsWithGrenade;
            }
            else if (index == 1)
            {
                go = _handsWithPistole;
            }
            else if (index == 2)
            {
                go = _handsWithTommyGun;
            }

            ChangeSelectedWeapon(go);

            UpdateHUD();
        }
    }

    /// <summary>
    /// Определяет, достаточно ли у бота вооружения, чтобы идти воевать
    /// </summary>
    public bool BotReadyToKill()
    {
        //граната
        //if (Slot1 != null && Slot1.GetSlotActive() && Slot1.ammo > 0)
        //{
        //    return true;
        //}
        if (Slot2 != null && Slot2.GetSlotActive() && Slot2.Ammo > 10)
        {
            return true;
        }
        else if (Slot3 != null && Slot3.GetSlotActive() && Slot3.Ammo > 10)
        {
            return true;
        }

        return false;
    }

    public Slot GetCurrentWeapon()
    {
        if (_currentWeaponIndex.HasValue)
        {
            int i = _currentWeaponIndex.Value;

            if (i == 0)
                return Slot1;
            else if (i == 1)
                return Slot2;
            if (i == 2)
                return Slot3;
        }

        return null;
    }

    #region HUD
    private void UpdateHUD(Slot weaponSlot)
    {
        if (gameObject.tag == "Player")
        {
            _hud.UpdateAmmo(weaponSlot.Ammo);
        }
    }

    private void UpdateHUD()
    {
        if (_currentWeaponIndex.HasValue)
        {
            if (_currentWeaponIndex.Value == 0)
            {
                UpdateHUD(Slot1);
            }
            if (_currentWeaponIndex.Value == 1)
            {
                UpdateHUD(Slot2);
            }
            if (_currentWeaponIndex.Value ==2)
            {
                UpdateHUD(Slot3);
            }
        }
    }

    public void ProcessShot(Slot slot, int val = 1)
    {
        slot.AddAmmo(-val);

        UpdateHUD(slot);
    }

    #endregion
}
