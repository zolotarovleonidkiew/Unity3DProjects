using Assets.Scripts.TacticMap;
using Assets.Scripts.TacticMap.V05;

using System.Collections.Generic;

using Unity.VisualScripting.FullSerializer;

using UnityEngine;

public class GridGenerator_05 : IGridGenerator
{
    public const string Algorith_version = "0.5";

    private const float SmallBoxHeight = 0.5f;
    private const float LiftAboveGround = 0.3f;

    // registry of created layers (populated by CreatePlatorm / CreateUpperPlatform)
    private static readonly List<GroundLayer> RegisteredLayers = new List<GroundLayer>();

    private static void RegisterLayer(GroundLayer layer)
    {
        if (layer == null) return;
        // avoid duplicates
        if (!RegisteredLayers.Contains(layer))
            RegisteredLayers.Add(layer);
    }

    public GroundLayer CreatePlatorm(GridConfig config, GroundHierarchyLevel level, string name)
    {
        var platform = CreateEmptyPlatmorm(config, level, name);
        var smallBoxes = CreateSmallBoxes(platform, config);

        var layer = new GroundLayer(name, platform, smallBoxes, level);

        // register newly created layer so runtime lookups can use it
        RegisterLayer(layer);

        return layer;
    }

    private GameObject CreateEmptyPlatmorm(GridConfig config, GroundHierarchyLevel level, string name)
    {
        // dimensions
        float bigWidth = config.Width * config.CellSize;
        float bigLength = config.Length * config.CellSize;

        // root cube (platform)
        var root = GameObject.CreatePrimitive(PrimitiveType.Cube);
        root.name = $"Floor_TacticalMap_{nameOrSize(config)}";
        root.transform.localScale = new Vector3(bigWidth, config.GroundHeight, bigLength);
        root.transform.position = new Vector3(0f, config.GroundHeight, 0f);
        root.transform.SetParent(config.Parent);
        root.GetComponent<Renderer>().material = config.GroundMaterial;

        var rb = root.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        return root;
    }

    // helper for naming; keeps previous behavior
    private string nameOrSize(GridConfig config) => $"{config.Length}x{config.Width}";

    private GameObject[,] CreateSmallBoxes(GameObject platform, GridConfig config)
    {
        float bigWidth = config.Width * config.CellSize;
        float bigLength = config.Length * config.CellSize;

        var smallCubes = new GameObject[config.Width, config.Length];
        Vector3 center = platform.transform.position;

        for (int i = 0; i < config.Width; i++)
        {
            for (int j = 0; j < config.Length; j++)
            {
                float x = -bigWidth / 2f + (i + 0.5f) * config.CellSize;
                float z = -bigLength / 2f + (j + 0.5f) * config.CellSize;
                // Use platform center as baseline so small boxes are placed on top of any platform,
                // not at an absolute config.GroundHeight which was causing incorrect vertical placement.
                Vector3 smallPos = new Vector3(
                    center.x + x,
                    center.y + LiftAboveGround,
                    center.z + z
                );

                GameObject smallBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
                smallBox.name = $"SmallBox_{i}_{j}";
                smallBox.transform.localScale = new Vector3(config.CellSize, SmallBoxHeight, config.CellSize);
                smallBox.transform.position = smallPos;
                smallBox.transform.SetParent(platform.transform);
                smallBox.tag = Constants.TagConstans.FloorGridTag;
                var rend = smallBox.GetComponent<Renderer>();
                rend.material = config.GridMaterial;
                rend.enabled = false;
                smallBox.GetComponent<BoxCollider>().isTrigger = true;

                smallCubes[i, j] = smallBox;
            }
        }

        return smallCubes;
    }

    /// <summary>
    /// Create small "second-floor" platforms for each lift position.
    /// Returns dictionary mapping lift grid position -> created GroundLayer.
    /// GridGenerator uses the already-created groundSmallBoxes to compute lift origin positions.
    /// </summary>
    public IDictionary<Vector2Int, GroundLayer> CreateUpperPlatform(GridConfig groundConfig, LiftConfig liftConfig, GameObject[,] groundSmallBoxes)
    {
        var result = new Dictionary<Vector2Int, GroundLayer>();

        if (liftConfig == null || liftConfig.LiftPositions == null || groundSmallBoxes == null)
            return result;

        foreach (var liftPos in liftConfig.LiftPositions)
        {
            // validate indices
            if (liftPos.x < 0 || liftPos.y < 0 || liftPos.x >= groundSmallBoxes.GetLength(0) || liftPos.y >= groundSmallBoxes.GetLength(1))
                continue;

            var originSmall = groundSmallBoxes[liftPos.x, liftPos.y];
            if (originSmall == null)
                continue;

            // compute second-floor world position using original positioning logic
            float cellSize = groundConfig.CellSize;
            float floor2_x = originSmall.transform.position.x + (2f * cellSize - cellSize / 2f);
            float floor2_z = originSmall.transform.position.z - (cellSize / 2f);
            float floor2_y = Constants.TacticMapConstructingConstants.Floor2Height;

            // build a GridConfig for the 2x2 platform
            var secondConfig = new GridConfig
            {
                Width = 2,
                Length = 2,
                CellSize = groundConfig.CellSize,
                // thickness of the platform (use same thickness as groundConfig to get ~0.5 height)
                GroundHeight = groundConfig.GroundHeight,
                SmallBoxHeight = groundConfig.SmallBoxHeight,
                LiftOffset = 0,
                GridMaterial = groundConfig.GridMaterial,
                GroundMaterial = groundConfig.GroundMaterial,
                Parent = liftConfig.Parent ?? groundConfig.Parent,
                Obstacles = null,
                Ramps = null,
                LiftPositions = new List<Vector2Int>()
            };

            // create platform root manually at computed world position (center.y = floor2_y)
            float bigWidth = secondConfig.Width * secondConfig.CellSize;
            float bigLength = secondConfig.Length * secondConfig.CellSize;

            var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
            platform.name = $"Lift_SecondFloor_{liftPos.x}_{liftPos.y}";
            // platform thickness = secondConfig.GroundHeight, center at floor2_y (so top ~= floor2_y + half thickness)
            platform.transform.localScale = new Vector3(bigWidth, secondConfig.GroundHeight, bigLength);
            platform.transform.position = new Vector3(floor2_x, floor2_y, floor2_z);
            platform.transform.SetParent(secondConfig.Parent);
            var rend = platform.GetComponent<Renderer>();
            rend.material = secondConfig.GroundMaterial;//GridMaterial;
            var rb = platform.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;

            // now create smallBoxes on that platform (uses existing CreateSmallBoxes helper)
            var smallBoxes = CreateSmallBoxes(platform, secondConfig);

            // wrap in GroundLayer and store
            var gl = new GroundLayer($"LiftFloor_{liftPos.x}_{liftPos.y}", platform, smallBoxes, GroundHierarchyLevel.Level_2);

            // register upper-floor layer for runtime lookups
            RegisterLayer(gl);

            result[liftPos] = gl;
        }

        return result;
    }

