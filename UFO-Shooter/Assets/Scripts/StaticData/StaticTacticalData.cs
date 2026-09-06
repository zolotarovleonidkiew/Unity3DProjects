using System.Collections.Generic;

/// <summary>
/// TEST-ONLY static data for tactical map
/// </summary>
public static class StaticTacticalData
{
    public static IGroundHierarchy GroundHierarchy = null;
    public static List<ObstacleOnTheMap> Obstacles = null;

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    //copied from : GridGenerator

    public static ObstacleOnTheMap GetObstacleAt(int i, int j)
    {
        if (Obstacles == null) return null;

        foreach (var obs in Obstacles)
        {
            var found = GetObstacleAtRecursive(obs, i, j);
            if (found != null)
                return found;
        }
        return null;
    }

    public static ObstacleOnTheMap GetObstacleAtRecursive(ObstacleOnTheMap obs, int i, int j)
    {
        int startX = obs.ObstacleGridPos.x;
        int startZ = obs.ObstacleGridPos.y;

        int endX = startX + obs.BuildingWidth - 1;
        int endZ = startZ + obs.BuildingLength - 1;

        if (i >= startX && i <= endX && j >= startZ && j <= endZ)
        {
            // 🔹 якщо є NestedObstacle, треба перевірити спочатку його
            if (obs.NestedObstacle != null)
            {
                var nestedFound = GetObstacleAtRecursive(obs.NestedObstacle, i, j);
                if (nestedFound != null)
                    return nestedFound; // повертаємо найглибший
            }
            return obs; // якщо немає вкладеного → цей obstacle і є результат
        }

        return null;
    }

    //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}