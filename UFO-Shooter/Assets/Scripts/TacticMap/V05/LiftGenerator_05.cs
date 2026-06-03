using Assets.Scripts.TacticMap;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LiftGenerator_05 : ILiftGenerator
{
    public const string Algorith_version = "0.5";
    private const float DefaultLiftHeight = 0.2f;

    /// <summary>
    /// Create lifts on the map.
    /// Uses liftLayers for upper-floor positions (platforms already created by IGridGenerator).
    /// </summary>
    public IEnumerable<GameObject> GenerateLifts(
        LiftConfig liftConfig,
        GameObject[,] groundSmallBoxes,
        IDictionary<Vector2Int, GroundLayer> liftLayers,
        GameObject heroesCollectionGUI)
    {
        if (liftConfig == null || liftConfig.LiftPositions == null || !liftConfig.LiftPositions.Any()) yield break;

        foreach (var pos in liftConfig.LiftPositions)
        {
            var smallBox = GetSmallCube(groundSmallBoxes, pos.x, pos.y);
            if (smallBox == null)
            {
                Debug.LogWarning($"[LiftGenerator_05] No small-box at {pos}. Skipping lift creation.");
                continue;
            }

            // remove small-box and compute lift world position (slightly above)
            UnityEngine.Object.Destroy(smallBox);
            Vector3 liftWorldPos = smallBox.transform.position + Vector3.up * 0.01f;

            // create lift platform
            var lift = CreateLift(liftWorldPos, heroesCollectionGUI, liftConfig.Parent, liftConfig.LiftMaterial, new Vector3(liftConfig.CellSize, DefaultLiftHeight, liftConfig.CellSize));
            lift.tag = Constants.TagConstans.FloorGridTag;
            var liftPlatform = lift.GetComponent<LiftPlatform>();
            if (liftPlatform == null)
                Debug.LogWarning("[LiftGenerator_05] LiftPlatform component missing on lift.");

            // place buttons (down on first floor, up on second). For upper button Y use created liftLayer height when available.
            float upperButtonY;
            if (liftLayers != null && liftLayers.TryGetValue(pos, out var upperLayer))
            {
                upperButtonY = upperLayer.GroundLevelObject.transform.position.y + 0.25f;
            }
            else
            {
                // fallback to heuristic matching original behaviour
                upperButtonY = liftConfig.GroundHeight + liftConfig.LiftAboveGround + (liftPlatform?.targetFloorHeight ?? 0f) + 0.25f;
            }

            CreateFloorButtons(liftWorldPos, liftPlatform, liftConfig.Parent, liftConfig.GroundHeight, liftConfig.LiftAboveGround, upperButtonY);

            yield return lift;
        }
    }

    private void CreateFloorButtons(Vector3 liftWorldPos, LiftPlatform liftPlatform, Transform parent, float bigHeight, float liftAboveBig, float upperButtonY)
    {
        if (liftPlatform == null) return;

        // Button on 1st floor (call to upper)
        GameObject btnDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
        btnDown.name = "LiftButtonDown->Up";
        btnDown.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        btnDown.transform.position = new Vector3(
            liftWorldPos.x + 1f,
            bigHeight + 0.5f,
            liftWorldPos.z
        );
        btnDown.transform.SetParent(parent);
        var collider = btnDown.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        // keep the original centers/sizes from the old code (they were tuned to prefab)
        collider.center = new Vector3(-2.16914f, -0.2331161f, 0.2354889f);
        collider.size = new Vector3(1.420128f, 0.5337677f, 1.470978f);
        btnDown.layer = Constants.Layers.LiftTriggerLayer;
        var buttonDown = btnDown.AddComponent<LiftButton>();
        buttonDown.lift = liftPlatform;
        buttonDown.callToUpper = true;

        // Button on 2nd floor (call to lower)
        GameObject btnUp = GameObject.CreatePrimitive(PrimitiveType.Cube);
        btnUp.name = "LiftButtonUp->Down";
        btnUp.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
        btnUp.transform.position = new Vector3(
            liftWorldPos.x + 1.5f,
            upperButtonY,
            liftWorldPos.z + 1.3f
        );
        btnUp.transform.SetParent(parent);
        collider = btnUp.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = new Vector3(-2.760944f, -0.1841786f, -2.004732f);
        collider.size = new Vector3(2.547562f, 0.6316428f, 1.88596f);
        btnUp.layer = Constants.Layers.LiftTriggerLayer;
        var buttonUp = btnUp.AddComponent<LiftButton>();
        buttonUp.lift = liftPlatform;
        buttonUp.callToUpper = false;
    }

    private GameObject CreateLift(Vector3 worldPos, GameObject heroesCollectionGUI, Transform parent = null, Material liftMaterial = null, Vector3? liftSizeNullable = null)
    {
        liftMaterial = liftMaterial ?? new Material(Shader.Find("Standard"));
        liftMaterial.color = Color.yellow;

        var liftSize = liftSizeNullable ?? new Vector3(2f, DefaultLiftHeight, 2f);

        GameObject lift = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lift.name = "LiftPlatform";

        if (parent != null)
            lift.transform.SetParent(parent, true);

        float topY = worldPos.y + 0.01f;
        float centerY = topY - (DefaultLiftHeight * 2f);

        lift.transform.position = new Vector3(worldPos.x, centerY, worldPos.z);
        lift.transform.localScale = liftSize;

        var rend = lift.GetComponent<Renderer>();
        rend.material = new Material(liftMaterial);

        lift.AddComponent<LiftMarker>();

        var liftPlatform = lift.AddComponent<LiftPlatform>();
        liftPlatform.HeroesCollectionGUI = heroesCollectionGUI;

        BoxCollider triggerCollider = lift.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.center = new Vector3(0f, 1.79f, 0f);
        triggerCollider.size = new Vector3(0.39f, 0.28f, 0.31f);

        return lift;
    }

    private GameObject GetSmallCube(GameObject[,] smallCubes, int i, int j)
    {
        if (smallCubes == null) return null;
        if (i < 0 || j < 0 || i >= smallCubes.GetLength(0) || j >= smallCubes.GetLength(1)) return null;
        return smallCubes[i, j];
    }
}
