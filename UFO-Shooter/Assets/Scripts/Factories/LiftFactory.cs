using System.Collections.Generic;
using UnityEngine;

public class LiftFactory : ILiftFactory
{
    public const float LiftHeight = 0.2f;
    private const float LiftAboveGround = 0.3f;

    /// <summary>
    /// Створює ліфт (платформу) у заданій позиції.
    /// </summary>
    public IEnumerable<GameObject> CreateLifts(
        List<Vector2Int> liftGridPosCollection, 
        GameObject heroesCollectionGUI,
        GameObject[,] smallBoxes,
        float bigHeight = 0.5f,
        Transform parent = null, 
        Material liftMaterial = null, 
        Vector3? liftSizeNullable = null)
    {
        var createdLifts = new List<GameObject>();

        foreach (Vector2Int liftGridPos in liftGridPosCollection)
        {
            var smallBox = GetSmallCube(smallBoxes, liftGridPos.x, liftGridPos.y);
            var lift = CreateLiftAndSecondFloor(liftGridPos, smallBox, heroesCollectionGUI, bigHeight, parent, liftMaterial, liftSizeNullable);
            createdLifts.Add(lift);
        }

        return createdLifts;
    }

    

    /// <summary>
    /// Створюємо 2ий поверх таліфт туди
    /// </summary>
    private GameObject CreateLiftAndSecondFloor(
        Vector2Int liftGridPos, 
        GameObject targetLiftCell, 
        GameObject heroesCollectionGUI,
        float bigHeight = 0.5f,
        Transform parent = null, 
        Material liftMaterial = null, 
        Vector3? liftSizeNullable = null)
    {
        // --- Додаємо ліфт ---
        LiftPlatform liftPlatform = null;

        if (targetLiftCell != null)
        {
            // Прибираємо smallCube, бо тут має стояти ліфт
            Object.Destroy(targetLiftCell);

            // Трохи піднімаємо ліфт, щоб він не перетинався з ground-box
            Vector3 liftWorldPos = targetLiftCell.transform.position + Vector3.up * 0.01f;

            //LiftFactory lf = new LiftFactory(HeroesCollectionGUI, liftMaterial);
            //GameObject lift = lf.CreateLift(liftWorldPos, transform);
            GameObject lift = CreateLift(liftWorldPos, heroesCollectionGUI, parent, liftMaterial, liftSizeNullable);
            lift.tag = Constants.TagConstans.FloorGridTag;//як у смол-бокса
            liftPlatform = lift.GetComponent<LiftPlatform>();
            lift.transform.SetParent(parent?.transform);

            // --- Створюємо кнопку на 1-му поверсі ---
            GameObject btnDown = GameObject.CreatePrimitive(PrimitiveType.Cube);
            btnDown.name = "LiftButtonDown->Up";
            btnDown.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            btnDown.transform.position = new Vector3(
                liftWorldPos.x + 1f,              // трохи збоку
                bigHeight + 0.5f,                 // висота 1-го поверху
                liftWorldPos.z
            );
            btnDown.transform.SetParent(parent?.transform); //transform
            var collider = btnDown.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = new Vector3(-2.16914f, -0.2331161f, 0.2354889f);
            collider.size = new Vector3(1.420128f, 0.5337677f, 1.470978f);
            btnDown.layer = Constants.Layers.LiftTriggerLayer;

            var buttonDown = btnDown.AddComponent<LiftButton>();
            buttonDown.lift = liftPlatform;
            buttonDown.callToUpper = true;

            // --- Створюємо кнопку на 2-му поверсі ---
            GameObject btnUp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            btnUp.name = "LiftButtonUp->Down";
            btnUp.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            btnUp.transform.position = new Vector3(
                //liftWorldPos.x + 1f,                            // збоку
                liftWorldPos.x + 1.5f,
                bigHeight + LiftAboveGround + liftPlatform.targetFloorHeight + 0.25f,//0.5f,
                liftWorldPos.z + 1.3f
            //liftWorldPos.z
            );
            btnUp.transform.SetParent(parent?.transform);//transform
            collider = btnUp.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.center = new Vector3(-2.760944f, -0.1841786f, -2.004732f);
            collider.size = new Vector3(2.547562f, 0.6316428f, 1.88596f);

            btnUp.layer = Constants.Layers.LiftTriggerLayer;

            var buttonUp = btnUp.AddComponent<LiftButton>();
            buttonUp.lift = liftPlatform;
            buttonUp.callToUpper = false;

            return lift;
        }
        else
        {
            Debug.LogWarning($"[BGS] Ліфт не створено — клітинка {liftGridPos} порожня або під будинком.");

            return null; //TO DO: check correctness of this case (null)
        }

        //TO DO
        //нужно предусмотреть это в GridGenerator_05

        // --- Ліфт + кнопки створені ---

        ////////створюємо 2ий поверх для цього ліфта
        //////var gg2 = new GridGenerator(
        //////    HeroesCollectionGUI,
        //////    2, 2,
        //////    cellSize, bigHeight, smallHeight, liftAboveBig,
        //////    gridBoxMaterial, groundBoxMaterial,
        //////    parent.transform,
        //////    obstacles: null
        //////);

        //////var floor2_x = targetLiftCell.transform.position.x + (2 * cellSize - cellSize / 2); //зміщуємо платформу щоб був доступ платформи ліфта до поверху
        //////var floor2_z = targetLiftCell.transform.position.z - (cellSize / 2);
        //////var floor2_y = Constants.TacticMapConstructingConstants.Floor2Height;
        //////Vector3 floor2_position = new Vector3 { x = floor2_x, y = floor2_y, z = floor2_z };
        ////////var floor2_smallCubes = gg2.GenerateGrid(null, 4f, floor2_position);

        ////////TEMP
        ////////TO DOuncomment
        //////floor2_smallCubes = gg2.GenerateGrid(null, 4f, floor2_position);
        ////////додаємо ліфт-
    }

