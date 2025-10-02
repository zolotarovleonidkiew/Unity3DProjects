//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

///// <summary>
///// PREVIOUS VERSION (Before refactoring)
///// </summary>
//public class BackgroundGenerationScript_OLD : MonoBehaviour
//{
//    [Header("Grid (counts)")]
//    public int width = 2;     // кількість малих кубів по X
//    public int length = 2;    // кількість малих кубів по Z
//    public Material groundBoxMaterial;

//    [Header("Sizes")]
//    public float cellSize = 2f;   // розмір малого куба по X і Z (ти писав, що це має бути 2)
//    public float bigHeight = 0.5f; // висота великого куба
//    public float smallHeight = 0.5f; // висота малих кубів
//    public float liftAboveBig = 0.3f; // на скільки підняти малі куби над великим

//    [Header("Hero Settings")]
//    public List<Vector2> heroGridPositions = new()  // 🔹 кілька героїв
//    {
//        new Vector2(0, 0),
//        new Vector2(1, 0),
//        new Vector2(0, 1),
//        new Vector2(1, 1)
//    };
//    public float playerSize = 1f;
//    public Material heroBoxMaterial;
//    [SerializeField] private List<WeaponData> heroStartingWeapons;
//    [SerializeField] private GameObject bulletPrefab;

//    [Header("Alien Settings")]
//    //public Vector2 alienGridPos = new Vector2(1, 1);
//    public List<Vector2> alienGridPositions = new() // 🔹 кілька прибульців
//    {
//        new Vector2(2, 2),
//        new Vector2(3, 2),
//        new Vector2(2, 3),
//        new Vector2(3, 3),
//        new Vector2(4, 2),
//        new Vector2(4, 3)
//    };
//    public float alienSize = 1f;
//    public Material alienBoxMaterial;

//    [Header("Misc. Settings")]
//    [SerializeField] private Button _btnGenerate;
//    [SerializeField] private GameController gameController;

//    [Header("Obstacles (Buildings)")]
//    [SerializeField] private Vector2Int buildingGridPos = new(16, 10);
//    [SerializeField] private int buildingWidth = 3;   // у клітинках
//    [SerializeField] private int buildingLength = 2;  // у клітинках
//    [SerializeField] private float buildingHeight = 2f;
//    [SerializeField] private Material buildingMaterial;

//    private GameObject[,] smallCubes;
//    public GameObject parent;
//    public Material gridBoxMaterial;

//    /// <summary>
//    /// TO CLOSE CANVAS ONLY
//    /// </summary>
//    public Canvas canvas;
//    public Canvas canvasGUI;

//    private void Start()
//    {
//        _btnGenerate.onClick.AddListener(() => CreateBoxAndSubBoxes(width, length));
//    }

//    /// <summary>
//    /// MAIN EVENT
//    /// </summary>
//    public void CreateBoxAndSubBoxes(int cellsX, int cellsZ)
//    {
//        Debug.Log($"[CreateBoxAndSubBoxes] OK, value: {length} / {width}");

//        // Гарантуємо коректні значення
//        cellsX = Mathf.Max(1, cellsX);
//        cellsZ = Mathf.Max(1, cellsZ);

//        // Розміри великого боксу = кількість осередків * розмір осередку
//        float bigWidth = cellsX * cellSize;
//        float bigLength = cellsZ * cellSize;

//        // Створюємо великий куб
//        GameObject bigBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        bigBox.name = $"BigBox_{bigWidth}x{bigHeight}x{bigLength}";
//        bigBox.transform.localScale = new Vector3(bigWidth, bigHeight, bigLength);
//        bigBox.transform.position = new Vector3(0f, bigHeight / 2f, 0f);
//        bigBox.transform.SetParent(parent.transform);
//        bigBox.GetComponent<Renderer>().material = groundBoxMaterial;

//        // добавляем физику (пол)
//        var rb = bigBox.AddComponent<Rigidbody>();
//        rb.isKinematic = true;
//        rb.useGravity = false;

//        // массив для маленьких кубов
//        smallCubes = new GameObject[cellsX, cellsZ];

//        // Центр великого куба
//        Vector3 center = bigBox.transform.position;

