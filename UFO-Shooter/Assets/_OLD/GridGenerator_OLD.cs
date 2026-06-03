//using System.Collections.Generic;
//using UnityEngine;

////TO DO: смол-бокси мають мати парента Обстакл, а не бігБог

////NEW
///// <summary>
///// Movement/platform Grid generator
///// </summary>
//public class GridGenerator_OLD
//{
//    private int _width;
//    private int _length;
//    private float _cellSize;
//    private float _bigHeight;
//    private float _smallHeight;
//    private float _liftAboveBig;
//    private Material _gridMaterial;
//    private Material _groundMaterial;
//    private Transform _parent;

//    // Перешкоди (будівлі)
//    private List<ObstacleOnTheMap> _obstacles = new();

//    // lift
//    private Vector2Int? _liftGridPos;
//    private LiftFactory _liftFactory;

//    private GameObject[,] _smallCubes;
//    private GameObject _bigBox;
//    private GameObject HeroesCollectionGUI;

//    public GridGenerator_OLD(
//        GameObject heroesCollectionGUI,
//        int width, int length,
//        float cellSize, float bigHeight, float smallHeight, float liftAboveBig,
//        Material gridMaterial, Material groundMaterial,
//        Transform parent,
//        List<ObstacleOnTheMap> obstacles = null,
//        Vector2Int? liftGridPos = null, LiftFactory liftFactory = null
//    )
//    {
//        _width = width;
//        _length = length;
//        _cellSize = cellSize;
//        _bigHeight = bigHeight;
//        _smallHeight = smallHeight;
//        _liftAboveBig = liftAboveBig;
//        _gridMaterial = gridMaterial;
//        _groundMaterial = groundMaterial;
//        _parent = parent;

//        HeroesCollectionGUI = heroesCollectionGUI;

//        if (obstacles != null && obstacles.Count > 0)
//            _obstacles = obstacles;

//        if (liftGridPos.HasValue)
//        {
//            _liftGridPos = liftGridPos;
//            _liftFactory = liftFactory ?? new LiftFactory(HeroesCollectionGUI);
//        }
//    }

//    /// <summary>
//    /// Створює великий куб + дрібні клітинки
//    /// </summary>
//    public GameObject[,] GenerateGrid(float? bigHeight = null, Vector3? startPosition = null)
//    {
//        // розрахунок розмірів
//        float bigWidth = _width * _cellSize;
//        float bigLength = _length * _cellSize;

//        // великий куб (основа)
//        _bigBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        _bigBox.name = $"BigBox_{bigWidth}x{_bigHeight}x{bigLength}";
//        _bigBox.transform.localScale = new Vector3(bigWidth, _bigHeight, bigLength);
//        bigHeight = bigHeight ?? _bigHeight / 2f;
//        startPosition = startPosition ?? new Vector3(0f, bigHeight.Value, 0f);
//        _bigBox.transform.position = startPosition.Value;
//        _bigBox.transform.SetParent(_parent);
//        _bigBox.GetComponent<Renderer>().material = _groundMaterial;

//        var rb = _bigBox.AddComponent<Rigidbody>();
//        rb.isKinematic = true;
//        rb.useGravity = false;

//        // масив клітинок
//        _smallCubes = new GameObject[_width, _length];
//        Vector3 center = _bigBox.transform.position;

//        for (int i = 0; i < _width; i++)
//        {
//            for (int j = 0; j < _length; j++)
//            {
//                // 🔹 якщо тут є перешкода
//                ObstacleOnTheMap obstacle = GetObstacleAt(i, j);
//                if (obstacle != null)
//                {
//                    if (!obstacle.NeedToCreateSmallBoxOnTheTop)
//                        continue; // не створюємо smallBox

//                    // створюємо смол-бокс зверху будівлі
//                    float xTop = -bigWidth / 2f + (i + 0.5f) * _cellSize;
//                    float zTop = -bigLength / 2f + (j + 0.5f) * _cellSize;

//                    Vector3 topPos = new Vector3(
//                        center.x + xTop,
//                        //bigHeight.Value + _liftAboveBig + _smallHeight, // над будівлею
//                        obstacle.BuildingHeight + _liftAboveBig + _smallHeight, //to do
//                        center.z + zTop
//                    );

//                    GameObject topBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                    topBox.name = $"TopSmallBox_{i}_{j}";
//                    topBox.transform.localScale = new Vector3(_cellSize, _smallHeight, _cellSize);
//                    topBox.transform.position = topPos;
//                    topBox.transform.SetParent(_bigBox.transform);
//                    topBox.tag = Constants.TagConstans.FloorGridTag;
//                    topBox.GetComponent<Renderer>().material = _gridMaterial;
//                    topBox.GetComponent<BoxCollider>().isTrigger = true;

//                    _smallCubes[i, j] = topBox;
//                    continue;
//                }

//                // 🔹 якщо це клітинка для ліфта
//                float x = -bigWidth / 2f + (i + 0.5f) * _cellSize;
//                float z = -bigLength / 2f + (j + 0.5f) * _cellSize;
//                Vector3 smallPos = new Vector3(
//                    center.x + x,
//                    bigHeight.Value + _liftAboveBig,
//                    center.z + z
//                );

