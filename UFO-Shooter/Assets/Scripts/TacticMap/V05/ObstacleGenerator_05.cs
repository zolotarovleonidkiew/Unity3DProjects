using Assets.Scripts.TacticMap.V04;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ObstacleFactory;

public class ObstacleGenerator_05 : IObstacleGenerator
{
    public const string Algorith_version = "0.5";

    public IObstacleFactory obstacleFactory = new ObstacleFactory();

    private const float SmallBoxHeight = 0.5f;
    private const float LiftAboveGround = 0.3f;
    private const float bigHeight = 0.5f;

    public void CreateObstaclesAndRamps(GridConfig config, GameObject layer, GameObject[,] layerSmallBoxes, ObstacleConfig obstacleConfig)
    {
        GenerateHillsAndClimbs(obstacleConfig);

        CreateObstacles(config, layer, layerSmallBoxes);

        obstacleFactory.CreateObstacle(obstacleConfig);

        CreateRamps(config, layer, layerSmallBoxes);
        var flatternObstacles = config.Obstacles.FlatternNestedObstacles();
        foreach (Ramp r in config.Ramps)
        {
            var obst = flatternObstacles[r.obstacleIndex];
            obst.RampsCollection.Add(r.GO);
        }
    }

    private void CreateObstacles(GridConfig config, GameObject layer, GameObject[,] layerSmallBoxes)
    {
        bool coordsOneBased = DetectCoordsOneBased(config);
        if (coordsOneBased)
            Debug.Log("[ObstacleGenerator_05] Detected obstacle coords appear 1-based -> mapping will subtract 1");

        // Prepare mapping from each grid cell -> deepest ObstacleOnTheMap that covers it.
        var cellObstacle = new ObstacleOnTheMap[config.Width, config.Length];

        // Recursive fill: nested obstacles processed first so nested takes precedence.
        void FillMappingRecursive(ObstacleOnTheMap obs)
        {
            if (obs == null) return;

            if (obs.NestedObstacle != null)
                FillMappingRecursive(obs.NestedObstacle);

            int rawStartX = obs.BuildingGridPos.x;
            int rawStartZ = obs.BuildingGridPos.y;
            int startX = rawStartX - (coordsOneBased ? 1 : 0);
            int startZ = rawStartZ - (coordsOneBased ? 1 : 0);

            int endX = startX + obs.BuildingWidth - 1;
            int endZ = startZ + obs.BuildingLength - 1;

            for (int xi = startX; xi <= endX; xi++)
            {
                for (int zj = startZ; zj <= endZ; zj++)
                {
                    if (xi < 0 || zj < 0 || xi >= config.Width || zj >= config.Length) continue;
                    // don't overwrite if nested obstacle already set this cell
                    if (cellObstacle[xi, zj] == null)
                        cellObstacle[xi, zj] = obs;
                }
            }
        }

        foreach (var topObs in config.Obstacles)
            FillMappingRecursive(topObs);

        // масив клітинок
        float bigWidth = config.Width * config.CellSize;
        float bigLength = config.Length * config.CellSize;
        Vector3 center = layer.transform.position;

        for (int i = 0; i < config.Width; i++)
        {
            for (int j = 0; j < config.Length; j++)
            {
                var obstacle = cellObstacle[i, j];
                if (obstacle != null)
                {
                    // delete underlying small-box at exact [i,j]
                    if (layerSmallBoxes[i, j] != null)
                    {
                        GameObject.Destroy(layerSmallBoxes[i, j]);
                        layerSmallBoxes[i, j] = null;
                    }

                    // optionally create top-box on obstacle at the same grid cell center
                    if (!obstacle.NeedToCreateSmallBoxOnTheTop)
                        continue;

                    float xTop = -bigWidth / 2f + (i + 0.5f) * config.CellSize;
                    float zTop = -bigLength / 2f + (j + 0.5f) * config.CellSize;

                    Vector3 topPos = new Vector3(
                        center.x + xTop,
                        GetTotalHeight(config.Obstacles, obstacle) + LiftAboveGround + SmallBoxHeight,
                        center.z + zTop
                    );

                    GameObject topBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    topBox.name = $"TopSmallBox_{i}_{j}";
                    topBox.transform.localScale = new Vector3(config.CellSize, SmallBoxHeight, config.CellSize);
                    topBox.transform.position = topPos;
                    topBox.transform.SetParent(layer.transform);
                    topBox.tag = Constants.TagConstans.FloorGridTag;
                    var topRend = topBox.GetComponent<Renderer>();
                    topRend.material = config.GroundMaterial;
                    topRend.enabled = false; // hide visuals, keep collider
                    topBox.GetComponent<BoxCollider>().isTrigger = true;

                    if (obstacle.SmallBoxes is null)
                    {
                        obstacle.SmallBoxes = new();
                    }
                    obstacle.SmallBoxes.Add(topBox);

                    layerSmallBoxes[i, j] = topBox;
                    continue;
                }

                // 🔹 otherwise create regular small-box (or lift-cell)
                float x = -bigWidth / 2f + (i + 0.5f) * config.CellSize;
                float z = -bigLength / 2f + (j + 0.5f) * config.CellSize;
                Vector3 smallPos = new Vector3(
                    center.x + x,
                    bigHeight + LiftAboveGround,
                    center.z + z
                );

                if (config.LiftPositions != null && config.LiftPositions.Any(l => l.x == i && l.y == j))
                {
                    smallPos.y = bigHeight * 2 + LiftAboveGround;
                }

                GameObject sb = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sb.name = $"SmallBox_{i}_{j}";
                sb.transform.localScale = new Vector3(config.CellSize, SmallBoxHeight, config.CellSize);
                sb.transform.position = smallPos;
                sb.transform.SetParent(layer.transform);
                sb.tag = Constants.TagConstans.FloorGridTag;
                var smallRend = sb.GetComponent<Renderer>();
                smallRend.material = config.GridMaterial;
                smallRend.enabled = false; // hide visuals, keep collider
                sb.GetComponent<BoxCollider>().isTrigger = true;

                layerSmallBoxes[i, j] = sb;
            }
        }
    }

