using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Service responsible for highlighting available moves for the hero on the grid.
/// </summary>
public class HighlightAvailableMovesService
{
    private readonly CreatureMovingService creatureMovingService = new CreatureMovingService();

    public void HighlightAvailableMoves(
        BackgroundGenerationScript groundObject,
        Renderer[,] cellRenderers,
        Vector2Int heroCoords,
        int movementPoints,
        Material highlightMaterial,
        IGroundHierarchy groundHierarchy,
        GroundHierarchyLevel currentLevel)
    {
        if (groundObject == null || cellRenderers == null) return;

        int w = groundObject.width;
        int l = groundObject.length;
        var reachableWayPoints = groundHierarchy == null
            ? new HashSet<WayPoint>()
            : creatureMovingService.GetReachableWayPoints(
                groundHierarchy,
                currentLevel,
                heroCoords,
                movementPoints);

        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < l; j++)
            {
                var rend = cellRenderers[i, j];
                if (rend == null) continue;

                var smallBox = GridGenerator_05.GetSmallCube(i, j);
                var wayPoint = smallBox == null ? null : smallBox.GetComponent<WayPoint>();
                bool isAvailable = wayPoint != null && reachableWayPoints.Contains(wayPoint);

                if (isAvailable)
                {
                    rend.enabled = true;
                    if (highlightMaterial != null)
                        rend.material = highlightMaterial;
                }
                else
                {
                    rend.enabled = false;
                }
            }
        }
    }
}