//                if (_liftGridPos.HasValue && i == _liftGridPos.Value.x && j == _liftGridPos.Value.y)
//                {
//                    smallPos.y = bigHeight.Value * 2 + _liftAboveBig;
//                }

//                GameObject smallBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
//                smallBox.name = $"SmallBox_{i}_{j}";
//                smallBox.transform.localScale = new Vector3(_cellSize, _smallHeight, _cellSize);
//                smallBox.transform.position = smallPos;
//                smallBox.transform.SetParent(_bigBox.transform);
//                smallBox.tag = Constants.TagConstans.FloorGridTag;
//                smallBox.GetComponent<Renderer>().material = _gridMaterial;
//                smallBox.GetComponent<BoxCollider>().isTrigger = true;

//                _smallCubes[i, j] = smallBox;
//            }
//        }

//        return _smallCubes;
//    }

//    public GameObject GetSmallCube(int i, int j)
//    {
//        if (_smallCubes == null) return null;
//        if (i < 0 || j < 0 || i >= _smallCubes.GetLength(0) || j >= _smallCubes.GetLength(1)) return null;
//        return _smallCubes[i, j];
//    }

//    public bool GetGridCoordsFromWorld(Vector3 worldPos, out int i, out int j)
//    {
//        i = -1;
//        j = -1;
//        float minDist = float.MaxValue;

//        for (int x = 0; x < _width; x++)
//        {
//            for (int z = 0; z < _length; z++)
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

//    ///// <summary>
//    ///// Чи є в даній клітинці перешкода
//    ///// </summary>
//    //private ObstacleOnTheMap GetObstacleAt(int i, int j)
//    //{
//    //    foreach (var obs in _obstacles)
//    //    {
//    //        //  i >= _buildingGridPos.x    && i < _buildingGridPos.x + _buildingWidth &&
//    //        //  j >= _buildingGridPos.y    && j < _buildingGridPos.y + _buildingLength;
//    //        if (i >= obs.BuildingGridPos.x && i < obs.BuildingGridPos.x + obs.BuildingWidth &&
//    //            j >= obs.BuildingGridPos.y && j < obs.BuildingGridPos.y + obs.BuildingLength)
//    //        {
//    //            return obs;
//    //        }
//    //    }
//    //    return null;
//    //}

//    /// <summary>
//    /// Повертає перешкоду, яка покриває клітинку (i,j), або null.
//    /// Явно використовує BuildingWidth для осі X (i) і BuildingLength для осі Z (j).
//    /// </summary>
//    private ObstacleOnTheMap GetObstacleAt(int i, int j)
//    {
//        if (_obstacles == null) return null;

//        foreach (var obs in _obstacles)
//        {
//            int startX = obs.BuildingGridPos.x;
//            int startZ = obs.BuildingGridPos.y;

//            int endX = startX + obs.BuildingWidth - 1;   // останній індекс по X
//            int endZ = startZ + obs.BuildingLength - 1;  // останній індекс по Z

//            if (i >= startX && i <= endX && j >= startZ && j <= endZ)
//                return obs;
//        }

//        return null;
//    }
//}

//////OLD
/////// <summary>
/////// Movement/platform Grid generator
/////// </summary>
////public class GridGenerator
////{
////    private int _width;
////    private int _length;
////    private float _cellSize;
////    private float _bigHeight;
////    private float _smallHeight;
////    private float _liftAboveBig;
////    private Material _gridMaterial;
////    private Material _groundMaterial;
////    private Transform _parent;

////    // building area
////    private Vector2Int _buildingGridPos;
////    private int _buildingWidth;
////    private int _buildingLength;
////    private bool _hasBuildingArea = false;

////    // lift
////    private Vector2Int? _liftGridPos;
////    private LiftFactory _liftFactory;

////    private GameObject[,] _smallCubes;
////    private GameObject _bigBox;

////    private GameObject HeroesCollectionGUI;

////    public GridGenerator(
////        GameObject heroesCollectionGUI,
////        int width, int length,
////        float cellSize, float bigHeight, float smallHeight, float liftAboveBig,
////        Material gridMaterial, Material groundMaterial,
////        Transform parent,
////        Vector2Int? buildingGridPos = null, int buildingWidth = 0, int buildingLength = 0,
////        Vector2Int? liftGridPos = null, LiftFactory liftFactory = null
////    )
////    {
////        _width = width;
////        _length = length;
////        _cellSize = cellSize;
////        _bigHeight = bigHeight;
////        _smallHeight = smallHeight;
////        _liftAboveBig = liftAboveBig;
////        _gridMaterial = gridMaterial;
////        _groundMaterial = groundMaterial;
////        _parent = parent;

////        HeroesCollectionGUI = heroesCollectionGUI;

////        if (buildingGridPos.HasValue && buildingWidth > 0 && buildingLength > 0)
////        {
////            _buildingGridPos = buildingGridPos.Value;
////            _buildingWidth = buildingWidth;
////            _buildingLength = buildingLength;
////            _hasBuildingArea = true;
////        }

