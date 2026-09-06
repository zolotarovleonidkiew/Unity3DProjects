using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Проверка и валидация WayPoint'ов и SmallBox'ов на карте.
/// </summary>
public class SmallBoxesValidation
{
    public void ValidateAndFix(IGroundHierarchy createdland, IEnumerable<Ramp> ramps, List<HillOnTheMap> _hills)
    {
        if (createdland?.GroudLayers == null)
        {
            return;
        }

        foreach (var layer in createdland.GroudLayers)
        {
            if (layer is null || layer.SmallBoxed is null) return;

            ValidateLayerSmallBoxed(layer.SmallBoxed);      // OK
            ValidateObstacleSmallBoxed(layer.SmallBoxed);   // OK

            if (_hills is not null)
            {
                ValidateHillsSmallBoxed(layer.SmallBoxed, _hills);
            }            

            ValidateObstacleClimbing(layer.SmallBoxed);     // OK

            if (ramps is not null)
            {
                ValidateRampsSmallBoxed(layer.SmallBoxed, ramps); // OK
            }            
        }
    }

    /// <summary>
    /// STEP - 1 (OK)
    /// </summary>
    private void ValidateLayerSmallBoxed(GameObject[,] smallBoxes)
    {
        for (var x = 0; x < smallBoxes.GetLength(0); x++)
        {
            for (var y = 0; y < smallBoxes.GetLength(1); y++)
            {
                WayPoint wayPoint = GetWayPoint(smallBoxes[x, y]);
                if (wayPoint == null)
                {
                    continue;
                }

                SetDirection(wayPoint, WayPointDirection.Up, x, y + 1, smallBoxes);
                SetDirection(wayPoint, WayPointDirection.Down, x, y - 1, smallBoxes);
                SetDirection(wayPoint, WayPointDirection.Left, x - 1, y, smallBoxes);
                SetDirection(wayPoint, WayPointDirection.Right, x + 1, y, smallBoxes);
            }
        }

    }

    /// <summary>
    /// STEP - 2 (OK)
    /// </summary>
    private void ValidateObstacleSmallBoxed(GameObject[,] smallBoxes)
    {
        for (var x = 0; x < smallBoxes.GetLength(0); x++)
        {
            for (var y = 0; y < smallBoxes.GetLength(1); y++)
            {
                //test
                //if (x == 9 && y == 11)
                //{
                //    var t = 9;
                //}

                var sourceBox = smallBoxes[x, y];
                WayPoint sourceWayPoint = GetWayPoint(sourceBox);
                if (sourceWayPoint == null || IsRampBox(sourceBox))
                {
                    continue;
                }

                if (IsObstacleBox(sourceBox))
                {
                    ValidateTopBoxDirection(sourceWayPoint, WayPointDirection.Up, x, y + 1, smallBoxes);
                    ValidateTopBoxDirection(sourceWayPoint, WayPointDirection.Down, x, y - 1, smallBoxes);
                    ValidateTopBoxDirection(sourceWayPoint, WayPointDirection.Left, x - 1, y, smallBoxes);
                    ValidateTopBoxDirection(sourceWayPoint, WayPointDirection.Right, x + 1, y, smallBoxes);
                    continue;
                }

                BlockObstacleDirection(sourceWayPoint, WayPointDirection.Up, x, y + 1, smallBoxes);
                BlockObstacleDirection(sourceWayPoint, WayPointDirection.Down, x, y - 1, smallBoxes);
                BlockObstacleDirection(sourceWayPoint, WayPointDirection.Left, x - 1, y, smallBoxes);
                BlockObstacleDirection(sourceWayPoint, WayPointDirection.Right, x + 1, y, smallBoxes);
            }
        }
    }

