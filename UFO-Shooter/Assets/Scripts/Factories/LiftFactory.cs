using UnityEngine;

public class LiftFactory
{
    private readonly GameObject HeroesCollectionGUI;

    private readonly Material _liftMaterial;
    private readonly Vector3 _liftSize;
    private readonly float _liftHeight;

    public LiftFactory(GameObject heroesCollectionGUI, Material liftMaterial = null, Vector3? liftSize = null, float liftHeight = 0.2f)
    {
        _liftMaterial = liftMaterial ?? new Material(Shader.Find("Standard"));
        _liftMaterial.color = Color.yellow;

        _liftSize = liftSize ?? new Vector3(2f, 0.2f, 2f);
        _liftHeight = liftHeight;

        HeroesCollectionGUI = heroesCollectionGUI;
    }

    /// <summary>
    /// Створює ліфт (платформу) у заданій позиції.
    /// </summary>
    public GameObject CreateLift(Vector3 worldPos, Transform parent = null)
    {
        GameObject lift = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lift.name = "LiftPlatform";
        lift.transform.SetParent(parent, true);

        float topY = worldPos.y + 0.01f;           // на 0.01f вище ніж bigBox, щоб герой зміг зайти на ліфт, і ліфт трози був бачний на землі
        float centerY = topY - (_liftHeight * 2f); // центр куба
        lift.transform.position = new Vector3(worldPos.x, centerY, worldPos.z);

        lift.transform.localScale = _liftSize;

        var rend = lift.GetComponent<Renderer>();
        rend.material = new Material(_liftMaterial); // копія матеріалу, щоб унікально виглядало

        lift.AddComponent<LiftMarker>(); // маркер для взаємодії
        var lp = lift.AddComponent<LiftPlatform>();
        lp.HeroesCollectionGUI = HeroesCollectionGUI;

        //коллайдер, який реагую на вступ героя на плтформу ліфта
        BoxCollider triggerCollider = lift.AddComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.center = new Vector3(0f, 1.79f, 0f);
        triggerCollider.size = new Vector3(0.39f, 0.28f, 0.31f);

        return lift;
    }
}