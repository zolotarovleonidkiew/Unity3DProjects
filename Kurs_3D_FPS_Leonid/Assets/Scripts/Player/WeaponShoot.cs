using System;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Weapon shoots
/// </summary>
public class WeaponShoot : MonoBehaviour
{
    //перезарядка
    [SerializeField] public int GreneReloadTimeSeconds = 4;
    [SerializeField] public int RevilverReloadTimeSeconds = 3;
    [SerializeField] public int TommyGunReloadTimeSeconds = 0;

    //вероятность попадания
    [SerializeField] public int GrenedeHitProbability = 100;
    [SerializeField] public int RevolverHitProbability = 75;
    [SerializeField] public int TommyGunHitProbability = 60;

    //урон при попадании
    [SerializeField] public int GrenedeHits = 100;
    [SerializeField] public int RevolverHits = 75;
    [SerializeField] public int TommyGunHits = 60;

    //звуки
    [SerializeField] public AudioClip GrenadeExplosion;
    [SerializeField] public AudioClip RevolverReload;
    [SerializeField] public AudioClip RevolverShot;

    [SerializeField] public AudioClip TommyGunReload;
    [SerializeField] public AudioClip TommyGunSingleShot;
    [SerializeField] public AudioClip TommyGunLoopShots;
    [SerializeField] public AudioClip TommyGunEndLoopShots;

    [SerializeField] public GameObject _blood;

    private InventoryController _inventory;
    private AI_BotSeePlayer _aimController;
    private HeroController _targetHealthController;

    //время, когда оружие снова будет готово после перезарядки
    private DateTime? _dtGrenadeReloaded = null;
    private DateTime? _dtRevolverReloaded = null;
    private DateTime? _dtTommyGunReloaded = null;

    private Slot _currentWeaponSlot;

    private Animator _animator;

    private bool _isAI;

    void Start()
    {
        _inventory = GetComponent<InventoryController>();
        _aimController = GetComponent<AI_BotSeePlayer>();
        _targetHealthController = _aimController.target.GetComponent<HeroController>();
        _isAI = GetComponent<AI_Navigation>() != null; 
    }

