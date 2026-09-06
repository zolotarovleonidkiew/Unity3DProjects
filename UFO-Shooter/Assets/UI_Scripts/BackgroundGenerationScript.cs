using Assets.Scripts.TacticMap;
using Assets.Scripts.TacticMap.V05;
using Assets.Scripts.TacticMap.V05.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static ObstacleFactory;

public class BackgroundGenerationScript : MonoBehaviour
{
    [Header("=== Hero Settings ===")]
    public List<UILiveCreatureDisposition> heroesGridPositions = new()
    {
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(0, 0), GroundLayer = GroundHierarchyLevel.Level_1 },
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(1, 0), GroundLayer = GroundHierarchyLevel.Level_1 },
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(0, 1), GroundLayer = GroundHierarchyLevel.Level_2 },
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(1, 1), GroundLayer = GroundHierarchyLevel.Level_2 }
    };
    public float playerSize = 1f;
    public Material heroBoxMaterial;
    [SerializeField] private List<WeaponData> heroStartingWeapons;
    [SerializeField] private GameObject bulletPrefab; //TO DO: make it a list of prefabs for different weapons
    [SerializeField] private GameObject HeroesCollectionGUI;
    [Header("=== Hero Settings END===")]

    [Header("=== Alien Settings ===")]
    public List<UILiveCreatureDisposition> aliensGridPositions = new()
    {
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(2, 2), alienTypesEnum = AlienTypesEnum.level_0_Greys,             GroundLayer = GroundHierarchyLevel.Level_1 },
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(3, 2), alienTypesEnum = AlienTypesEnum.level_0_Light_Drone,       GroundLayer = GroundHierarchyLevel.Level_1 },
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(2, 3), alienTypesEnum = AlienTypesEnum.level_1_Heavy_Drone,       GroundLayer = GroundHierarchyLevel.Level_1 },
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(3, 3), alienTypesEnum = AlienTypesEnum.level_2_Assault_Trooper,   GroundLayer = GroundHierarchyLevel.Level_1 },
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(4, 2), alienTypesEnum = AlienTypesEnum.level_2_Insect,            GroundLayer = GroundHierarchyLevel.Level_1 },
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(4, 3), alienTypesEnum = AlienTypesEnum.level_3_Assault_Machine,   GroundLayer = GroundHierarchyLevel.Level_1 },
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(5, 3), alienTypesEnum = AlienTypesEnum.level_3_Master_Mind,       GroundLayer = GroundHierarchyLevel.Level_1 },
        new UILiveCreatureDisposition { GridPosition = new Vector2Int(3, 3), alienTypesEnum = AlienTypesEnum.level_4_Insect_Matriarch,  GroundLayer = GroundHierarchyLevel.Level_1 }
    };

    [SerializeField] private GameObject level_0_Greys_Prefab;
    [SerializeField] private GameObject level_0_Light_Drone_Prefab;
    [SerializeField] private GameObject level_1_Heavy_Drone_Prefab;
    [SerializeField] private GameObject level_2_Assault_Trooper_Prefab;
    [SerializeField] private GameObject level_2_Insect_Prefab;
    [SerializeField] private GameObject level_3_Assault_Machine_Prefab;
    [SerializeField] private GameObject level_3_Master_Mind_Prefab;
    [SerializeField] private GameObject level_4_Insect_Matriarch_Prefab;

    public float alienSize = 1f;
    public Material alienBoxMaterial;
    [SerializeField] private GameObject AlienCollectionGUI;
    [Header("=== Alien Settings END===")]

    [Header("Misc. Settings")]
    [SerializeField] private Button _btnGenerate;
    [SerializeField] private GameController gameController;

    [Header("Obstacles => Buildings, trees")]
    [SerializeField] private List<ObstacleOnTheMap> _obstacles;    
    [SerializeField] private List<Ramp> _RampsCollection;
    [SerializeField] private Material Level1_ObstacleMaterial;
    [SerializeField] private Material Level2_ObstacleMaterial;
    [SerializeField] private Material Level3_ObstacleMaterial;

    [Header("Obstacles => Hills (calls 'RampsCollection')")]
    [SerializeField] private List<HillOnTheMap> _hills;

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
    [SerializeField] private List<Vector2Int> liftGridPosCollection = new List<Vector2Int> { new(2, 2) };
    [SerializeField] private Material liftMaterial;

    [Header("Misc.")]
    [SerializeField] private ObstructionManager obstructionManager;

    public GameObject parent;
    public Material gridBoxMaterial;
    public Material gridBoxMaterialHighlited;

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
        IGridGenerator gridGenerator = new GridGenerator_05();
        IObstacleGenerator obstacleGenerator = new ObstacleGenerator_05();
        ILiftGenerator liftGenerator = new LiftGenerator_05();
        ILandCreator landReactor = new LandCreator(gridGenerator, obstacleGenerator, liftGenerator);

        #region Main Land

        var landCreatorConfig = new LandCreatorConfig {
            GridConfig = new GridConfig
            {
                CellSize = cellSize,
                Length = length,
                Width = width,
                GroundHeight = bigHeight,
                SmallBoxHeight = smallHeight,
                GridMaterial = gridBoxMaterial,
                GroundMaterial = groundBoxMaterial,
                LiftPositions = liftGridPosCollection,
                Parent = parent.transform,
                Obstacles = _obstacles,
                Ramps = _RampsCollection
            },
            ObstacleConfig = new ObstacleConfig
            {
                CellSize = cellSize,
                Obstacles = _obstacles,
                Hills = _hills,
                Parent = parent.transform,
                PlatformHeight = bigHeight,
                TotalLength = length,
                TotalWidth = width,
                Level1_ObstacleMaterial = Level1_ObstacleMaterial,
                Level2_ObstacleMaterial = Level2_ObstacleMaterial,
                Level3_ObstacleMaterial = Level3_ObstacleMaterial
            },
            LiftConfig = new LiftConfig(cellSize, bigHeight, smallHeight, liftGridPosCollection, parent.transform,
                liftMaterial, gridBoxMaterial, groundBoxMaterial),
            HeroesCollectionGUI = HeroesCollectionGUI
        };

        IGroundHierarchy platformLayersCollection = landReactor.CreateLand(landCreatorConfig);

        #endregion
        
        //live enities:
        var heroFactory = new HeroFactory(playerSize, heroBoxMaterial, heroStartingWeapons, bulletPrefab, this, HeroesCollectionGUI, gridBoxMaterialHighlited);
        var alienFactory = new AlienFactory(alienSize, alienBoxMaterial, this, AlienCollectionGUI);

        // Герої    
        for (int h = 0; h < heroesGridPositions.Count; h++)
        {
            var heroPosition = heroesGridPositions[h];
            int xi = Mathf.FloorToInt(heroPosition.GridPosition.x);
            int zj = Mathf.FloorToInt(heroPosition.GridPosition.y);

            var layer = platformLayersCollection.GetGroundLayer(heroPosition.GroundLayer);
            
            if (layer is null)
            {
                throw new Exception($"[CreateBoxAndSubBoxes] Layer {heroPosition.GroundLayer} not found or not generated");
            }

            var smallBoxed = layer.SmallBoxed;

            GameObject targetCell = GridGenerator_05.GetSmallCube(xi, zj, smallBoxed);
            if (targetCell == null)
            {
                // знаходимо найближчу валідну клітинку (якщо початкова була під будинком або поза межею)
                targetCell = GridGenerator_05.FindNearestValidCell(xi, zj, smallBoxed, cellSize, bigHeight, liftAboveBig);
                if (targetCell == null)
                {
                    Debug.LogError($"[BGS] Не знайдено валідної клітинки для героя {h} (запитано {xi},{zj})");
                    continue;
                }
            }

            Hero hero = heroFactory.CreateHero(targetCell, h, heroPosition.CreaturePrefab);
            if (hero != null)
                gameController.RegisterHero(hero);
        }

        // Чужі
        for (int a = 0; a < aliensGridPositions.Count; a++)
        {
            var alienPosition = aliensGridPositions[a];
            int xi = Mathf.FloorToInt(alienPosition.GridPosition.x);
            int zj = Mathf.FloorToInt(alienPosition.GridPosition.y);

            var smallBoxed = platformLayersCollection.GetGroundLayer(alienPosition.GroundLayer)?.SmallBoxed;

            GameObject targetCell = GridGenerator_05.GetSmallCube(xi, zj, smallBoxed);
            if (targetCell == null)
            {
                targetCell = GridGenerator_05.FindNearestValidCell(xi, zj, smallBoxed, cellSize, bigHeight, liftAboveBig);
                if (targetCell == null)
                {
                    Debug.LogError($"[BGS] Не знайдено валідної клітинки для alien {a} (запитано {xi},{zj})");
                    continue;
                }
            }

            //choose required alien prefab based on alienTypesEnum +
            GameObject alienPrefab = GetAlienPrefabByType(alienPosition.alienTypesEnum);

            Alien al = alienFactory.CreateAlien(alienPosition.alienTypesEnum, targetCell, a, alienPrefab);
            if (al != null)
                gameController.RegisterAlien(al);
        }

        gameController.RegisterObstructionManager(obstructionManager);

        ValidateSmallBoxes(platformLayersCollection);
        //game started:

        //запуск першого ходу першого героя
        gameController.BeginGame();

        //смикаємо "режим слідкування" камери за активним героєм
        var cam = FindAnyObjectByType<CameraMovement>();
        if (cam != null)
            cam.FocusOnHero();     

        //відобразити GUI
        ShowGUI();

        //закрити вікно генерації обїєкту
        CloseScriptWindow();

        //Test static data:
        StaticTacticalData.GroundHierarchy = platformLayersCollection;
        StaticTacticalData.Obstacles = _obstacles;


    }

    private void ShowGUI()
    {
        canvasGUI.gameObject.SetActive(true);
    }
    private void CloseScriptWindow()
    {
        canvas.enabled = false;
    }

    private GameObject GetAlienPrefabByType(AlienTypesEnum alienTypesEnum)
    {
        GameObject prefab = null;
        switch (alienTypesEnum)
        {
            case AlienTypesEnum.level_0_Greys:
                prefab = level_0_Greys_Prefab;
                break;
            case AlienTypesEnum.level_0_Light_Drone:
                prefab = level_0_Light_Drone_Prefab;
                break;
            case AlienTypesEnum.level_1_Heavy_Drone:
                prefab = level_1_Heavy_Drone_Prefab;
                break;
            case AlienTypesEnum.level_2_Assault_Trooper:
                prefab = level_2_Assault_Trooper_Prefab;
                break;
            case AlienTypesEnum.level_2_Insect:
                prefab = level_2_Insect_Prefab;
                break;
            case AlienTypesEnum.level_3_Assault_Machine:
                prefab = level_3_Assault_Machine_Prefab;
                break;
            case AlienTypesEnum.level_3_Master_Mind:
                prefab = level_3_Master_Mind_Prefab;
                break;
            case AlienTypesEnum.level_4_Insect_Matriarch:
                prefab = level_4_Insect_Matriarch_Prefab;
                break;
        }

        return prefab;
    }

    /// <summary>
    /// Validates and modifies WayPoint component in SB.
    /// </summary>
    private void ValidateSmallBoxes(IGroundHierarchy platformLayersCollection)
    {
        var v  = new SmallBoxesValidation();
        v.ValidateAndFix(platformLayersCollection);
    }
}