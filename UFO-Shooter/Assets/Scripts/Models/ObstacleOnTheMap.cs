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
    /// Editor obst. name
    /// </summary>
    public string Name;

    public Vector2Int ObstacleGridPos;          // позиція початку будівлі/схила
    public int BuildingWidth;                   // ширина
    public int BuildingLength;                  // довжина
    public float BuildingHeight;                // висота
    public bool NeedToCreateSmallBoxOnTheTop;   // чи треба робити смол-бокси зверху
    public Material Material;    

    /// <summary>
    /// Represents prefab of for the Obstacle (instead of Cube by Default)
    /// </summary>
    public GameObject ObstaclePrefab = null;

    /// <summary>
    /// Represents flag that this obstacle is a tree (for special handling in the code)
    /// </summary>
    public bool IsTree = false;

    /// <summary>
    /// if > 0 then rotating obstacle
    /// </summary>
    public float RotateY = 0;

    //----------------------------------------------------------
    #region Climbing to the top of the obstacle

    /// <summary>
    /// If true, hero can climb to the top of this obstacle
    /// </summary>
    public bool AllowedClimbing; //INSPECTOR

    /*
     Чому необхідні sm_coord_to_START_climbing та sm_coord_to_END_climbing?
        -> на момент створення obstacle, sm_box'и ще не створені, тому не можна посилатися на sm_box'и напряму
     */

    /// <summary>
    /// Coords of Small boxes where hero can start climbing
    /// </summary>
    public Vector2 Climbing_from_coords; //CODE


    /// <summary>
    /// Coords of Small boxes where hero can stops climbing
    /// </summary>
    public Vector2 Climbing_to_coords; //CODE
    #endregion

    //----------------------------------------------------------
    // INTERNAL KITCHEN - DO NOT TOUCH

    /// <summary>
    /// Дочірній obstacle (може бути null)
    /// </summary>
    public ObstacleOnTheMap NestedObstacle { get; set; }

    /// <summary>
    /// Смол-бокси, згенеровані в GridGenerator для конкретного Obstacl'а
    /// </summary>
    public List<GameObject> SmallBoxes { get; set; }

    /// <summary>
    /// Represents GO of model (INTERNAL) - don't modify in editor
    /// </summary>
    public GameObject inetrnalGameObjectObstacleInstance;

    /// <summary>
    /// Ramps collection - filled programmaticaly
    /// </summary>
    public List<GameObject> RampsCollection;
    //----------------------------------------------------------

    public ObstacleOnTheMap(string name, Vector2Int pos, int width, int length, float height, bool needSmallBoxOnTop,
        Material material, ObstacleOnTheMap nestedObstacle = null, bool isTree = false, float rotateY = 0)
    {
        Name = name;
        ObstacleGridPos = pos;
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