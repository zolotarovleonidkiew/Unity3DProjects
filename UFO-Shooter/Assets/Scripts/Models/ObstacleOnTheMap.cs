using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Перешкода на карті
/// 
/// TO DO: rename props to 'Obstacle'
/// </summary>
[Serializable]
public class ObstacleOnTheMap
{
    /// <summary>
    /// Editor only name for better readability in inspector; not used in code
    /// </summary>
    public string Name { get; set; }

    public Vector2Int BuildingGridPos;          // позиція початку будівлі
    public int BuildingWidth;                   // ширина
    public int BuildingLength;                  // довжина
    public float BuildingHeight;                // висота
    public bool NeedToCreateSmallBoxOnTheTop;   // чи треба робити смол-бокси зверху
    public Material Material;
    public List<GameObject> RampsCollection; //Added ramp

    /// <summary>
    /// Дочірній obstacle (може бути null)
    /// </summary>
    public ObstacleOnTheMap NestedObstacle { get; set; }

    /// <summary>
    /// Смол-бокси, згенеровані в GridGenerator для конкретного Obstacl'а
    /// </summary>
    public List<GameObject> SmallBoxes { get; set; }

    /// <summary>
    /// Represents GO of model (INTERNAL)
    /// </summary>
    public GameObject inetrnalGameObjectObstacleInstance; //don't modify in editor

    /// <summary>
    /// Represents prefab of for the Building (instead of Cube by Default)
    /// </summary>
    public GameObject buildingPrefab = null;

    /// <summary>
    /// Represents flag that this obstacle is a tree (for special handling in the code)
    /// </summary>
    public bool IsTree = false;

    /// <summary>
    /// if > 0 then rotating obstacle
    /// </summary>
    public float RotateY = 0;

    public ObstacleOnTheMap(string name, Vector2Int pos, int width, int length, float height, bool needSmallBoxOnTop,
        Material material, ObstacleOnTheMap nestedObstacle = null, bool isTree = false, float rotateY = 0)
    {
        Name = name;
        BuildingGridPos = pos;
        BuildingWidth = width;
        BuildingLength = length;
        BuildingHeight = height;
        NeedToCreateSmallBoxOnTheTop = needSmallBoxOnTop;
        Material = material;
        NestedObstacle = nestedObstacle;
        IsTree = isTree;
        RotateY = rotateY;

        //obstacles only
        SmallBoxes = new List<GameObject>();

        RampsCollection = new List<GameObject>();
    }
}