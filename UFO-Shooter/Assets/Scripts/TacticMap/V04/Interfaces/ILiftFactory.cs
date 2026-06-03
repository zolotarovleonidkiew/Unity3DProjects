using System.Collections.Generic;
using UnityEngine;

public interface ILiftFactory
{
    public IEnumerable<GameObject> CreateLifts(
        List<Vector2Int> liftGridPosCollection, //2D координати ліфтів
        GameObject heroesCollectionGUI, //link to heroes
        GameObject[,] smallBoxes,
        float bigHeight = 0.5f,
        Transform parent = null,        //parent platform
        Material liftMaterial = null,
        Vector3? liftSizeNullable = null);   
}