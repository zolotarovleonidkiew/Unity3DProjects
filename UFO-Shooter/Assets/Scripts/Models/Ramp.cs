using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Заїзд на перешкоду (дім, холм та ін.)
/// </summary>
[Serializable]
public class Ramp
{
    public int i;
    public int j;
    public RampDirection direction;
    public Material material;
    public Vector2Int targetCell;

    ///// <summary>
    ///// Треба для звязку рампи і обстекла для ЗАХОДУ на нього
    ///// </summary>
    //public ObstacleOnTheMap RampLeadsToThisObstacle { get; set; }

    public int obstacleIndex = 0;
    public GameObject GO;


    [HideInInspector]
    public Transform parent;


    //Allowed/Disallowed movements
    public List<Vector2Int> allowedMovements = new List<Vector2Int>();
    public List<Vector2Int> disallowedMovements = new List<Vector2Int>();


    public Ramp(int i, int j, RampDirection direction, Transform parent, Material material, Vector2Int targetCell)
    {
        this.i = i;
        this.j = j;
        this.direction = direction;
        this.parent = parent;
        this.material = material;
        this.targetCell = targetCell;
    }
}

[Serializable]
public enum RampDirection
{
    North, // знизу догори (вздовж осі Z+)
    South, // згори вниз (вздовж осі Z-)
    East,  // зліва направо (вздовж осі X+)
    West   // справа наліво (вздовж осі X-)
}
