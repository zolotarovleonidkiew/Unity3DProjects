using System;
using UnityEngine;

/// <summary>
/// Как вспомогательный элемент для ObstacleOnTheMap, не более. 
/// [Inspector]
/// </summary>
[Serializable]
public class HillOnTheMap
{
    public string Name;
    public Vector2Int Position2D;
    public int Width;
    public int Length;
    public float Height;
    public bool NeedToCreateSmallBoxOnTheTop;
    public Material Material;
    public bool NextHillIsNestedForCurrentHill = false;
}