    /// <summary>
    /// STEP - 2A (OK)
    /// </summary>
    private void ValidateHillsSmallBoxed(GameObject[,] smallBoxes, List<HillOnTheMap> hills)
    {
        if (smallBoxes == null || hills == null || hills.Count == 0)
        {
            return;
        }

        for (var x = 0; x < smallBoxes.GetLength(0); x++)
        {
            for (var y = 0; y < smallBoxes.GetLength(1); y++)
            {
                var sourceBox = smallBoxes[x, y];
                var sourceWayPoint = GetWayPoint(sourceBox);
                if (sourceWayPoint == null || !IsObstacleBox(sourceBox))
                {
                    continue;
                }

                var sourceHeight = GetHillSurfaceHeight(new Vector2Int(x, y), hills);
                if (!sourceHeight.HasValue)
                {
                    continue;
                }

                ValidateHillDirection(sourceWayPoint, WayPointDirection.Up, x, y + 1, sourceHeight.Value, smallBoxes, hills);
                ValidateHillDirection(sourceWayPoint, WayPointDirection.Down, x, y - 1, sourceHeight.Value, smallBoxes, hills);
                ValidateHillDirection(sourceWayPoint, WayPointDirection.Left, x - 1, y, sourceHeight.Value, smallBoxes, hills);
                ValidateHillDirection(sourceWayPoint, WayPointDirection.Right, x + 1, y, sourceHeight.Value, smallBoxes, hills);
            }
        }
    }

    /// <summary>
    /// STEP - 3 (OK)
    /// </summary>
    private void ValidateObstacleClimbing(GameObject[,] smallBoxes)
    {
        if (smallBoxes == null)
        {
            return;
        }

        for (var x = 0; x < smallBoxes.GetLength(0); x++)
        {
            for (var y = 0; y < smallBoxes.GetLength(1); y++)
            {
                var sourceWayPoint = GetWayPoint(smallBoxes[x, y]);
                if (sourceWayPoint == null)
                {
                    continue;
                }

                SetClimbingDirection(sourceWayPoint, WayPointDirection.Up, x, y + 1, smallBoxes);
                SetClimbingDirection(sourceWayPoint, WayPointDirection.Down, x, y - 1, smallBoxes);
                SetClimbingDirection(sourceWayPoint, WayPointDirection.Left, x - 1, y, smallBoxes);
                SetClimbingDirection(sourceWayPoint, WayPointDirection.Right, x + 1, y, smallBoxes);
            }
        }
    }

    /// <summary>
    /// STEP - 4 (OK)
    /// </summary>
    private void ValidateRampsSmallBoxed(GameObject[,] smallBoxes, IEnumerable<Ramp> ramps)
    {
        foreach (var ramp in ramps.Where(ramp => ramp != null))
        {
            ApplyRampMovementList(smallBoxes, ramp, ramp.allowedMovements, true);
            ApplyRampMovementList(smallBoxes, ramp, ramp.disallowedMovements, false);
        }
    }

    private static void BlockObstacleDirection(
        WayPoint sourceWayPoint,
        WayPointDirection direction,
        int targetX,
        int targetY,
        GameObject[,] smallBoxes)
    {
        var targetBox = GetBox(smallBoxes, targetX, targetY);
        if (IsObstacleBox(targetBox))
        {
            sourceWayPoint.SetDirectionAllowed(direction, false);
        }
    }

    private static void ValidateTopBoxDirection(
        WayPoint sourceWayPoint,
        WayPointDirection direction,
        int targetX,
        int targetY,
        GameObject[,] smallBoxes)
    {
        sourceWayPoint.SetDirectionAllowed(
            direction,
            IsObstacleBox(GetBox(smallBoxes, targetX, targetY)));
    }

    private static void SetClimbingDirection(
        WayPoint sourceWayPoint,
        WayPointDirection direction,
        int targetX,
        int targetY,
        GameObject[,] smallBoxes)
    {
        var targetWayPoint = GetWayPoint(GetBox(smallBoxes, targetX, targetY));
        if (targetWayPoint == null)
        {
            return;
        }

        var sourceCoords = ToGridCoords(sourceWayPoint.Corrds);
        var targetCoords = ToGridCoords(targetWayPoint.Corrds);
        if (IsClimbingTransition(sourceCoords, targetCoords))
        {
            sourceWayPoint.SetDirectionAllowed(direction, true);
        }
    }

