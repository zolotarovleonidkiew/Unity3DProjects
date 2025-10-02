using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Movement/platform Grid generator
/// </summary>
public class GridGenerator
{
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
            _liftFactory = liftFactory ?? new LiftFactory(HeroesCollectionGUI);
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

                    GameObject topBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    topBox.name = $"TopSmallBox_{i}_{j}";
                    topBox.transform.localScale = new Vector3(_cellSize, _smallHeight, _cellSize);
                    topBox.transform.position = topPos;
                    topBox.transform.SetParent(_bigBox.transform);
                    topBox.tag = Constants.TagConstans.FloorGridTag;
                    topBox.GetComponent<Renderer>().material = _gridMaterial;
                    topBox.GetComponent<BoxCollider>().isTrigger = true;

                    //додаємо створений смол-бокс в колекцію (ObstacleOnTheMap), щоб потім паретна переасаайнити конкретному обстеклу
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

                GameObject smallBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                smallBox.name = $"SmallBox_{i}_{j}";
                smallBox.transform.localScale = new Vector3(_cellSize, _smallHeight, _cellSize);
                smallBox.transform.position = smallPos;
                smallBox.transform.SetParent(_bigBox.transform);
                smallBox.tag = Constants.TagConstans.FloorGridTag;
                smallBox.GetComponent<Renderer>().material = _gridMaterial;
                smallBox.GetComponent<BoxCollider>().isTrigger = true;

                _smallCubes[i, j] = smallBox;
            }
        }

        if (RampsCollection != null)
        {
            //створити пандуси для холмів тут 
            //TO DO
            foreach (var ramp in RampsCollection)
            {
                CreateRamp(ramp.i, ramp.j, ramp.direction, ramp.parent, ramp.material);
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

    private ObstacleOnTheMap GetObstacleAt(int i, int j)
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

    private ObstacleOnTheMap GetObstacleAtRecursive(ObstacleOnTheMap obs, int i, int j)
    {
        int startX = obs.BuildingGridPos.x;
        int startZ = obs.BuildingGridPos.y;

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

    /// <summary>
    /// TO DO: задавати їх в інвпекторі
    /// </summary>
    private void CreateRamp(int i, int j, RampDirection direction, Transform parent, Material material)
    {
        float bigWidth = _width * _cellSize;
        float bigLength = _length * _cellSize;
        Vector3 center = _bigBox.transform.position;

        // центр клітинки
        float x = -bigWidth / 2f + (i + 0.5f) * _cellSize;
        float z = -bigLength / 2f + (j + 0.5f) * _cellSize;

        // шукаємо obstacle
        ObstacleOnTheMap obstacle = GetObstacleAt(i, j);
        float baseY = obstacle != null ? GetTotalHeight(obstacle) : 0.03f; //OLD: _bigHeight / 2f;

        // позиція пандуса
        Vector3 rampPos = new Vector3(center.x + x, baseY, center.z + z);

        // створюємо пандус
        GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ramp.name = $"Ramp_{i}_{j}";
        ramp.transform.localScale = new Vector3(2, 3, 2);
        ramp.transform.position = rampPos;
        ramp.transform.SetParent(parent);
        ramp.GetComponent<Renderer>().material = material;
        ramp.GetComponent<BoxCollider>().isTrigger = false;

        var angle = 69.549f; //OLD: 45

        // орієнтація за напрямком
        switch (direction)
        {
            case RampDirection.North: // "піднімаємось на північ"
                ramp.transform.rotation = Quaternion.Euler(angle, 0f, 0f);
                break;
            case RampDirection.South:
                ramp.transform.rotation = Quaternion.Euler(-angle, 0f, 0f);
                break;
            case RampDirection.East:
                ramp.transform.rotation = Quaternion.Euler(0f, 0f, -angle);
                break;
            case RampDirection.West:
                ramp.transform.rotation = Quaternion.Euler(0f, 0f, angle);
                break;
        }

        // створюємо small-box зверху пандуса
        Vector3 topBoxPos = ramp.transform.position + new Vector3(0, _smallHeight / 2f + _cellSize / 2f, 0);
        GameObject topBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topBox.name = $"RampSmallBox_{i}_{j}";
        topBox.transform.SetParent(ramp.transform);

        // локальна позиція (поверхня пандуса)
        topBox.transform.localPosition = new Vector3(0, ramp.transform.localScale.y / 2f, 0);

        topBox.transform.localScale = new Vector3(_cellSize, _smallHeight, _cellSize);
        //topBox.transform.position = topBoxPos;
        //topBox.transform.SetParent(parent);
        topBox.tag = Constants.TagConstans.FloorGridTag;
        topBox.GetComponent<Renderer>().material = _gridMaterial;
        topBox.GetComponent<BoxCollider>().isTrigger = true;

        ///!!!
        //TO DO; смол-бокс повернутий наче правлиьно, але він зїхав в сторону і ДУЖЕ великий
    }


}