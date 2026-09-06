using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement/platform Grid generator
/// </summary>
public class GridGenerator
{
    private readonly SmallBoxCreatingService _smallBoxCreatingService = new SmallBoxCreatingService();
    private int _width;
    private int _length;
    private float _cellSize;
    private float _bigHeight;
    private float _smallHeight;
    private float _liftAboveBig;
    private Material _gridMaterial;
    private Material _groundMaterial;
    private Transform _parent;

    // Перешкоди (будівлі)
    private List<ObstacleOnTheMap> _obstacles = new();

    // lift (don't remove)
    private Vector2Int? _liftGridPos;
    private LiftFactory _liftFactory;

    private GameObject[,] _smallCubes;
    private GameObject _bigBox;
    private GameObject HeroesCollectionGUI;

    public GridGenerator(
        GameObject heroesCollectionGUI,
        int width, int length,
        float cellSize, float bigHeight, float smallHeight, float liftAboveBig,
        Material gridMaterial, Material groundMaterial,
        Transform parent,
        List<ObstacleOnTheMap> obstacles = null,
        Vector2Int? liftGridPos = null, LiftFactory liftFactory = null
    )
    {
        _width = width;
        _length = length;
        _cellSize = cellSize;
        _bigHeight = bigHeight;
        _smallHeight = smallHeight;
        _liftAboveBig = liftAboveBig;
        _gridMaterial = gridMaterial;
        _groundMaterial = groundMaterial;
        _parent = parent;

        HeroesCollectionGUI = heroesCollectionGUI;

        if (obstacles != null && obstacles.Count > 0)
            _obstacles = obstacles;

        if (liftGridPos.HasValue)
        {
            _liftGridPos = liftGridPos;
            _liftFactory = liftFactory ?? new LiftFactory();
        }
    }