    void Update()
    {
        //Стрелять из любого оружия
        if (Input.GetKeyDown(KeyCode.Mouse0) && !_isAI)
        {
            //проверим, что время перезарядки уже прошло
            _currentWeaponSlot = _inventory.GetCurrentWeapon();

            TryShoot();
        }

        //если мы отпустили кнопку стрелять, и стреляли мы из Пулемета, то запустим звук окончания стрельбы
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            if (_currentWeaponSlot != null && _currentWeaponSlot.ItemType == PickableItemTypes.WeaponSlot3_TommyGun)
                PlayTommyGunEndSound();
        }
    }

    private void UpdateReloadDateTime(Slot slot)
    {
        if (slot.ItemType == PickableItemTypes.WeaponSlot1_Grenade)
        {
            _dtGrenadeReloaded = DateTime.Now.AddSeconds(GreneReloadTimeSeconds);
        }
        else if (slot.ItemType == PickableItemTypes.WeaponSlop2_Revolver)
        {
            _dtRevolverReloaded = DateTime.Now.AddSeconds(RevilverReloadTimeSeconds);
        }
        else if (slot.ItemType == PickableItemTypes.WeaponSlot3_TommyGun)
        {
            _dtTommyGunReloaded = DateTime.Now.AddSeconds(TommyGunReloadTimeSeconds);
        }
    }

    private bool CheckReloadTimeExpired(Slot slot)
    {
        DateTime? dt = null;

        if (slot.ItemType == PickableItemTypes.WeaponSlot1_Grenade)
        {
            dt = _dtGrenadeReloaded;
        }
        else if (slot.ItemType == PickableItemTypes.WeaponSlop2_Revolver)
        {
            dt = _dtRevolverReloaded;
        }
        else if (slot.ItemType == PickableItemTypes.WeaponSlot3_TommyGun)
        {
            dt = _dtTommyGunReloaded;
        }

        if (dt == null)
            return true;
        else if (DateTime.Now >= dt)
            return true;
        else
            return false;

    }

    public void TryShoot()
    {
        _currentWeaponSlot = _inventory.GetCurrentWeapon();

        if (_currentWeaponSlot != null)
        {
            if (CheckReloadTimeExpired(_currentWeaponSlot)) // проверка не Релоад тайм
            {

                /*
                 Все проверки:
                    1. у игрока есть пушка и в ней есть патроны
                    2. время перезарядки этой пушки уже прошло
                 */

                if (_currentWeaponSlot.GetSlotActive() || _currentWeaponSlot.Ammo > 0)
                {
                     var animator = _inventory.PlayerWeaponPlaceHolder.transform.GetChild(1).GetComponent<Animator>();
                    //_animator = GetComponent<Animator>();
                    //GetComponentInChildren<Animator>();

                    //_animator.SetBool("RevolverShooted", true);
                    animator.Play("Base Layer.SHOOT");

                    Shoot(_currentWeaponSlot, _animator);

                    _inventory.ProcessShot(_currentWeaponSlot);

                   // Debug.Log($" {_currentWeaponSlot.Ammo} left.");

                    UpdateReloadDateTime(_currentWeaponSlot); // UPDATE DT
                }
                else
                {
                    Debug.LogError("Can't shoot without weapon or ammo");
                }
            }
        }
        else
        {
            Debug.LogError("Can't shoot without weapon");
        }
    }

    private void Shoot(Slot weapon, Animator animator)
    {
        _animator?.SetBool("RevolverShooted", true);

        if (AimAndShoot()) //прицелиться и выстрелить
        {
            //отнять жизни у противника при поадании
            DecreaseHealthPointsInOpponent();
        }

        //звук после выстрела
        GetSXPSoundAndPlay(weapon.ItemType);
    }

    #region SFX Sounds
    private void GetSXPSoundAndPlay(PickableItemTypes type)
    {
        if (type == PickableItemTypes.WeaponSlot1_Grenade)
        {
            AudioSource.PlayClipAtPoint(GrenadeExplosion, transform.position);
        }
        else if (type == PickableItemTypes.WeaponSlop2_Revolver)
        {
            AudioSource.PlayClipAtPoint(RevolverShot, transform.position);
        }
        else
        {
            AudioSource.PlayClipAtPoint(TommyGunLoopShots, transform.position);
        }
    }

    private void PlayTommyGunEndSound()
    {
        AudioSource.PlayClipAtPoint(TommyGunEndLoopShots, transform.position);
    }
    #endregion

    #region Shooting calculation

    private bool AimAndShoot()
    {
        // AI целится
        if (_isAI && _aimController.TARGET_AIMED)
        {
            var p = Random.Range(0, 100);

            int maxHitProbability = GrenedeHitProbability;

            if (_currentWeaponSlot.ItemType == PickableItemTypes.WeaponSlop2_Revolver)
            {
                maxHitProbability = RevolverHitProbability;
            }
            else if (_currentWeaponSlot.ItemType == PickableItemTypes.WeaponSlot3_TommyGun)
            {
                maxHitProbability = TommyGunHitProbability;
            }

            Debug.Log($"Aim and shoot: { p <= maxHitProbability} ");

            return p <= maxHitProbability;
        }
        else
        {
            //через рейкаст посомтрим в кого мы попали, без учета вероятностей попадания

            return PlayetShootHandler();
        }
    }

    private Ray _ray;
    private RaycastHit _raycastHit;

    private void CreateShootImpact(Vector3 position)
    {
        Instantiate(_blood, position, Quaternion.identity);

        Debug.LogWarning("SHOOTED !!!");
    }

    private bool PlayetShootHandler()
    {
       // DebugRaycastLayers();

        var mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.transform.position.z;

        Debug.DrawLine(transform.position, mousePosition, Color.green);

        _ray = Camera.main.ScreenPointToRay(mousePosition);

        Debug.DrawLine(_ray.origin, _ray.direction, Color.red); //DEBUG

        int layerMask = 1 << 8;

        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"Мы стрельнули в : {_raycastHit.collider.name}");

            CreateShootImpact(_raycastHit.point);

            return true;
        }

        Debug.Log($"Мы стрельнули в никуда :(");

        return false;
    }

    /// <summary>
    /// DEBUG
    /// </summary>
    private void DebugRaycastLayers()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int layerMask = 1 << 12;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 12] Мы стрельнули в : {_raycastHit.collider.name}");
        }
         layerMask = 1 << 11;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 11] Мы стрельнули в : {_raycastHit.collider.name}");
        }
         layerMask = 1 << 10;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 10] Мы стрельнули в : {_raycastHit.collider.name}");
        }
         layerMask = 1 << 9;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 9] Мы стрельнули в : {_raycastHit.collider.name}");
        }
         layerMask = 1 << 8;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 8] Мы стрельнули в : {_raycastHit.collider.name}");
        }

         layerMask = 1 << 7;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 7] Мы стрельнули в : {_raycastHit.collider.name}");
        }

        layerMask = 1 << 6;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 6] Мы стрельнули в : {_raycastHit.collider.name}");
        }
        layerMask = 1 << 5;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 5] Мы стрельнули в : {_raycastHit.collider.name}");
        }
        layerMask = 1 << 4;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 4] Мы стрельнули в : {_raycastHit.collider.name}");
        }
        layerMask = 1 << 3;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 3] Мы стрельнули в : {_raycastHit.collider.name}");
        }
        layerMask = 1 << 2;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 2] Мы стрельнули в : {_raycastHit.collider.name}");
        }
        layerMask = 1 << 1;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 1] Мы стрельнули в : {_raycastHit.collider.name}");
        }
        layerMask = 1 << 0;
        if (Physics.Raycast(_ray, out _raycastHit, 10000f, layerMask))
        {
            Debug.Log($"[1 << 0] Мы стрельнули в : {_raycastHit.collider.name}");
        }
    }

    private void DecreaseHealthPointsInOpponent()
    {
        int hits = GrenedeHits;

        if (_currentWeaponSlot.ItemType == PickableItemTypes.WeaponSlop2_Revolver)
        {
            hits = RevolverHits;
        }
        else if (_currentWeaponSlot.ItemType == PickableItemTypes.WeaponSlot3_TommyGun)
        {
            hits = TommyGunHits;
        }

        bool needToUpdateHUD = false;

        if (_targetHealthController.gameObject.tag == "Player")
        {
            needToUpdateHUD = true;
        }

        _targetHealthController.OnTakeDamageHandler(hits, needToUpdateHUD);
    }

    #endregion

    /// <summary>
    /// Используется в анимации префаба
    /// </summary>
    public void StopShootAnimation()
    {
        Debug.LogWarning("StopShootAnimation() called");
        _animator?.SetBool("RevolverShooted", false);
    }
}
