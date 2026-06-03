using UnityEngine;
using static ObstacleFactory;

public interface IObstacleGenerator
{
    /// <summary>
    /// Creates obstacles and ramps for {root}
    /// </summary>
    void CreateObstaclesAndRamps(GridConfig config, GameObject root, GameObject[,] smallBoxes, ObstacleConfig obstacleConfig);
}