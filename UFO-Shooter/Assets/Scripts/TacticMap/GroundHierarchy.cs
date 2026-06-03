using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents all ground levels as Hierarchy structure with it's small boxes and Heroes on it
/// </summary>
public class GroundHierarchy
{
    public List<GroundLayer> GroudLayers { get; set; }

    /// <summary>
    /// Returns the GroundLayer matching the provided GroundHierarchyLevel.
    /// </summary>
    /// <param name="GroundLevel">The target ground hierarchy level to find.</param>
    /// <returns>The matching GroundLayer or null if not found.</returns>
    public GroundLayer GetGroundLayer(GroundHierarchyLevel GroundLevel)
    {
        if (GroudLayers == null || GroudLayers.Count == 0) return null;

        foreach (var layer in GroudLayers)
        {
            if (layer != null && layer.GroundLevel == GroundLevel)
            {
                return layer;
            }
        }

        return null;
    }
}

[Serializable]
public enum GroundHierarchyLevel
{
    /// <summary>
    /// Ground / Main
    /// </summary>
    Level_1 = 1,
    Level_2,
    Level_3,
    Level_4,
    Level_5
}

/// <summary>
/// Represents a single Ground Layer and heroes on it
/// </summary>
public class GroundLayer
{
    /// <summary>
    /// Heroes on th level
    /// </summary>
    public List<Hero> HeroesOnTheLayer = new List<Hero>();

    /// <summary>
    /// Linked other layers with current
    /// </summary>
    public List<GroundLayer> LinkedLayers = null;

    /// <summary>
    /// Small-boxes
    /// </summary>
    public GameObject[,] SmallBoxed => _smallBoxed;

    //misc
    public GroundHierarchyLevel GroundLevel => _groundLayer;
    public GameObject GroundLevelObject => _groundLayerObject;
    public string Name => _name;

    private GroundHierarchyLevel _groundLayer;
    private GameObject _groundLayerObject;
    public string _name { get; set; }
    private GameObject[,] _smallBoxed;

    //TO DO: check to add small boxed here

    public GroundLayer(string name, GameObject groundLayerObject, GameObject[,] smallBoxed, GroundHierarchyLevel groundLayer)
    {
        if (groundLayerObject == null)
        {
            throw new System.Exception("[GroundLayer] groundLevelObject is ('null')");
        }

        _groundLayer = groundLayer;
        _groundLayerObject = groundLayerObject;
        _smallBoxed = smallBoxed;
        _name = name;
    }

    public void AddHero(Hero h)
    {
        if (!HeroesOnTheLayer.Contains(h))
        {
            HeroesOnTheLayer.Add(h);
        }
    }

    public void RemoveHero(Hero h)
    {
        HeroesOnTheLayer.Remove(h);
    }

    public void LinkNewLevel(GroundLayer l)
    {
        if (LinkedLayers is null) LinkedLayers = new List<GroundLayer>();

        if (!LinkedLayers.Contains(l))
        {
            LinkedLayers.Add(l);
        }
    }

    public void UnlinkLayer(GroundLayer l)
    {
        LinkedLayers?.Remove(l);
    }
}