    // -------------------------------------------------------------------------
    // Static grid helpers moved here (used by runtime code: HeroMovement, BackgroundGenerationScript etc.)
    // -------------------------------------------------------------------------
    public static bool GetGridCoordsFromWorld(Vector3 worldPos, GameObject[,] smallBoxed, out int i, out int j)
    {
        i = -1;
        j = -1;

        if (smallBoxed == null) return false;

        float minDist = float.MaxValue;
        int width = smallBoxed.GetLength(0);
        int length = smallBoxed.GetLength(1);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                // pass the actual smallBoxed array so we check the correct cells
                var cube = GridGenerator_05.GetSmallCube(x, z, smallBoxed);
                if (cube == null) continue;

                float dist = Vector3.Distance(worldPos, cube.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    i = x;
                    j = z;
                }
            }
        }

        return (i >= 0 && j >= 0);
    }

    // New overload: search all registered layers and return nearest cell indices
    public static bool GetGridCoordsFromWorld(Vector3 worldPos, out int i, out int j)
    {
        i = -1;
        j = -1;

        if (RegisteredLayers.Count == 0) return false;

        float minDist = float.MaxValue;
        int bestI = -1, bestJ = -1;

        foreach (var layer in RegisteredLayers)
        {
            var arr = layer.SmallBoxed;
            if (arr == null) continue;

            if (GetGridCoordsFromWorld(worldPos, arr, out int ti, out int tj))
            {
                var cube = GridGenerator_05.GetSmallCube(ti, tj);
                if (cube == null) continue;

                float dist = Vector3.Distance(worldPos, cube.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    bestI = ti;
                    bestJ = tj;
                }
            }
        }

        if (bestI >= 0 && bestJ >= 0)
        {
            i = bestI;
            j = bestJ;
            return true;
        }

        return false;
    }

    public static GameObject GetSmallCube(int i, int j, GameObject[,] smallBoxed)
    {
        if (smallBoxed == null) return null;
        if (i < 0 || j < 0 || i >= smallBoxed.GetLength(0) || j >= smallBoxed.GetLength(1)) return null;
        return smallBoxed[i, j];
    }

    // Add this static method into GridGenerator_05 (next to the existing GetSmallCube/get helpers)
    public static GameObject GetSmallCube(int i, int j)
    {
        // Safety
        if (i < 0 || j < 0) return null;

        // Search registered layers (first non-null cell wins)
        foreach (var layer in RegisteredLayers)
        {
            var arr = layer?.SmallBoxed;
            if (arr == null) continue;

            if (i < arr.GetLength(0) && j < arr.GetLength(1))
            {
                var cube = arr[i, j];
                if (cube != null) return cube;
            }
        }

        return null;
    }

    public static GameObject FindNearestValidCell(int wantI, int wantJ, GameObject[,] smallBoxed, float cellSize, float bigHeight, float liftAboveBig)
    {
        if (smallBoxed == null) return null;

        int width = smallBoxed.GetLength(0);
        int length = smallBoxed.GetLength(1);

        // target world pos (approximate)
        float bigWidth = width * cellSize;
        float bigLength = length * cellSize;
        Vector3 center = new Vector3(0f, bigHeight / 2f, 0f);
        Vector3 wantedWorld = new Vector3(
            center.x + (-bigWidth / 2f + (wantI + 0.5f) * cellSize),
            bigHeight + liftAboveBig,
            center.z + (-bigLength / 2f + (wantJ + 0.5f) * cellSize)
        );

        float bestDist = float.MaxValue;
        GameObject best = null;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < length; j++)
            {
                var c = GetSmallCube(i, j, smallBoxed);
                if (c == null) continue;
                float d = Vector3.Distance(wantedWorld, c.transform.position);
                if (d < bestDist)
                {
                    bestDist = d;
                    best = c;
                }
            }
        }

        return best;
    }
}