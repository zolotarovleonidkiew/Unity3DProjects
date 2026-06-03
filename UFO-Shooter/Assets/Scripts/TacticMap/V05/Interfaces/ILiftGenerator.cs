using Assets.Scripts.TacticMap;
using System.Collections.Generic;
using UnityEngine;

public interface ILiftGenerator
{
    public IEnumerable<GameObject> GenerateLifts(LiftConfig liftConfig,
        GameObject[,] groundSmallBoxes,
        IDictionary<Vector2Int, GroundLayer> liftLayers,
        GameObject heroesCollectionGUI);
}