    private void CreateRamps(GridConfig config, GameObject root, GameObject[,] smallBoxes)
    {
        bool coordsOneBased = DetectCoordsOneBased(config);
        //створити пандуси для холмів тут 
        foreach (Ramp ramp in config.Ramps)
        {
            if (ramp == null) continue;

            int ri = ramp.i - (coordsOneBased ? 1 : 0);
            int rj = ramp.j - (coordsOneBased ? 1 : 0);

            Vector2Int targetCellAdjusted = ramp.targetCell;
            if (coordsOneBased && ramp.targetCell != null)
            {
                targetCellAdjusted = new Vector2Int(ramp.targetCell.x - 1, ramp.targetCell.y - 1);
            }

            // validate indices
            if (ri < 0 || rj < 0 || ri >= config.Width || rj >= config.Length)
            {
                Debug.LogWarning($"[ObstacleGenerator_05] Ramp coords out of range after adjustment: {ri},{rj} (orig {ramp.i},{ramp.j})");
                ramp.GO = null;
                continue;
            }

            //далі привяжем пандус по obstacle
            ramp.GO =
                CreateRamp(config, smallBoxes, root, ri, rj, ramp.direction, ramp.parent, ramp.material, targetCellAdjusted);
        }
    }

    private GameObject CreateRamp(GridConfig config, GameObject[,] smallBoxes, GameObject root, int i, int j, RampDirection direction, Transform parent, Material material, Vector2Int targetCell)
    {
        // validate indices
        if (i < 0 || j < 0 || i >= config.Width || j >= config.Length)
        {
            Debug.LogWarning($"[ObstacleGenerator_05] CreateRamp: indices out of range {i},{j}");
            return null;
        }

        // розміри (per-cell wedge)
        float w = config.CellSize;
        float l = config.CellSize;
        float h; // висота пандуса

        // Центр великого боксу
        Vector3 center = root.transform.position;

        // світова позиція клітинки (центр)
        float bigWidth = config.Width * config.CellSize;
        float bigLength = config.Length * config.CellSize;
        float x = -bigWidth / 2f + (i + 0.5f) * config.CellSize;
        float z = -bigLength / 2f + (j + 0.5f) * config.CellSize;

        // знаходимо перешкоду під цією клітинкою (як є)
       // var true_i = i > 0 ? i - 1 : i;
       // var true_j = j > 0 ? j - 1 : j;
        ObstacleOnTheMap obstacle = GetObstacleAt(config, i + 1, j + 1); //i, j)

        // базовий Y
        float baseSurfaceY;
        if (obstacle != null)
        {
            baseSurfaceY = center.y + GetTotalHeight(config.Obstacles, obstacle); //+ 0.2f;
            h = obstacle.BuildingHeight;
        }
        else
        {
            baseSurfaceY = center.y + LiftAboveGround;
            h = config.CellSize * 0.5f;
        }

        // створюємо об'єкт рами
        GameObject ramp = new GameObject($"Ramp_{i}_{j}");
        ramp.transform.SetParent(parent, true);
        ramp.transform.position = new Vector3(center.x + x, baseSurfaceY, center.z + z);
        ramp.transform.rotation = Quaternion.identity;

        // mesh creation ...
        Mesh mesh = new Mesh();
        float halfW = w * 0.5f;
        float halfL = l * 0.5f;

        Vector3[] verts = new Vector3[]
        {
            new Vector3(-halfW, 0f, -halfL),
            new Vector3( halfW, 0f, -halfL),
            new Vector3(-halfW, 0f,  halfL),
            new Vector3( halfW, 0f,  halfL),
            new Vector3(-halfW, h,  halfL),
            new Vector3( halfW, h,  halfL)
        };

        int[] tris = new int[]
        {
            0,1,3, 0,3,2,
            0,2,4,
            1,5,3,
            2,3,5, 2,5,4,
            0,4,1, 1,4,5
        };

        mesh.vertices = verts;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        var mf = ramp.AddComponent<MeshFilter>();
        mf.mesh = mesh;
        var mr = ramp.AddComponent<MeshRenderer>();
        mr.material = material != null ? material : config.GridMaterial;

        var mc = ramp.AddComponent<MeshCollider>();
        mc.sharedMesh = mesh;
        mc.convex = false;
        mc.isTrigger = false;

        // rotate according to direction
        switch (direction)
        {
            case RampDirection.North: break;
            case RampDirection.South: ramp.transform.Rotate(0f, 180f, 0f); break;
            case RampDirection.East: ramp.transform.Rotate(0f, 90f, 0f); break;
            case RampDirection.West: ramp.transform.Rotate(0f, -90f, 0f); break;
        }

        // top small-box on ramp
        GameObject topBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        topBox.name = $"RampTopBox_{i}_{j}";
        topBox.transform.SetParent(ramp.transform, false);
        topBox.transform.localScale = new Vector3(config.CellSize, SmallBoxHeight, config.CellSize);
        topBox.tag = Constants.TagConstans.FloorGridTag;
        topBox.GetComponent<Renderer>().material = config.GridMaterial;

        float forwardOffset = halfL;
        float verticalOffset = h + (SmallBoxHeight / 2f);
        topBox.transform.localPosition = new Vector3(0f, verticalOffset, forwardOffset);

        var topBoxCollider = topBox.GetComponent<BoxCollider>();
        topBoxCollider.isTrigger = true;

        var marker = topBox.AddComponent<RampMarker>();
        marker.targetCell = targetCell;
        topBox.tag = Constants.TagConstans.RampTopBox;

        if (obstacle != null)
        {
            if (obstacle.SmallBoxes == null) obstacle.SmallBoxes = new List<GameObject>();
            obstacle.SmallBoxes.Add(topBox);
        }

        if (smallBoxes != null && i >= 0 && i < smallBoxes.GetLength(0) && j >= 0 && j < smallBoxes.GetLength(1))
        {
            smallBoxes[i, j] = topBox;
        }

        return ramp;
    }

