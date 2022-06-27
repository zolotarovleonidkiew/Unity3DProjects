using UnityEngine;

/// <summary>
/// Почему нет звука ??????????????????????
/// </summary>
public class WeaponShoot : MonoBehaviour
{
    //перезарядка
    [SerializeField] public int GreneReloadTimeSeconds = 4;
    [SerializeField] public int RevilverReloadTimeSeconds = 3;
    [SerializeField] public int TommyGunTimeSeconds = 0;

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

    private InventoryController _inventory;
    private AI_BotSeePlayer _aimController;
    private HeroController _targetHealthController;

    private Slot _currentWeaponSlot;

    void Start()
    {
        _inventory = GetComponent<InventoryController>();
        _aimController = GetComponent<AI_BotSeePlayer>();
        _targetHealthController = _aimController.target.GetComponent<HeroController>();
    }

    void Update()
    {
        //Стрелять из любого оружия
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _currentWeaponSlot = _inventory.GetCurrentWeapon();

            TryShoot();
        }

        //если мы отпустили кнопку стрелять, и стреляли мы из Пулемета, то запустим звук окончания стрельбы
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_currentWeaponSlot != null && _currentWeaponSlot.ItemType == PickableItemTypes.WeaponSlot3_TommyGun)
                PlayTommyGunEndSound();
        }
    }

    public void TryShoot()
    {
        if (_currentWeaponSlot != null)
        {
            if (_currentWeaponSlot.GetSlotActive() || _currentWeaponSlot.Ammo > 0)
            {
                Shoot(_currentWeaponSlot);

                _inventory.ProcessShot(_currentWeaponSlot);

                Debug.Log($" {_currentWeaponSlot.Ammo} left.");
            }
            else
            {
                Debug.LogError("Can't shoot without weapon or ammo");
            }
        }
        else
        {
            Debug.LogError("Can't shoot without weapon");
        }
    }

    private void Shoot(Slot weapon)
    {
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
        if (_aimController.TARGET_AIMED)
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

        return false;
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
}