////        if (liftGridPos.HasValue)
////        {
////            _liftGridPos = liftGridPos;
////            _liftFactory = liftFactory ?? new LiftFactory(HeroesCollectionGUI);
////        }
////    }

////    /// <summary>
////    /// Створює великий куб + дрібні клітинки
////    /// </summary>
////    public GameObject[,] GenerateGrid(float? bigHeight = null, Vector3? startPosition = null)
////    {
////        // розрахунок розмірів
////        float bigWidth = _width * _cellSize;
////        float bigLength = _length * _cellSize;

////        // великий куб (основа)
////        _bigBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
////        _bigBox.name = $"BigBox_{bigWidth}x{_bigHeight}x{bigLength}";
////        _bigBox.transform.localScale = new Vector3(bigWidth, _bigHeight, bigLength);
////         bigHeight = bigHeight ?? _bigHeight / 2f;
////         startPosition = startPosition ?? new Vector3(0f, bigHeight.Value, 0f);
////        _bigBox.transform.position = startPosition.Value;
////        _bigBox.transform.SetParent(_parent);
////        _bigBox.GetComponent<Renderer>().material = _groundMaterial;

////        var rb = _bigBox.AddComponent<Rigidbody>();
////        rb.isKinematic = true;
////        rb.useGravity = false;

////        // масив клітинок
////        _smallCubes = new GameObject[_width, _length];

////        Vector3 center = _bigBox.transform.position;

////        for (int i = 0; i < _width; i++)
////        {
////            for (int j = 0; j < _length; j++)
////            {
////                if (i == 1-1 && j == 24-1)
////                {
////                    var t = 5;
////                }
////                // Якщо клітинка лежить під будинком — пропускаємо створення smallBox
////                if (IsInsideHouseArea(i, j))
////                    continue;

////                float x = -bigWidth / 2f + (i + 0.5f) * _cellSize;
////                float z = -bigLength / 2f + (j + 0.5f) * _cellSize;

////                Vector3 smallPos = new Vector3(
////                    center.x + x,
////                    //_bigHeight + _liftAboveBig,  //old
////                    bigHeight.Value + _liftAboveBig, //new
////                    center.z + z
////                );

////                //для ліфта костиль - щоб вирівняти смол-бокси другого поверху і платформу ліфта
////                if (_liftGridPos.HasValue && i == _liftGridPos.Value.x && j == _liftGridPos.Value.y)
////                {
////                    //var t = 5;
////                    smallPos.y = bigHeight.Value * 2 + _liftAboveBig;
////                }
////                // 🔹 Якщо це клітинка ліфта
////                //тут НЕ правильна платформа створюється!
////                //if (_liftGridPos.HasValue && i == _liftGridPos.Value.x && j == _liftGridPos.Value.y)
////                //{
////                //    GameObject lift = _liftFactory.CreateLift(smallPos, _bigBox.transform);
////                //    _smallCubes[i, j] = lift;
////                //    continue; // smallBox не створюємо
////                //}

////                // 🔹 Інакше — звичайний smallBox
////                GameObject smallBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
////                smallBox.name = $"SmallBox_{i}_{j}";
////                smallBox.transform.localScale = new Vector3(_cellSize, _smallHeight, _cellSize);
////                smallBox.transform.position = smallPos;
////                smallBox.transform.SetParent(_bigBox.transform);
////                smallBox.tag = Constants.TagConstans.FloorGridTag;
////                smallBox.GetComponent<Renderer>().material = _gridMaterial;
////                BoxCollider collider = smallBox.GetComponent<BoxCollider>();
////                collider.isTrigger = true;

////                _smallCubes[i, j] = smallBox;
////            }
////        }

////        return _smallCubes;
////    }

////    public GameObject GetSmallCube(int i, int j)
////    {
////        if (_smallCubes == null) return null;
////        if (i < 0 || j < 0 || i >= _smallCubes.GetLength(0) || j >= _smallCubes.GetLength(1)) return null;
////        return _smallCubes[i, j];
////    }

////    public bool GetGridCoordsFromWorld(Vector3 worldPos, out int i, out int j)
////    {
////        i = -1;
////        j = -1;
////        float minDist = float.MaxValue;

////        for (int x = 0; x < _width; x++)
////        {
////            for (int z = 0; z < _length; z++)
////            {
////                var cube = GetSmallCube(x, z);
////                if (cube == null) continue;

////                float dist = Vector3.Distance(worldPos, cube.transform.position);
////                if (dist < minDist)
////                {
////                    minDist = dist;
////                    i = x;
////                    j = z;
////                }
////            }
////        }
////        return (i >= 0 && j >= 0);
////    }

////    private bool IsInsideHouseArea(int i, int j)
////    {
////        if (!_hasBuildingArea) return false;

////        return i >= _buildingGridPos.x && i < _buildingGridPos.x + _buildingWidth &&
////               j >= _buildingGridPos.y && j < _buildingGridPos.y + _buildingLength;
////    }
////}