    //misc

    private ObstacleOnTheMap GetObstacleAt(GridConfig config, int i, int j)
    {
        foreach (var obs in config.Obstacles)
        {
            var found = GetObstacleAtRecursive(obs, i, j);
            if (found != null)
                return found;
        }
        return null;
    }

    private ObstacleOnTheMap GetObstacleAtRecursive(ObstacleOnTheMap obs, int i, int j)
    {
        int startX = obs.BuildingGridPos.x;
        int startZ = obs.BuildingGridPos.y;

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

    private float GetTotalHeight(List<ObstacleOnTheMap> obstacles, ObstacleOnTheMap obs)
    {
        float height = obs.BuildingHeight;
        var parent = FindParentObstacle(obstacles, obs);
        while (parent != null)
        {
            height += parent.BuildingHeight;
            parent = FindParentObstacle(obstacles, parent);
        }
        return height;
    }

    private ObstacleOnTheMap FindParentObstacle(List<ObstacleOnTheMap> obstacles, ObstacleOnTheMap child)
    {
        foreach (var obs in obstacles)
        {
            if (obs.NestedObstacle == child)
                return obs;
            var parent = FindParentObstacleRecursive(obs, child);
            if (parent != null)
                return parent;
        }
        return null;
    }

    private ObstacleOnTheMap FindParentObstacleRecursive(ObstacleOnTheMap parent, ObstacleOnTheMap child)
    {
        if (parent.NestedObstacle == child)
            return parent;

        if (parent.NestedObstacle != null)
            return FindParentObstacleRecursive(parent.NestedObstacle, child);

        return null;
    }

    private void GenerateHillsAndClimbs(ObstacleConfig obstacleConfig)
    {
        var hill_level1 = new ObstacleOnTheMap("Obstacle__w:7_l:9", new Vector2Int(10, 10), 7, 9, 1f, needSmallBoxOnTop: true, obstacleConfig.Level1_ObstacleMaterial);
        var hill_level2 = new ObstacleOnTheMap("Obstacle_(level2)_w:2_l:3", new Vector2Int(10, 10), 3, 2, 1f, needSmallBoxOnTop: true, obstacleConfig.Level2_ObstacleMaterial);
        var hill_level3 = new ObstacleOnTheMap("Obstacle_(level3)_w:1_l:1", new Vector2Int(10, 10), 1, 1, 1f, needSmallBoxOnTop: true, obstacleConfig.Level3_ObstacleMaterial);
        hill_level2.NestedObstacle = hill_level3;
        hill_level1.NestedObstacle = hill_level2;

        obstacleConfig.Obstacles.Add(hill_level1);
    }

    private bool DetectCoordsOneBased(GridConfig gridConfig)
    {
        if (gridConfig == null) return false;
        // If any obstacle or ramp references index 0 -> assume 0-based.
        foreach (var o in gridConfig.Obstacles ?? new List<ObstacleOnTheMap>())
        {
            if (o == null) continue;
            if (o.BuildingGridPos.x == 0 || o.BuildingGridPos.y == 0) return false;
        }
        foreach (var r in gridConfig.Ramps ?? new List<Ramp>())
        {
            if (r == null) continue;
            if (r.i == 0 || r.j == 0) return false;
        }
        // default: treat as 1-based when no zero found (legacy input)
        return true;
    }

}