using System.Collections.Generic;
using UnityEngine;

public class ObstacleFactory
{
    private List<ObstacleOnTheMap> _obstacles;

    private float _cellSize;
    private Transform _parent;
    private float _bigHeight;

    public ObstacleFactory(float cellSize, float bigHeight, Transform parent, List<ObstacleOnTheMap> obstacles)
    {
        _parent = parent;
        _cellSize = cellSize;
        _obstacles = obstacles ?? new();
        _bigHeight = bigHeight;
    }

    /// <summary>
    /// Створюємо перешкоди на карті (будівлі, холми та ін).
    /// </summary>
    /// <param name="totalWidth">ширина карти (в смол-боксах)</param>
    /// <param name="totalLength">довжина карти (в смол-боксах)</param>
    public void CreateObstacle(int totalWidth, int totalLength)
    {
        foreach (ObstacleOnTheMap obstacle in _obstacles)
        {
            if (obstacle.BuildingWidth <= 0 || obstacle.BuildingLength <= 0) continue;

            var obstacleGO = CreateSingleObstacle(obstacle, totalWidth, totalLength, _bigHeight, _parent);
            obstacle.gameObject = obstacleGO;
        }
    }

    /// <summary>
    /// Створює один obstacle + його вкладені (NestedObstacle)
    /// </summary>
    private GameObject CreateSingleObstacle(ObstacleOnTheMap obstacle, int totalWidth, int totalLength, float baseY, Transform parent)
    {
        var gridPos = obstacle.BuildingGridPos;
        var width = obstacle.BuildingWidth;
        var length = obstacle.BuildingLength;
        var height = obstacle.BuildingHeight;

        float bigWidth = totalWidth * _cellSize;
        float bigLength = totalLength * _cellSize;

        float x = -bigWidth / 2f + (gridPos.x + width / 2f) * _cellSize;
        float z = -bigLength / 2f + (gridPos.y + length / 2f) * _cellSize;
        float y = baseY + (height / 2f);   // 👈 база + половина своєї висоти

        Vector3 pos = new(x, y, z);

        GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
        building.name = $"Obstacle_{gridPos.x}_{gridPos.y}";
        building.transform.localScale = new Vector3(width * _cellSize, height, length * _cellSize);
        building.transform.position = pos;
        building.transform.SetParent(parent);

        var material = obstacle.Material;
        if (material != null)
            building.GetComponent<Renderer>().material = material;

        // міняємо парента смол-бокса на Obstacle
        if (obstacle.SmallBoxes != null)
        {
            foreach (var sm in obstacle.SmallBoxes)
            {
                sm.transform.SetParent(building.transform);
            }
        }

        var rb = building.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // 👇 створюємо вкладений obstacle (якщо є)
        if (obstacle.NestedObstacle != null)
        {
            // базова висота зсунулась вгору на висоту цього obstacle
            CreateSingleObstacle(obstacle.NestedObstacle, totalWidth, totalLength, baseY + height, building.transform);
        }

        return building;
    }
}
