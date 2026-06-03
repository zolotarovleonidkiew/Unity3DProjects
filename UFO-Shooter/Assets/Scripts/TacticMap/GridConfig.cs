using System.Collections.Generic;
using UnityEngine;

public class GridConfig
{
    public int Width;
    public int Length;
    public float CellSize;
    public float GroundHeight;
    public float SmallBoxHeight;
    public float LiftOffset; //??

    public Material GridMaterial;
    public Material GroundMaterial;

    public Transform Parent;

    public List<ObstacleOnTheMap> Obstacles;
    public List<Ramp> Ramps;
    public List<Vector2Int> LiftPositions;
}