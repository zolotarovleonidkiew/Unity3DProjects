using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundGenerationScript : MonoBehaviour
{
    [Header("Hero Settings")]
    public List<Vector2> heroGridPositions = new()
    {
        new Vector2(0, 0),
        new Vector2(1, 0),
        new Vector2(0, 1),
        new Vector2(1, 1)
    };
    public float playerSize = 1f;
    public Material heroBoxMaterial;
    [SerializeField] private List<WeaponData> heroStartingWeapons;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject HeroesCollectionGUI;

    [Header("Alien Settings")]
    public List<Vector2> alienGridPositions = new()
    {
        new Vector2(2, 2),
        new Vector2(3, 2),
        new Vector2(2, 3),
        new Vector2(3, 3),
        new Vector2(4, 2),
        new Vector2(4, 3)
    };
    public float alienSize = 1f;
    public Material alienBoxMaterial;
    [SerializeField] private GameObject AlienCollectionGUI;

    [Header("Misc. Settings")]
    [SerializeField] private Button _btnGenerate;
    [SerializeField] private GameController gameController;

    [Header("Obstacles (Buildings)")]
    [SerializeField] private Material Level1_ObstacleMaterial;
    [SerializeField] private Material Level2_ObstacleMaterial;
    [SerializeField] private Material Level3_ObstacleMaterial;
    [SerializeField] private List<ObstacleOnTheMap> _obstacles;    
    [SerializeField] private List<Ramp> _RampsCollection;

    [Header("Grid (counts)")]
    public int width = 25;
    public int length = 25;
    public Material groundBoxMaterial;

    [Header("Sizes")]
    public float cellSize = 2f;
    public float bigHeight = 0.5f;
    public float smallHeight = 0.5f;
    public float liftAboveBig = 0.3f;

    [Header("Lift settings")]
    [SerializeField] private Vector2Int liftGridPos = new(2, 2); // координати ліфта на bigBox
    [SerializeField] private Material liftMaterial;
    private LiftFactory liftFactory;

    public GameObject parent;
    public Material gridBoxMaterial;
    public Material gridBoxMaterialHighlited;

    private GameObject[,] smallCubes;
    private GridGenerator _gridGenerator;
    public GridGenerator GetGridGenerator => _gridGenerator;

    /// <summary>
    /// TO CLOSE CANVAS ONLY
    /// </summary>
    public Canvas canvas;
    /// <summary>
    /// Player GUI
    /// </summary>
    public Canvas canvasGUI;

    private void Start()
    {
        _btnGenerate.onClick.AddListener(() => CreateBoxAndSubBoxes(width, length));
    }

    /// <summary>
    /// MAIN EVENT
    /// </summary>
    public void CreateBoxAndSubBoxes(int cellsX, int cellsZ)
    {
        liftFactory = new LiftFactory(HeroesCollectionGUI, liftMaterial);

        //додаємо нерівності ландафту
        GenerateHillsAndClimbs();

        _gridGenerator = new GridGenerator(
            HeroesCollectionGUI,
            width, length,
            cellSize, bigHeight, smallHeight, liftAboveBig,
            gridBoxMaterial, groundBoxMaterial,
            parent.transform,
            obstacles: _obstacles,
            liftGridPos: liftGridPos,
            liftFactory: liftFactory
        );
        //---------------------

        //створюємо ландшафт
        smallCubes = _gridGenerator.GenerateGrid(_RampsCollection);

        //привязка пандусов к преградам (использ в HeroMovemnt)
        var flatternObstacles = _obstacles.FlatternNestedObstacles();
        foreach (Ramp r in _RampsCollection)
        {
            var obst = flatternObstacles[r.obstacleIndex];
            obst.RampsCollection.Add(r.GO);
        }

        //додаємо ліфт та другий поверх
        CreateLiftAndSecondFloor();

        var heroFactory = new HeroFactory(playerSize, heroBoxMaterial, heroStartingWeapons, bulletPrefab, this, HeroesCollectionGUI, gridBoxMaterialHighlited);
        var alienFactory = new AlienFactory(alienSize, alienBoxMaterial, this, AlienCollectionGUI);
        var obstacleFactory = new ObstacleFactory(cellSize, bigHeight, parent.transform, obstacles: _obstacles);

        // Герої
        for (int h = 0; h < heroGridPositions.Count; h++)
        {
            int xi = Mathf.FloorToInt(heroGridPositions[h].x);
            int zj = Mathf.FloorToInt(heroGridPositions[h].y);

            GameObject targetCell = GetSmallCube(xi, zj);
            if (targetCell == null)
            {
                // знаходимо найближчу валідну клітинку (якщо початкова була під будинком або поза межею)
                targetCell = FindNearestValidCell(xi, zj);
                if (targetCell == null)
                {
                    Debug.LogError($"[BGS] Не знайдено валідної клітинки для героя {h} (запитано {xi},{zj})");
                    continue;
                }
            }

            Hero hero = heroFactory.CreateHero(targetCell, h);
            if (hero != null)
                gameController.RegisterHero(hero);
        }

        // Чужі
        for (int a = 0; a < alienGridPositions.Count; a++)
        {
            int xi = Mathf.FloorToInt(alienGridPositions[a].x);
            int zj = Mathf.FloorToInt(alienGridPositions[a].y);

            GameObject targetCell = GetSmallCube(xi, zj);
            if (targetCell == null)
            {
                targetCell = FindNearestValidCell(xi, zj);
                if (targetCell == null)
                {
                    Debug.LogError($"[BGS] Не знайдено валідної клітинки для alien {a} (запитано {xi},{zj})");
                    continue;
                }
            }

            Alien al = alienFactory.CreateAlien(targetCell, a);
            if (al != null)
                gameController.RegisterAlien(al);
        }

        //Будинки, холми та інші перепони (Obstacles):
        obstacleFactory.CreateObstacle(width, length);

        //запуск першого ходу першого героя
        gameController.BeginGame();

        //смикаємо "режим слідкування" камери за активним героєм
        var cam = FindObjectOfType<CameraMovement>();
        if (cam != null)
            cam.FocusOnHero();

        //відобразити GUI
        ShowGUI();

        //закрити вікно генерації обїєкту
        CloseScriptWindow();
    }

    //TO DO : исправить отображение сетки передвижений с лифтами (на втором этаже не отображается)
    //также: запретить прыгатть со второго этажа - под сомнением (?)

    //TO DO
    private void GenerateHillsAndClimbs()
    {
        //TO DO: додати матераіл в ObstacleOnTheMap

        //add hills (по холмах можна пересуватись)        
        var hill_level1 = new ObstacleOnTheMap(new Vector2Int(10, 10), 7, 9, 1f, needSmallBoxOnTop: true, Level1_ObstacleMaterial);
        var hill_level2 = new ObstacleOnTheMap(new Vector2Int(10, 10), 3, 2, 1f, needSmallBoxOnTop: true, Level2_ObstacleMaterial);
        var hill_level3 = new ObstacleOnTheMap(new Vector2Int(10, 10), 1, 1, 1f, needSmallBoxOnTop: true, Level3_ObstacleMaterial);
        hill_level2.NestedObstacle = hill_level3;
        hill_level1.NestedObstacle = hill_level2;

        _obstacles.Add(hill_level1);
        //холми можуть матидекілька рівнів

        //add climbs
    }

    /// <summary>
    /// Створюємо 2ий поверх таліфт туди
    /// </summary>
    private void CreateLiftAndSecondFloor()
    {
        // --- Додаємо ліфт ---
        GameObject targetLiftCell = GetSmallCube(liftGridPos.x, liftGridPos.y);
        LiftPlatform liftPlatform = null;

        if (targetLiftCell != null)
        {
            // Прибираємо smallCube, бо тут має стояти ліфт
            Destroy(targetLiftCell);

            // Трохи піднімаємо ліфт, щоб він не перетинався з ground-box
            Vector3 liftWorldPos = targetLiftCell.transform.position + Vector3.up * 0.01f;

            LiftFactory lf = new LiftFactory(HeroesCollectionGUI, liftMaterial);
            GameObject lift = lf.CreateLift(liftWorldPos, transform);
            lift.tag = Constants.TagConstans.FloorGridTag;//як у смол-бокса
            liftPlatform = lift.GetComponent<LiftPlatform>();
            lift.transform.SetParent(parent.transform);

            // --- Створюємо кнопку на 1-му поверсі ---
            GameObject btnDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
            btnDown.name = "LiftButtonDown->Up";
            btnDown.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            btnDown.transform.position = new Vector3(
                liftWorldPos.x + 1f,              // трохи збоку
                bigHeight + 0.5f,                 // висота 1-го поверху
                liftWorldPos.z
            );
            btnDown.transform.SetParent(transform);
            var collider = btnDown.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = new Vector3(-2.16914f, -0.2331161f, 0.2354889f);
            collider.size = new Vector3(1.420128f, 0.5337677f, 1.470978f);
            btnDown.layer = Constants.Layers.LiftTriggerLayer;

            var buttonDown = btnDown.AddComponent<LiftButton>();
            buttonDown.lift = liftPlatform;
            buttonDown.callToUpper = true;

            // --- Створюємо кнопку на 2-му поверсі ---
            GameObject btnUp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            btnUp.name = "LiftButtonUp->Down";
            btnUp.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            btnUp.transform.position = new Vector3(
                //liftWorldPos.x + 1f,                            // збоку
                liftWorldPos.x + 1.5f,
                bigHeight + liftAboveBig + liftPlatform.targetFloorHeight + 0.25f,//0.5f,
                liftWorldPos.z + 1.3f
            //liftWorldPos.z
            );
            btnUp.transform.SetParent(transform);
            collider = btnUp.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = new Vector3(-2.760944f, -0.1841786f, -2.004732f);
            collider.size = new Vector3(2.547562f, 0.6316428f, 1.88596f);

            btnUp.layer = Constants.Layers.LiftTriggerLayer;

            var buttonUp = btnUp.AddComponent<LiftButton>();
            buttonUp.lift = liftPlatform;
            buttonUp.callToUpper = false;
        }
        else
        {
            Debug.LogWarning($"[BGS] Ліфт не створено — клітинка {liftGridPos} порожня або під будинком.");
        }
        // --- Ліфт + кнопки створені ---

        //створюємо 2ий поверх для цього ліфта
        var gg2 = new GridGenerator(
            HeroesCollectionGUI,
            2, 2,
            cellSize, bigHeight, smallHeight, liftAboveBig,
            gridBoxMaterial, groundBoxMaterial,
            parent.transform,
            obstacles: null
        );

        var floor2_x = targetLiftCell.transform.position.x + (2 * cellSize - cellSize / 2); //зміщуємо платформу щоб був доступ платформи ліфта до поверху
        var floor2_z = targetLiftCell.transform.position.z - (cellSize / 2);
        var floor2_y = Constants.TacticMapConstructingConstants.Floor2Height;
        Vector3 floor2_position = new Vector3 { x = floor2_x, y = floor2_y, z = floor2_z };
        var flor2_smallCubes = gg2.GenerateGrid(null, 4f, floor2_position);
        //додаємо ліфт-
    }

    //MISC
    public bool GetGridCoordsFromWorld(Vector3 worldPos, out int i, out int j)
    {
        i = -1;
        j = -1;

        float minDist = float.MaxValue;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                var cube = GetSmallCube(x, z);
                if (cube == null) continue;

                float dist = Vector3.Distance(worldPos, cube.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    i = x;
                    j = z;
                }
            }
        }

        return (i >= 0 && j >= 0);
    }
    public GameObject GetSmallCube(int i, int j)
    {
        if (smallCubes == null) return null;
        if (i < 0 || j < 0 || i >= smallCubes.GetLength(0) || j >= smallCubes.GetLength(1)) return null;
        return smallCubes[i, j];
    }
    private void ShowGUI()
    {
        canvasGUI.gameObject.SetActive(true);
    }
    private void CloseScriptWindow()
    {
        canvas.enabled = false;
    }
    /// <summary>
    /// Шукає найближчу ненульову (валідну) small-cube до переданих індексів.
    /// Повертає GameObject або null, якщо взагалі нічого немає.
    /// </summary>
    private GameObject FindNearestValidCell(int wantI, int wantJ)
    {
        if (smallCubes == null) return null;

        // цільова світоова позиція (орієнтовна) — може допомогти шукати близьку клітинку
        float bigWidth = width * cellSize;
        float bigLength = length * cellSize;
        Vector3 center = new Vector3(0f, bigHeight / 2f, 0f);
        Vector3 wantedWorld = new Vector3(
            center.x + (-bigWidth / 2f + (wantI + 0.5f) * cellSize),
            bigHeight + liftAboveBig,
            center.z + (-bigLength / 2f + (wantJ + 0.5f) * cellSize)
        );

        float bestDist = float.MaxValue;
        GameObject best = null;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                var c = GetSmallCube(i, j);
                if (c == null) continue;
                float d = Vector3.Distance(wantedWorld, c.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = c;
                }
            }
        }

        return best;
    }
}