//        // Створюємо малі куби у центрах кожної "клітинки"
//        for (int i = 0; i < cellsX; i++)
//        {
//            for (int j = 0; j < cellsZ; j++)
//            {
//                // Центр клітинки (зліва-направо, зпереду-назад)
//                float x = -bigWidth / 2f + (i + 0.5f) * cellSize;
//                float z = -bigLength / 2f + (j + 0.5f) * cellSize;

//                Vector3 smallPos = new Vector3(
//                    center.x + x,
//                    bigHeight + liftAboveBig,
//                    center.z + z
//                );

//                //перевіряємо - внутри перепон (будинків ) не створюємо smallBox*и
//                if (IsInsideHouseArea(i, j))
//                {
//                    continue;
//                }

//                GameObject smallBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                smallBox.name = $"SmallBox_{i}_{j}";
//                smallBox.transform.localScale = new Vector3(cellSize, smallHeight, cellSize);
//                smallBox.transform.position = smallPos;
//                smallBox.tag = Constants.TagConstans.FloorGridTag;
//                smallBox.GetComponent<Renderer>().material = gridBoxMaterial;
//                smallBox.transform.SetParent(bigBox.transform);

//                // удаляем физику, оставляем только визуал
//                //Destroy(smallBox.GetComponent<Collider>());
//                BoxCollider dc = smallBox.GetComponent<BoxCollider>();
//                dc.isTrigger = true;

//                smallCubes[i, j] = smallBox;
//            }
//        }

//        //Deprecated (герой + alien)
//        //CreatePlayerAt(playerGridPos);
//        //CreateAlienAt(alienGridPos);
//        List<Hero> createdHeroes = new List<Hero>();
//        for (int h = 0; h < heroGridPositions.Count; h++)
//        {
//            Hero hero = CreatePlayerAt(heroGridPositions[h], h);
//            createdHeroes.Add(hero);

//            //register hero's
//            gameController.RegisterHero(hero);
//        }

//        // Створюємо всіх прибульців
//        for (int a = 0; a < alienGridPositions.Count; a++)
//        {
//            Alien al = CreateAlienAt(alienGridPositions[a], a);

//            //register alien's
//            gameController.RegisterAlien(al);
//        }

//        // Створюємо будинок
//        CreateBuilding();

//        //запуск першого ходу першого героя
//        gameController.BeginGame();

//        //смикаємо "режим слідкування" камери за активним героєм
//        var cam = FindObjectOfType<CameraMovement>();
//        if (cam != null)
//            cam.FocusOnHero();

//        //відобразити GUI
//        ShowGUI();

//        //закрити вікно генерації обїєкту
//        CloseScriptWindow();
//    }

//    /// <summary>
//    /// Create Player
//    /// </summary>
//    private Hero CreatePlayerAt(Vector2 gridCoords, int index)
//    {
//        int i = Mathf.FloorToInt(gridCoords.x);
//        int j = Mathf.FloorToInt(gridCoords.y);

//        if (smallCubes == null || i < 0 || j < 0 || i >= smallCubes.GetLength(0) || j >= smallCubes.GetLength(1))
//        {
//            Debug.LogError($"Некорректные координаты игрока: ({i},{j})");
//            return null;
//        }

//        GameObject targetCell = smallCubes[i, j];
//        Vector3 pos = targetCell.transform.position;

//        float playerY = bigHeight + playerSize / 2f;

//        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        player.name = $"Hero_{index}";
//        player.transform.localScale = new Vector3(playerSize, playerSize, playerSize);
//        player.transform.position = new Vector3(pos.x, playerY, pos.z);

//        player.GetComponent<Renderer>().material = heroBoxMaterial;

//        var rb = player.AddComponent<Rigidbody>();
//        rb.mass = 1f;
//        rb.constraints = RigidbodyConstraints.FreezeRotation;

//        var sh = player.AddComponent<Hero>();
//        sh.SettoShowAvailableMovementSquares();
//        sh.SetGroundObject(this);
//        sh.SetStartingWeapons(heroStartingWeapons);
//        sh.SetBulletPrefab(bulletPrefab);

//        GameObject shootPoint = new GameObject("ShootPoint");
//        shootPoint.transform.SetParent(player.transform);
//        shootPoint.transform.localPosition = Vector3.up * 0.5f;
//        sh.SetShootPoint(shootPoint.transform);

