using System;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public int CurrentFloor = 1;
    public bool isAlive => CurentHealth > 0;
    public bool CanMoving => ActiveMovementRoundsAvailable > 0;

    #region Hero data
    public int MaxHealth;
    public int CurentHealth;

    public float Speed;
    public float DetectionRange;
    public float AttackCooldown = 1f;
    public int GrenadeCount { get; private set; }
    #endregion


    /// <summary>
    /// Кількість раундів на пересування/дії
    /// </summary>
    private int ActiveMovementRoundsAvailable;

    /// <summary>
    /// Відобразити доступні клітинки для пересування.
    /// </summary>
    [SerializeField] private bool ShowAvailableMovementSquares;

    /// <summary>
    /// Кількість smallBox, яку може пройти наш герой за 1 раунд
    /// </summary>
    [SerializeField] private int MovementPointsPerRoundInternal = 5;
    public int MovementPointsPerRound => MovementPointsPerRoundInternal;

    /// <summary>
    /// Лінка на BackgroundGenerationScript (в якому є smallCubes)
    /// </summary>
    [SerializeField] private BackgroundGenerationScript groundObject; //TO DO : убрать линк на скрипт
    
    /// <summary>
    /// Текстура доступного переміщення героя, для smallBox only
    /// </summary>
    [SerializeField] private Material highlightMaterial;

    [Header("Hero Weapons")]
    [SerializeField] private List<HeroWeapon> weapons = new List<HeroWeapon>();
    private HeroWeapon activeWeapon;
    public IReadOnlyList<HeroWeapon> Weapons => weapons;

    [Header("Shooting")]
    [SerializeField] private float bulletSpeed = 15f; // швидкість кулі
    private Transform shootPoint;

    /// <summary>
    /// Герой їде на ліфті (зміннює поверх)
    /// </summary>
    [SerializeField] private bool heroChangingFloor = false;
    public void ChangeHeroChangingFloor(bool val, int newFloor)
    {
        heroChangingFloor = val;
        CurrentFloor = newFloor;
    }
    public bool GeyPlayerChangingFloor()
    {
        return heroChangingFloor;
    }

    #region Misc. & Pre-set
    private Renderer[,] cellRenderers; // для швидкого доступу   
    private int heroI;
    private int heroJ;

    public void SetStartingWeapons(List<WeaponData> startingWeapons)
    {
        weapons.Clear();
        if (startingWeapons == null)
            return;

        foreach (var w in startingWeapons)
        {
            weapons.Add(new HeroWeapon(w)); // створюємо копію зі своїм боєзапасом
        }

        if (weapons.Count > 0)
            activeWeapon = weapons[0];
    }
    public void SetGrenadeCount(int grenadeCount)
    {
        GrenadeCount = grenadeCount;//Mathf.Clamp(grenadeCount, 0, 3);//?
    }
    public void SetGroundObject(BackgroundGenerationScript script)
    {
        groundObject = script;
    }
    public void SetFlag_ShowAvailableMovementSquares()
    {
        ShowAvailableMovementSquares = true;
    }
    public void SetHighlightMaterial(Material m)
    {
        highlightMaterial = m;
    }
    #endregion

    //services:
    private readonly HighlightAvailableMovesService _highlightAvailableMovesService = new HighlightAvailableMovesService();

    #region Events
    private void Start()
    {
        ActiveMovementRoundsAvailable = Constants.GlobalLivingConstans.MaxActionRounds;

        // Якщо SetStartingWeapons() вже викликано до Start(), то activeWeapon вже може бути встановлено.
        // На всяк випадок забезпечимо активну зброю тут:
        if (activeWeapon == null && weapons != null && weapons.Count > 0)
        {
            activeWeapon = weapons[0];
        }

        if (ShowAvailableMovementSquares && groundObject == null)
        {
            Debug.LogError("[Hero.cs] Cannot use 'toShowAvailableMovementSquares' flag due to groundObject is null!");
            return;
        }

        if (groundObject != null)
        {
            CacheGridRenderers();
            FindHeroGridCoords();

        }
    }

    private void Update()
    {
        if (!IsActiveHero()) return;

        if (Input.GetMouseButtonDown(0))
        {
            ShootAlien();
        }
        // Перемикання зброї на цифри
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(WeaponType.Pistol);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(WeaponType.Rifle);
        if (Input.GetKeyDown(KeyCode.R))
            ReloadActiveWeapon();
    }
    #endregion

    /// <summary>
    /// Підсвітити доступні клітинки
    /// </summary>
    public void HighlightAvailableMoves()
    {
        // Ensure we have cached grid renderers (lazy init if needed)
        if (groundObject == null) return;
        EnsureGridCached();

        //temp
        if (cellRenderers == null) return;

        _highlightAvailableMovesService.HighlightAvailableMoves(
            groundObject,
            cellRenderers,
            new Vector2Int(heroI, heroJ),
            MovementPointsPerRound,
            highlightMaterial,
            StaticTacticalData.GroundHierarchy,
            (GroundHierarchyLevel)CurrentFloor);
    }

    /// <summary>
    /// Координати героя
    /// </summary>
    public Vector2Int GetHeroCoords()
    {
        return new Vector2Int(heroI, heroJ);
    }

    /// <summary>
    /// Оновлення координат героя після руху
    /// </summary>
    public void UpdateHeroCoords(int i, int j)
    {
        heroI = i;
        heroJ = j;
    }

    /// <summary>
    /// Прибрати підсвітку руху
    /// </summary>
    public void ClearHighlights()
    {
        if (groundObject == null) return;
        EnsureGridCached();

        //temp
        if (cellRenderers == null) return;

        int w = groundObject.width;
        int l = groundObject.length;

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < l; j++)
            {
                var rend = cellRenderers[i, j];
                if (rend != null)
                {
                    rend.enabled = false;
                }
            }
        }
    }

    /// <summary>
    /// Використати 1 хід
    /// </summary>
    public void UseActionPoint()
    {
        if (ActiveMovementRoundsAvailable > 0)
            ActiveMovementRoundsAvailable--;
    }

    /// <summary>
    /// Призначити кількість ходів герою
    /// </summary>
    public void SetMovementPoints(int points)
    {
        ActiveMovementRoundsAvailable = points;

        if (ShowAvailableMovementSquares && groundObject != null)
        {
            HighlightAvailableMoves();
        }
    }

    /// <summary>
    /// Повернути поточну зброю (HeroWeapon)
    /// </summary>
    public HeroWeapon GetActiveHeroWeapon()
    {
        return activeWeapon;
    }

    public int GetActiveWeaponIndex()
    {
        return activeWeapon == null || weapons == null ? -1 : weapons.IndexOf(activeWeapon);
    }

    /// <summary>
    /// Змінити зброю
    /// </summary>
    public void SwitchWeapon(WeaponType type)
    {
        if (weapons == null || weapons.Count == 0) return;
        var heroWeapon = weapons.Find(x => x.data != null && x.data.weaponType == type);

        if (heroWeapon != null)
        {
            activeWeapon = heroWeapon;
            Debug.Log($"[Hero] {name} switched to {type}");
        }
        else
        {
            Debug.LogWarning($"[Hero] {name} doesn't have weapon {type}");
        }
    }

    public void SelectWeapon(int weaponIndex)
    {
        if (weapons == null || weaponIndex < 0 || weaponIndex >= weapons.Count)
            return;

        HeroWeapon selectedWeapon = weapons[weaponIndex];
        if (selectedWeapon?.data == null)
            return;

        activeWeapon = selectedWeapon;
        Debug.Log($"Выбрано активным оружие {selectedWeapon.data.weaponName}");
    }

    private void ReloadActiveWeapon()
    {
        activeWeapon?.Reload();
    }

    private void ShootAlien()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            GameObject clicked = hit.collider.gameObject;

            // перевірка: клітинка з тегом FloorGrid
            if (clicked.CompareTag(Constants.TagConstans.AlienTag))
            {
                var alien = clicked.transform;

                if (alien != null)
                {
                    // prefer the child's TargetShootingPoint if present
                    var targetPoint = alien.Find("TargetShootingPoint");
                    if (targetPoint != null)
                    {
                        Debug.LogWarning("[SHOOTNG] Alien found, targeting TargetShootingPoint :)");
                        ShootAt(targetPoint);
                    }
                    else
                    {
                        Debug.LogWarning("[SHOOTNG] TargetShootingPoint not found, targeting alien transform :)");
                        ShootAt(alien);
                    }

                    //Shooting end hero's turn
                    ActiveMovementRoundsAvailable = 0;
                }
                else
                {
                    Debug.LogWarning("[SHOOTNG] Alien not found :(");
                }
            }
        }
    }

    #region ######################################  Supplement

    /// <summary>
    /// Зберігаємо Renderer-и маленьких кубиків
    /// </summary>
    private void CacheGridRenderers()
    {
        int w = groundObject.width;
        int l = groundObject.length;

        cellRenderers = new Renderer[w, l];

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < l; j++)
            {
                var cube = GridGenerator_05.GetSmallCube(i, j);
                if (cube != null)
                {
                    var rend = cube.GetComponent<Renderer>();
                    cellRenderers[i, j] = rend;
                    if (rend != null)
                    {
                        rend.enabled = false; // ховаємо за замовчуванням
                    }
                }
                else
                {
                    cellRenderers[i, j] = null;
                }
            }
        }
    }

    /// <summary>
    /// Ensure grid renderers and hero coords are available (lazy init)
    /// </summary>
    private void EnsureGridCached()
    {
        if (cellRenderers != null) return;
        if (groundObject == null) return;

        CacheGridRenderers();
        FindHeroGridCoords();
    }

    /// <summary>
    /// Визначаємо координати героя в матриці
    /// </summary>
    private void FindHeroGridCoords()
    {
        Vector3 heroPos = transform.position;

        float minDist = float.MaxValue;

        for (int i = 0; i < groundObject.width; i++)
        {
            for (int j = 0; j < groundObject.length; j++)
            {
                // Use overload that searches registered layers so we find the actual small box instance
                var cube = GridGenerator_05.GetSmallCube(i, j);
                if (cube == null) continue;

                float dist = Vector3.Distance(heroPos, cube.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    heroI = i;
                    heroJ = j;
                }
            }
        }
    }

    /// <summary>
    /// Призначити GO, звідки летять кулі
    /// </summary>
    public void SetShootPoint(Transform sp)
    {
        shootPoint = sp;
    }

    /// <summary>
    /// Повернути  GO, звідки вилітають кулі
    /// </summary>
    public Transform GetShootPoint()
    {
        return shootPoint;
    }

    /// <summary>
    /// Виконати постріл в ціль
    /// Повернутися до цілі та створити префаб кулі у FirePoint (або shootPoint як запасний варіант)
    /// </summary>
    public void ShootAt(Transform target)
    {
        if (activeWeapon == null) return;

        if (!activeWeapon.CanShoot())
        {
            Debug.Log($"{name} — {activeWeapon.data?.weaponName} нема патронів!");
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("ShootAt: target is null");
            return;
        }

        // Rotate instantly to face target on horizontal plane
        Vector3 lookDir = target.position - transform.position;
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(lookDir);

        // Prefer child named "FirePoint" on the hero prefab
        Transform firePoint = transform.Find("FirePoint");
        if (firePoint == null && shootPoint != null)
            firePoint = shootPoint;
        if (firePoint == null)
            firePoint = transform;

        GameObject bulletPrefab = activeWeapon.data?.BulltPrefab;
        if (bulletPrefab == null)
        {
            Debug.LogWarning($"ShootAt: bullet prefab is not assigned for weapon '{activeWeapon.data?.weaponName}'");
            return;
        }

        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        if (bullet == null)
        {
            Debug.LogWarning("ShootAt: bulletPrefab does not contain a Bullet component");
            Destroy(bulletObj);
            return;
        }

        bullet.speed = bulletSpeed;
        bullet.SetShotData(gameObject, activeWeapon.data);
        bullet.SetTarget(target);
        activeWeapon.Shoot();

        //TODO : visuals
        // Відтворюємо звук/анімацію (якщо є)
        //if (activeWeapon.fireSound != null)
        //{
        //    AudioSource.PlayClipAtPoint(activeWeapon.fireSound, transform.position);
        //}
        //if (activeWeapon.fireAnimation != null)
        //{
        //    // TODO: тут можна запустити анімацію героя
        //}

        //Debug.Log($"[Hero] Стрельнув з {activeWeapon.data.weaponType} в {target.name}");
    }
    #endregion

    /// <summary>
    /// Показати зону доступних ходів для цього героя
    /// </summary>
    public void ShowAvailableMoves()
    {
        if (ShowAvailableMovementSquares && groundObject != null)
            HighlightAvailableMoves();
    }

    /// <summary>
    /// Приховати (стерти) підсвітку цього героя
    /// </summary>
    public void HideAvailableMoves()
    {
        ClearHighlights();
    }

    /// <summary>
    /// Дозволяє вмикати/вимикати внутрішній прапорець (не обов'язково)
    /// </summary>
    public void SetShowAvailableMovementSquaresFlag(bool value)
    {
        ShowAvailableMovementSquares = value;
    }

    /// <summary>
    /// Checks smallBox cell[i;j] is available for movement
    /// </summary>
    public bool IsCellAvailable(int i, int j)
    {
        float dx = i - heroI;
        float dz = j - heroJ;
        float distance = Mathf.Sqrt(dx * dx + dz * dz);
        return (distance > 0f && distance <= MovementPointsPerRound);
    }

    private bool IsActiveHero()
    {
        return GameController.Instance != null
            && GameController.Instance.ActiveHero == this;
    }

    /// <summary>
    /// Apply damage to the hero, reducing current health. If health drops to 0 or below, the hero is considered dead.
    /// </summary>
    internal void TakeDamage(int damage)
    {
        if (damage > 0)
        {
            CurentHealth -= damage;
            if (CurentHealth <= 0)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        Debug.Log($"[Hero] {name} has died.");
    }
}