    //+

    private GameObject CreateLift(Vector3 worldPos, GameObject heroesCollectionGUI, Transform parent = null, Material liftMaterial = null, Vector3? liftSizeNullable = null)
    {
        liftMaterial = liftMaterial ?? new Material(Shader.Find("Standard"));
        liftMaterial.color = Color.yellow;

        var liftSize = liftSizeNullable ?? new Vector3(2f, 0.2f, 2f);

        GameObject lift = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lift.name = "LiftPlatform";

        if (parent != null)
        {
            lift.transform.SetParent(parent, true);
        }

        float topY = worldPos.y + 0.01f;           // на 0.01f вище ніж bigBox, щоб герой зміг зайти на ліфт, і ліфт трози був бачний на землі
        float centerY = topY - (LiftHeight * 2f); // центр куба

        lift.transform.position = new Vector3(worldPos.x, centerY, worldPos.z);
        lift.transform.localScale = liftSize;

        var rend = lift.GetComponent<Renderer>();
        rend.material = new Material(liftMaterial); // копія матеріалу, щоб унікально виглядало

        lift.AddComponent<LiftMarker>(); // маркер для взаємодії

        var liftPlatform = lift.AddComponent<LiftPlatform>();
        liftPlatform.HeroesCollectionGUI = heroesCollectionGUI;

        //коллайдер, який реагує на вступ героя на плтформу ліфта
        BoxCollider triggerCollider = lift.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.center = new Vector3(0f, 1.79f, 0f);
        triggerCollider.size = new Vector3(0.39f, 0.28f, 0.31f);

        return lift;
    }

    public GameObject GetSmallCube(GameObject[,] smallCubes, int i, int j)
    {
        if (smallCubes == null) return null;
        if (i < 0 || j < 0 || i >= smallCubes.GetLength(0) || j >= smallCubes.GetLength(1)) return null;
        return smallCubes[i, j];
    }
}