//        var hms = player.AddComponent<HeroMovement>();
//        hms.SetMoveSpeed(3);

//        return sh;
//    }

//    /// <summary>
//    /// Create Alien
//    /// </summary>
//    private Alien CreateAlienAt(Vector2 gridCoords, int index)
//    {
//        int i = Mathf.FloorToInt(gridCoords.x);
//        int j = Mathf.FloorToInt(gridCoords.y);

//        if (smallCubes == null || i < 0 || j < 0 || i >= smallCubes.GetLength(0) || j >= smallCubes.GetLength(1))
//        {
//            Debug.LogError($"Некорректные координаты чужого: ({i},{j})");
//            return null;
//        }

//        GameObject targetCell = smallCubes[i, j];
//        Vector3 pos = targetCell.transform.position;

//        float alienY = bigHeight + alienSize / 2f;

//        GameObject alien = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        alien.name = $"Alien_{index}";
//        alien.transform.localScale = new Vector3(alienSize, alienSize, alienSize);
//        alien.transform.position = new Vector3(pos.x, alienY, pos.z);
//        alien.GetComponent<Renderer>().material = alienBoxMaterial;
//        alien.tag = Constants.TagConstans.AlienTag;

//        var rb = alien.AddComponent<Rigidbody>();
//        rb.mass = 1f;
//        rb.constraints = RigidbodyConstraints.FreezeRotation;

//        var al = alien.AddComponent<Alien>();
//        al.SetGroundObject(this);

//        return al;
//    }

//    /// <summary>
//    /// Create Building (Obstacle)
//    /// </summary>
//    private void CreateBuilding()
//    {
//        if (buildingWidth <= 0 || buildingLength <= 0) return;

//        float bigWidth = width * cellSize;
//        float bigLength = length * cellSize;

//        // центр будинку по XZ
//        float x = -bigWidth / 2f + (buildingGridPos.x + buildingWidth / 2f) * cellSize;
//        float z = -bigLength / 2f + (buildingGridPos.y + buildingLength / 2f) * cellSize;

//        // правильна висота: як у smallBox (поверхня bigBox)
//        float y = bigHeight + (buildingHeight / 2f);

//        Vector3 pos = new Vector3(x, y, z);

//        GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        building.name = $"Building_{buildingGridPos.x}_{buildingGridPos.y}";
//        building.transform.localScale = new Vector3(buildingWidth * cellSize, buildingHeight, buildingLength * cellSize);
//        building.transform.position = pos;
//        building.transform.SetParent(parent.transform);

//        if (buildingMaterial != null)
//            building.GetComponent<Renderer>().material = buildingMaterial;

//        // фізика — як стіна
//        var rb = building.AddComponent<Rigidbody>();
//        rb.isKinematic = true;
//        rb.useGravity = false;
//    }

//    //MISC
//    public bool GetGridCoordsFromWorld(Vector3 worldPos, out int i, out int j)
//    {
//        i = -1;
//        j = -1;

//        float minDist = float.MaxValue;

//        for (int x = 0; x < width; x++)
//        {
//            for (int z = 0; z < length; z++)
//            {
//                var cube = GetSmallCube(x, z);
//                if (cube == null) continue;

//                float dist = Vector3.Distance(worldPos, cube.transform.position);
//                if (dist < minDist)
//                {
//                    minDist = dist;
//                    i = x;
//                    j = z;
//                }
//            }
//        }

//        return (i >= 0 && j >= 0);
//    }
//    public GameObject GetSmallCube(int i, int j)
//    {
//        if (smallCubes == null) return null;
//        if (i < 0 || j < 0 || i >= smallCubes.GetLength(0) || j >= smallCubes.GetLength(1)) return null;
//        return smallCubes[i, j];
//    }
//    private bool IsInsideHouseArea(float x, float z)
//    {
//        return x >= buildingGridPos.x && x < buildingGridPos.x + buildingWidth &&
//              z >= buildingGridPos.y && z < buildingGridPos.y + buildingLength;
//    }
//    private void ShowGUI()
//    {
//        canvasGUI.gameObject.SetActive(true);
//    }
//    private void CloseScriptWindow()
//    {
//        canvas.enabled = false;
//    }
//}