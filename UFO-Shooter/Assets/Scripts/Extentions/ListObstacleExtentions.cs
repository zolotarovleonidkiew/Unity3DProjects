using System.Collections.Generic;

public static class ListObstacleExtentions
{
    public static List<ObstacleOnTheMap> FlatternNestedObstacles(this List<ObstacleOnTheMap> obstacles)
    {
        var result = new List<ObstacleOnTheMap>();

        foreach (ObstacleOnTheMap obs in obstacles)
        {
            GetNestedObstacle(obs, result);
        }

        return result;
    }

    private static void GetNestedObstacle(ObstacleOnTheMap obs, List<ObstacleOnTheMap> result)
    {
        result.Add(obs);

        if (obs.NestedObstacle != null)
        {
            GetNestedObstacle(obs.NestedObstacle, result);
        }
    }
}
