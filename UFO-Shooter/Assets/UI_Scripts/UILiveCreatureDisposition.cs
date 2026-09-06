using System;
using UnityEngine;

/// <summary>
/// ”казывает уровень (layer) на котором находитс€ живой персонаж (герой или чужой) и его позицию на гриде. 
/// Ёто нужно дл€ правильного отображени€ персонажа на уровне земли, лифта или верхнего уровн€.
/// </summary>
[Serializable]
public class UILiveCreatureDisposition
{
    public Vector2Int GridPosition;
    public AlienTypesEnum alienTypesEnum; 
    public GroundHierarchyLevel GroundLayer;

    /// <summary>
    /// Heros and aliens prefabs here - should be removed
    /// </summary>
    public GameObject CreaturePrefab = null;
}