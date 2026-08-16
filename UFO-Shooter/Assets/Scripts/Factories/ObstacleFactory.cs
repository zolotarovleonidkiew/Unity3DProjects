using Assets.Scripts.TacticMap.V04;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleFactory : IObstacleFactory
{
    public const string Algorith_version = "0.4";

    public void CreateObstacle(ObstacleConfig obstacleConfig)
    {
        foreach (ObstacleOnTheMap obstacle in obstacleConfig.Obstacles)
        {
            if (obstacle.BuildingWidth <= 0 || obstacle.BuildingLength <= 0) continue;

            var obstacleGO = CreateSingleObstacle(obstacle, obstacleConfig, obstacleConfig.PlatformHeight, obstacleConfig.Parent);
            obstacle.inetrnalGameObjectObstacleInstance = obstacleGO;
        }
    }

    /// <summary>
    /// Створює один obstacle + його вкладені (NestedObstacle)
    /// </summary>
    private GameObject CreateSingleObstacle(ObstacleOnTheMap obstacle, ObstacleConfig obstacleConfig, 
        float baseY, Transform parent)
    {
        var cellSize = obstacleConfig.CellSize;
        var totalWidth = obstacleConfig.TotalWidth;
        var totalLength = obstacleConfig.TotalLength;

        var gridPos = obstacle.BuildingGridPos;
        var width = obstacle.BuildingWidth;
        var length = obstacle.BuildingLength;
        var height = obstacle.BuildingHeight;

        float bigWidth = totalWidth * cellSize;
        float bigLength = totalLength * cellSize;

        float x = -bigWidth / 2f + (gridPos.x + width / 2f) * cellSize;
        float z = -bigLength / 2f + (gridPos.y + length / 2f) * cellSize;
        float y = baseY + (height / 2f);   // 👈 база + половина своєї висоти

        //move to REAL world corrdinates in small-box (''2 is lengtm of SM)
        x -= obstacleConfig.CellSize; // '-2'
        z -= obstacleConfig.CellSize;

        Vector3 pos = new(x, y, z);

        GameObject building;

        // If obstacle provides a prefab, instantiate it; otherwise create a cube primitive
        if (obstacle.buildingPrefab != null)
        {
            //prefab oveerride position:
            pos.y = baseY; // 👈 база (не половина своєї висоти, бо prefab вже має свою висоту) //baseY
            pos.z += 1; // 👈 щоб prefab не перекривався з SM (бо SM має довжину 2)     // 2 ?
            pos.x += 1.5f; // 👈 щоб prefab не перекривався з SM (бо SM має довжину 2)  // 2 ?

            building = Object.Instantiate(obstacle.buildingPrefab, pos, Quaternion.identity, parent);

            if (obstacle.Name is null || obstacle.Name == "")
                building.name = $"Building-Obstacle_{gridPos.x}_{gridPos.y}";
            else
                building.name = obstacle.Name;

            // keep prefab's original scale to avoid unexpected stretching
        }
        else
        {
            building = GameObject.CreatePrimitive(PrimitiveType.Cube);
            if (obstacle.Name is null || obstacle.Name == "")
            {
                building.name = $"Raw-Obstacle_{gridPos.x}_{gridPos.y}";
            }
            building.transform.localScale = new Vector3(width * cellSize, height, length * cellSize);
            building.transform.position = pos;
            building.transform.SetParent(parent);

            var material = obstacle.Material;
            if (material != null)
                building.GetComponent<Renderer>().material = material;
        }

        // міняємо парента смол-бокса на Obstacle
        if (obstacle.SmallBoxes != null)
        {
            foreach (var sm in obstacle.SmallBoxes)
            {
                sm.transform.SetParent(building.transform);
            }
        }

        // Ensure Rigidbody exists but don't duplicate if prefab already has one
        var rb = building.GetComponent<Rigidbody>();
        if (rb == null)
            rb = building.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        // 👇 створюємо вкладений obstacle (якщо є)
        if (obstacle.NestedObstacle != null)
        {
            CreateSingleObstacle(obstacle.NestedObstacle, obstacleConfig, baseY + height, 
                building.transform);
        }

        return building;
    }

    /// <summary>
    /// Parameters for ObstacleFactory
    /// </summary>
    public class ObstacleConfig
    {
        public float CellSize { get; set; }
        public float PlatformHeight { get; set; }
        public Transform Parent { get; set; }
        public int TotalWidth { get; set; }
        public int TotalLength { get; set; }
        public List<ObstacleOnTheMap> Obstacles { get; set; }
        public Material Level1_ObstacleMaterial { get; set; }
        public Material Level2_ObstacleMaterial { get; set; }
        public Material Level3_ObstacleMaterial { get; set; }
    }
}