    /// <summary>
    /// Створює великий куб + дрібні клітинки
    /// </summary>
    public GameObject[,] GenerateGrid(List<Ramp> RampsCollection = null, float? bigHeight = null, Vector3? startPosition = null)
    {
        // розрахунок розмірів
        float bigWidth = _width * _cellSize;
        float bigLength = _length * _cellSize;

        // етаж, він же "великий куб" (основа)
        _bigBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _bigBox.name = $"Floor_TacticalMap_{bigLength}x{bigWidth}";
        _bigBox.transform.localScale = new Vector3(bigWidth, _bigHeight, bigLength);
        bigHeight = bigHeight ?? _bigHeight / 2f;
        startPosition = startPosition ?? new Vector3(0f, bigHeight.Value, 0f);
        _bigBox.transform.position = startPosition.Value;
        _bigBox.transform.SetParent(_parent);
        _bigBox.GetComponent<Renderer>().material = _groundMaterial;

        var rb = _bigBox.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // масив клітинок
        _smallCubes = new GameObject[_width, _length];
        Vector3 center = _bigBox.transform.position;

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _length; j++)
            {
                // 🔹 якщо тут є перешкода
                ObstacleOnTheMap obstacle = GetObstacleAt(i, j);
                if (obstacle != null)
                {
                    if (!obstacle.NeedToCreateSmallBoxOnTheTop)
                        continue; // не створюємо smallBox

                    // створюємо смол-бокс зверху будівлі
                    float xTop = -bigWidth / 2f + (i + 0.5f) * _cellSize;
                    float zTop = -bigLength / 2f + (j + 0.5f) * _cellSize;

                    Vector3 topPos = new Vector3(
                        center.x + xTop,
                        GetTotalHeight(obstacle) + _liftAboveBig + _smallHeight,
                        center.z + zTop
                    );

                    GameObject topBox = _smallBoxCreatingService.CreateSmallBox(
                        $"TopSmallBox_{i}_{j}",
                        topPos,
                        new Vector3(_cellSize, _smallHeight, _cellSize),
                        _bigBox.transform,
                        _gridMaterial,
                        i,
                        j);

                    //додаємо створений смол-бокс в колекцію (ObstacleOnTheMap), щоб потім паретна переасаайнити конкретному обстеклу
                    //видалити???
                    if (obstacle.SmallBoxes is null)
                    {
                        obstacle.SmallBoxes = new();
                    }
                    obstacle.SmallBoxes.Add(topBox);

                    _smallCubes[i, j] = topBox;
                    continue;
                }

                // 🔹 якщо це клітинка для ліфта
                float x = -bigWidth / 2f + (i + 0.5f) * _cellSize;
                float z = -bigLength / 2f + (j + 0.5f) * _cellSize;
                Vector3 smallPos = new Vector3(
                    center.x + x,
                    bigHeight.Value + _liftAboveBig,
                    center.z + z
                );

                if (_liftGridPos.HasValue && i == _liftGridPos.Value.x && j == _liftGridPos.Value.y)
                {
                    smallPos.y = bigHeight.Value * 2 + _liftAboveBig;
                }

                GameObject smallBox = _smallBoxCreatingService.CreateSmallBox(
                    $"SmallBox_{i}_{j}",
                    smallPos,
                    new Vector3(_cellSize, _smallHeight, _cellSize),
                    _bigBox.transform,
                    _gridMaterial,
                    i,
                    j);

                _smallCubes[i, j] = smallBox;
            }
        }

        if (RampsCollection != null)
        {
            //створити пандуси для холмів тут 
            foreach (Ramp ramp in RampsCollection)
            {
                //далі привяжем пандус по obstacle
                ramp.GO =
                    CreateRamp(ramp.i, ramp.j, ramp.direction, ramp.parent, ramp.material, ramp.targetCell);
            }
        }

        return _smallCubes;
    }

    public GameObject GetSmallCube(int i, int j)
    {
        if (_smallCubes == null) return null;
        if (i < 0 || j < 0 || i >= _smallCubes.GetLength(0) || j >= _smallCubes.GetLength(1)) return null;
        return _smallCubes[i, j];
    }

    public bool GetGridCoordsFromWorld(Vector3 worldPos, out int i, out int j)
    {
        i = -1;
        j = -1;
        float minDist = float.MaxValue;

        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _length; z++)
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

    public ObstacleOnTheMap GetObstacleAt(int i, int j)
    {
        if (_obstacles == null) return null;

        foreach (var obs in _obstacles)
        {
            var found = GetObstacleAtRecursive(obs, i, j);
            if (found != null)
                return found;
        }
        return null;
    }

    public ObstacleOnTheMap GetObstacleAtRecursive(ObstacleOnTheMap obs, int i, int j)
    {
        int startX = obs.ObstacleGridPos.x;
        int startZ = obs.ObstacleGridPos.y;

        int endX = startX + obs.BuildingWidth - 1;
        int endZ = startZ + obs.BuildingLength - 1;

        if (i >= startX && i <= endX && j >= startZ && j <= endZ)
        {
            // 🔹 якщо є NestedObstacle, треба перевірити спочатку його
            if (obs.NestedObstacle != null)
            {
                var nestedFound = GetObstacleAtRecursive(obs.NestedObstacle, i, j);
                if (nestedFound != null)
                    return nestedFound; // повертаємо найглибший
            }
            return obs; // якщо немає вкладеного → цей obstacle і є результат
        }

        return null;
    }

    private float GetTotalHeight(ObstacleOnTheMap obs)
    {
        float height = obs.BuildingHeight;
        var parent = FindParentObstacle(obs);
        while (parent != null)
        {
            height += parent.BuildingHeight;
            parent = FindParentObstacle(parent);
        }
        return height;
    }

    /// <summary>
    /// Шукає батька конкретного obstacle у списку _obstacles
    /// </summary>
    private ObstacleOnTheMap FindParentObstacle(ObstacleOnTheMap child)
    {
        foreach (var obs in _obstacles)
        {
            if (obs.NestedObstacle == child)
                return obs;
            var parent = FindParentObstacleRecursive(obs, child);
            if (parent != null)
                return parent;
        }
        return null;
    }

    private ObstacleOnTheMap FindParentObstacleRecursive(ObstacleOnTheMap parent, ObstacleOnTheMap child)
    {
        if (parent.NestedObstacle == child)
            return parent;

        if (parent.NestedObstacle != null)
            return FindParentObstacleRecursive(parent.NestedObstacle, child);

        return null;
    }

    private GameObject CreateRamp(int i, int j, RampDirection direction, Transform parent, Material material, Vector2Int targetCell)
    {
        // розміри
        float w = _cellSize;         // ширина по X
        float l = _cellSize;         // довжина по Z (похилу беремо по цій осі)
        float h;                     // висота пандуса

        // Центр великого боксу
        Vector3 center = _bigBox.transform.position;

        // світова позиція клітинки (центр)
        float bigWidth = _width * _cellSize;
        float bigLength = _length * _cellSize;
        float x = -bigWidth / 2f + (i + 0.5f) * _cellSize;
        float z = -bigLength / 2f + (j + 0.5f) * _cellSize;

        // знаходимо перешкоду під цією клітинкою (якщо є)
        ObstacleOnTheMap obstacle = GetObstacleAt(i, j);

        // базовий Y: якщо є obstacle — ставимо пандус НА її верх (GetTotalHeight дає сумарну висоту над землею)
        // Якщо obstacle == null — пандус починається з поверхні мапи
        float baseSurfaceY;
        if (obstacle != null)
        {
            // GetTotalHeight(obs) дає сумарну висоту над землею до верхньої поверхні цієї перешкоди
            // Ми розташовуємо нижню грань пандуса саме на цій поверхні (щоб пандус "лежав" на перешкоді)
            baseSurfaceY = center.y + GetTotalHeight(obstacle) + 0.2f;
            h = obstacle.BuildingHeight; // висота підйому пандуса — висота цієї перешкоди
        }
        else
        {
            // якщо немає перешкоди — ставимо пандус з нульовим підйомом (або невеликим)
            baseSurfaceY = center.y + _liftAboveBig; // використаємо рівень small-box
            h = _cellSize * 0.5f; // невеликий підйом, можна налаштувати
        }

        // створюємо об'єкт рами (буде містити mesh, renderer та коллайдер)
        GameObject ramp = new GameObject($"Ramp_{i}_{j}");
        ramp.transform.SetParent(parent, true);
        ramp.transform.position = new Vector3(center.x + x, baseSurfaceY, center.z + z);
        ramp.transform.rotation = Quaternion.identity;

        // *** Створюємо mesh клину (wedge) орієнтований вздовж +Z (тобто "вищий кінець" буде по +Z) ***
        Mesh mesh = new Mesh();
        float halfW = w * 0.5f;
        float halfL = l * 0.5f;

        // Вершини (локальні)
        Vector3[] verts = new Vector3[]
        {
        // нижній прямокутник (низ)
        new Vector3(-halfW, 0f, -halfL), // 0 back-left bottom
        new Vector3( halfW, 0f, -halfL), // 1 back-right bottom
        new Vector3(-halfW, 0f,  halfL), // 2 front-left bottom
        new Vector3( halfW, 0f,  halfL), // 3 front-right bottom

        // верхні вершини на "фронті" (top edge)
        new Vector3(-halfW, h,  halfL),  // 4 front-left top
        new Vector3( halfW, h,  halfL)   // 5 front-right top
        };

        // Трикутники: нижня грань, боки, нахилена верхня грань і закриття "задньої" сторони
        int[] tris = new int[]
        {
        // bottom
        0,1,3,
        0,3,2,

        // left side
        0,2,4,

        // right side
        1,5,3,

        // sloped (front) face (quad 2,3,5,4)
        2,3,5,
        2,5,4,

        // back face (закриваємо форму)
        0,4,1,
        1,4,5
        };

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // додаємо компонент MeshFilter/Renderer/Collider
        var mf = ramp.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        var mr = ramp.AddComponent<MeshRenderer>();
        mr.material = material != null ? material : _gridMaterial;

        var mc = ramp.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        mc.convex = false;    // статична перешкода — залишаємо false
        mc.isTrigger = false; // важливо: щоб герой НЕ проходив наскрізь

        // Тепер повертаємо раму відповідно до direction (за замовчуванням — підйом у +Z)
        switch (direction)
        {
            case RampDirection.North: // підйом у +Z — нічого не робимо
                ramp.transform.Rotate(0f, 0f, 0f);
                break;
            case RampDirection.South: // перевернути на 180°
                ramp.transform.Rotate(0f, 180f, 0f);
                break;
            case RampDirection.East: // повернути праворуч (+X)
                ramp.transform.Rotate(0f, 90f, 0f);
                break;
            case RampDirection.West: // повернути ліворуч (-X)
                ramp.transform.Rotate(0f, -90f, 0f);
                break;
        }

        // *** Створюємо горизонтальний small-box зверху пандуса ***
        // ставимо його як дочірній об'єкт ramp і позиціонуємо локально на "фронті" (верхній край пандуса)
        // Локальна позиція: вперед на половину довжини + невеликий зміщ, вгору на (h + smallHeight/2)
        float forwardOffset = halfL; // front most position
        float verticalOffset = h + (_smallHeight / 2f);

        // Т.к. ramp є повернутий відповідно до direction, встановимо localPosition у локальних координатах ramp:
        GameObject topBox = _smallBoxCreatingService.CreateSmallBox(
            $"RampTopBox_{i}_{j}",
            new Vector3(0f, verticalOffset, forwardOffset),
            new Vector3(_cellSize, _smallHeight, _cellSize),
            ramp.transform,
            _gridMaterial,
            i,
            j,
            useLocalPosition: true,
            hideRenderer: false);

        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        var marker = topBox.AddComponent<RampMarker>();
        marker.targetCell = targetCell;
        topBox.tag = Constants.TagConstans.RampTopBox;
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        // зберігаємо у відповідній перешкоді (якщо є), щоб потім пересунути його в парент-перешкоду
        if (obstacle != null)
        {
            if (obstacle.SmallBoxes == null) obstacle.SmallBoxes = new List<GameObject>();
            obstacle.SmallBoxes.Add(topBox);
        }

        // запис у загальний масив _smallCubes, щоб GetSmallCube повертав цей topBox (замість пустоти)
        if (_smallCubes != null && i >= 0 && i < _smallCubes.GetLength(0) && j >= 0 && j < _smallCubes.GetLength(1))
        {
            _smallCubes[i, j] = topBox;
        }

        return ramp;
    }
}