    private static void ApplyRampMovementList(
        GameObject[,] smallBoxes,
        Ramp ramp,
        IEnumerable<Vector2Int> sourceCoordinates,
        bool allowed)
    {
        int rampX; int rampY; int sbX; int sbY;

        if (!TryFindRampCoordinates(smallBoxes, ramp, out rampX, out rampY))
        {
            return;
        }

        foreach (var sourceCoordinate in sourceCoordinates)
        {
            var smallBox = smallBoxes[sourceCoordinate.x, sourceCoordinate.y];

            var wayPoint = GetWayPoint(smallBox);
            var direction = GetDirection2(rampX, rampY, sourceCoordinate.x, sourceCoordinate.y);
            wayPoint?.SetDirectionAllowed(direction, allowed);
        }
    }

    private static bool TryFindRampCoordinates(
        GameObject[,] smallBoxes,
        Ramp ramp,
        out int rampX,
        out int rampY)
    {
        for (var x = 0; x < smallBoxes.GetLength(0); x++)
        {
            for (var y = 0; y < smallBoxes.GetLength(1); y++)
            {
                var wayPoint = GetWayPoint(smallBoxes[x, y]);
                if (IsRampBox(smallBoxes[x, y]) && wayPoint != null &&
                    CoordinatesMatch(ToGridCoords(wayPoint.Corrds), new Vector2Int(ramp.i, ramp.j)))
                {
                    rampX = x;
                    rampY = y;
                    return true;
                }
            }
        }

        rampX = -1;
        rampY = -1;
        return false;
    }

    private static WayPointDirection GetDirection2(int rampX, int rampY, int sbX, int sbY)
    {
        if (rampX == sbX)
        {
            if (sbY > rampY) return WayPointDirection.Down;
            if (sbY < rampY) return WayPointDirection.Up;
        }
        else if (rampY == sbY)
        {
            if (sbX > rampX) return WayPointDirection.Left;
            if (sbX < rampX) return WayPointDirection.Right;
        }
        else
        {
            throw new Exception($"Invalid direction for ramp ({rampX}, {rampY}) and small box ({sbX}, {sbY})");            
        }

        //default
        return WayPointDirection.Down;
    }

    private void SetDirection(
        WayPoint wayPoint,
        WayPointDirection direction,
        int targetX,
        int targetY,
        GameObject[,] smallBoxes)
    {
        wayPoint.SetDirectionAllowed(direction, GetWayPoint(GetBox(smallBoxes, targetX, targetY)) != null);
    }
    private static bool CoordinatesMatch(Vector2Int actual, Vector2Int configured)
    {
        return actual == configured || actual + Vector2Int.one == configured;
    }

    private static WayPoint GetWayPoint(GameObject box)
    {
        return box == null ? null : box.GetComponent<WayPoint>();
    }

    private static GameObject GetBox(GameObject[,] smallBoxes, int x, int y)
    {
        if (x < 0 || y < 0 || x >= smallBoxes.GetLength(0) || y >= smallBoxes.GetLength(1))
        {
            return null;
        }

        return smallBoxes[x, y];
    }

    private static Vector2Int ToGridCoords(Vector2 coords)
    {
        return new Vector2Int(Mathf.RoundToInt(coords.x), Mathf.RoundToInt(coords.y));
    }

    private static bool IsObstacleBox(GameObject box)
    {
        return box != null && box.name.StartsWith("TopSmallBox_");
    }

    private static bool IsRampBox(GameObject box)
    {
        return box != null && box.name.StartsWith("RampTopBox_");
    }

