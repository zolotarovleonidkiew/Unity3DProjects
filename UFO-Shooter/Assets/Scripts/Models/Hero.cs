using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    public int CurrentFloor = 1;
    public bool isAlive => Health > 0;
    public bool CanMoving => ActiveMovementRoundsAvailable > 0;

    [SerializeField] private int Health;

    /// <summary>
    /// Кількість раундів на пересування/дії
    /// </summary>
    private int ActiveMovementRoundsAvailable;

    /// <summary>
    /// Відобразити доступні клітинки для пересування.
    /// </summary>
    [SerializeField] private bool toShowAvailableMovementSquares;

    /// <summary>
    /// Кількість smallBox, яку може пройти наш герой за 1 раунд
    /// </summary>
    [SerializeField] private int MovementPointsPerRoundInternal = 5;
    public int MovementPointsPerRound => MovementPointsPerRoundInternal;

    /// <summary>
    /// Лінка на BackgroundGenerationScript (в якому є smallCubes)
    /// </summary>
    [SerializeField] private BackgroundGenerationScript groundObject;
    
    /// <summary>
    /// Текстура доступного переміщення героя, для smallBox only
    /// </summary>
    [SerializeField] private Material highlightMaterial;

    [Header("Hero Weapons")]
    [SerializeField] private List<HeroWeapon> weapons = new List<HeroWeapon>();
    private HeroWeapon activeWeapon;

    [Header("Shooting")]
    [SerializeField] private GameObject bulletPrefab; // префаб кулі
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

    public void SetBulletPrefab(GameObject _prefab)
    {
        bulletPrefab = _prefab;
    }
    public void SetStartingWeapons(List<WeaponData> startingWeapons)
    {
        weapons.Clear();
        foreach (var w in startingWeapons)
        {
            weapons.Add(new HeroWeapon(w)); // створюємо копію зі своїм боєзапасом
        }

        if (weapons.Count > 0)
            activeWeapon = weapons[0];
    }
    public void SetGroundObject(BackgroundGenerationScript script)
    {
        groundObject = script;
    }
    public void SettoShowAvailableMovementSquares()
    {
        toShowAvailableMovementSquares = true;
    }
    public void SetHighlightMaterial(Material m)
    {
        highlightMaterial = m;
    }
    #endregion

    #region Events
    private void Start()
    {
        Health = Constants.GlobalLivingConstans.MaxHealth;
        ActiveMovementRoundsAvailable = Constants.GlobalLivingConstans.MaxActionRounds;

        // Якщо SetStartingWeapons() вже викликано до Start(), то activeWeapon вже може бути встановлено.
        // На всяк випадок забезпечимо активну зброю тут:
        if (activeWeapon == null && weapons != null && weapons.Count > 0)
        {
            activeWeapon = weapons[0];
        }

        if (toShowAvailableMovementSquares && groundObject == null)
        {
            Debug.LogError("[Hero.cs] Cannot use 'toShowAvailableMovementSquares' flag due to groundObject is null!");
            return;
        }

        if (groundObject != null)
        {
            CacheGridRenderers();
            FindHeroGridCoords();

            if (toShowAvailableMovementSquares)
            {
                HighlightAvailableMoves();
            }
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
    }
    #endregion

    /// <summary>
    /// Підсвітити доступні клітинки
    /// </summary>
    public void HighlightAvailableMoves()
    {
        //temp
        if (cellRenderers == null) return;

        int w = groundObject.width;
        int l = groundObject.length;

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < l; j++)
            {
                var rend = cellRenderers[i, j];
                if (rend == null) continue;

                float dx = i - heroI;
                float dz = j - heroJ;
                float distance = Mathf.Sqrt(dx * dx + dz * dz);

                if (distance > 0f && distance <= MovementPointsPerRound)
                {
                    rend.enabled = true;
                    if (highlightMaterial != null)
                        rend.material = highlightMaterial; // опційно змінюємо матеріал підсвітки
                }
                else
                {
                    rend.enabled = false;
                }
            }
        }
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

        if (toShowAvailableMovementSquares && groundObject != null)
        {
            HighlightAvailableMoves();
        }
    }

    /// <summary>
    /// Виконати постріл в ціль
    /// </summary>
    public void ShootAt(Transform target)
    {
        if (activeWeapon == null) return;

        if (!activeWeapon.CanShoot())
        {
            Debug.Log($"{name} — {activeWeapon.data.weaponName} нема патронів!");
            return;
        }

        // Створюємо кулю
        GameObject bulletObj = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        Bullet bullet = bulletObj.GetComponent<Bullet>();
        bullet.speed = bulletSpeed;
        bullet.SetTarget(target);

        // Віднімаємо патрон
        activeWeapon.Shoot();

        //TODO : vosuals
        // Відтворюємо звук/анімацію (якщо є)
        //if (activeWeapon.fireSound != null)
        //{
        //    AudioSource.PlayClipAtPoint(activeWeapon.fireSound, transform.position);
        //}
        //if (activeWeapon.fireAnimation != null)
        //{
        //    // TODO: тут можна запустити анімацію героя
        //}

        Debug.Log($"[Hero] Стрельнув з {activeWeapon.data.weaponType} в {target.name}");
    }

    /// <summary>
    /// Повернути поточну зброю (HeroWeapon)
    /// </summary>
    public HeroWeapon GetActiveHeroWeapon()
    {
        return activeWeapon;
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
                    Debug.LogWarning("[SHOOTNG] Alien found :)");
                    ShootAt(alien.transform);
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
                var cube = groundObject.GetSmallCube(i, j);
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
                var cube = groundObject.GetSmallCube(i, j);
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
    #endregion

    /// <summary>
    /// Показати зону доступних ходів для цього героя
    /// </summary>
    public void ShowAvailableMoves()
    {
        if (toShowAvailableMovementSquares && groundObject != null)
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
        toShowAvailableMovementSquares = value;
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
}