    private static bool IsClimbingTransition(Vector2Int source, Vector2Int target)
    {
        foreach (var obstacle in EnumerateObstacles(StaticTacticalData.Obstacles))
        {
            if (obstacle == null || !obstacle.AllowedClimbing)
            {
                continue;
            }

            var from = ToGridCoords(obstacle.Climbing_from_coords);
            var to = ToGridCoords(obstacle.Climbing_to_coords);

            if (IsSameTransition(source, target, from, to) ||
                IsSameTransition(source, target, from + Vector2Int.one, to + Vector2Int.one) ||
                IsSameTransition(source, target, from - Vector2Int.one, to - Vector2Int.one))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsSameTransition(
        Vector2Int source,
        Vector2Int target,
        Vector2Int from,
        Vector2Int to)
    {
        return (source == from && target == to) ||
               (source == to && target == from);
    }

    private static IEnumerable<ObstacleOnTheMap> EnumerateObstacles(
        IEnumerable<ObstacleOnTheMap> obstacles)
    {
        if (obstacles == null)
        {
            yield break;
        }

        foreach (var obstacle in obstacles)
        {
            foreach (var nested in EnumerateObstacle(obstacle))
            {
                yield return nested;
            }
        }
    }

    private static IEnumerable<ObstacleOnTheMap> EnumerateObstacle(ObstacleOnTheMap obstacle)
    {
        if (obstacle == null)
        {
            yield break;
        }

        yield return obstacle;

        foreach (var nested in EnumerateObstacle(obstacle.NestedObstacle))
        {
            yield return nested;
        }
    }

    private static void ValidateHillDirection(
    WayPoint sourceWayPoint,
    WayPointDirection direction,
    int targetX,
    int targetY,
    float sourceHeight,
    GameObject[,] smallBoxes,
    List<HillOnTheMap> hills)
    {
        var targetBox = GetBox(smallBoxes, targetX, targetY);
        if (!IsObstacleBox(targetBox))
        {
            sourceWayPoint.SetDirectionAllowed(direction, false);
            return;
        }

        var targetHeight = GetHillSurfaceHeight(new Vector2Int(targetX, targetY), hills);
        sourceWayPoint.SetDirectionAllowed(
            direction,
            targetHeight.HasValue && Mathf.Approximately(sourceHeight, targetHeight.Value));
    }

    private static float? GetHillSurfaceHeight(Vector2Int cell, List<HillOnTheMap> hills)
    {
        var isOneBased = DetectOneBasedHillCoordinates(hills);
        float? highestSurface = null;

        for (var index = 0; index < hills.Count; index++)
        {
            var hill = hills[index];
            if (hill == null || !IsCellInsideHill(cell, hill, isOneBased))
            {
                continue;
            }

            var surfaceHeight = GetHillHeight(index, hills);
            if (!highestSurface.HasValue || surfaceHeight > highestSurface.Value)
            {
                highestSurface = surfaceHeight;
            }
        }

        return highestSurface;
    }

    private static float GetHillHeight(int index, List<HillOnTheMap> hills)
    {
        var hill = hills[index];
        var height = hill.Height;

        if (index > 0 && hills[index - 1] != null && hills[index - 1].NextHillIsNestedForCurrentHill)
        {
            height += GetHillHeight(index - 1, hills);
        }

        return height;
    }

    private static bool IsCellInsideHill(Vector2Int cell, HillOnTheMap hill, bool isOneBased)
    {
        var startX = hill.Position2D.x - (isOneBased ? 1 : 0);
        var startY = hill.Position2D.y - (isOneBased ? 1 : 0);

        return cell.x >= startX &&
               cell.x < startX + hill.Width &&
               cell.y >= startY &&
               cell.y < startY + hill.Length;
    }

    private static bool DetectOneBasedHillCoordinates(List<HillOnTheMap> hills)
    {
        foreach (var hill in hills)
        {
            if (hill != null && (hill.Position2D.x == 0 || hill.Position2D.y == 0))
            {
                return false;
            }
        }

        return true;
    }
}