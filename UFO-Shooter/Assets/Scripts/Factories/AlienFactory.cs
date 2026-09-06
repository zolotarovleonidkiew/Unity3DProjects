using UnityEngine;

public class AlienFactory
{
    private float _alienSize;
    private Material _alienDefaultMaterial = null;
    private BackgroundGenerationScript _ground;
    private GameObject AlienCollectionGUI;

    public AlienFactory(float alienSize, Material alienDefaultMaterial, BackgroundGenerationScript ground, GameObject alienCollectionGUI)
    {
        _alienSize = alienSize;
        _alienDefaultMaterial = alienDefaultMaterial;
        _ground = ground;

        AlienCollectionGUI = alienCollectionGUI;
    }

    public Alien CreateAlien(AlienTypesEnum alienTypesEnum, GameObject targetCell, int index, GameObject creaturePrefab = null)
    {
        if (targetCell == null) return null;

        Vector3 pos = targetCell.transform.position;
        float alienY = _ground.bigHeight + _alienSize / 2f;

        #region Alien Game Object
        GameObject alienGO;

        if (creaturePrefab != null)
        {
            alienGO = Object.Instantiate(creaturePrefab);            
            // place prefab at desired position (keep prefab rotation)           
        }
        else
        {
            alienGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            alienGO.transform.localScale = new Vector3(_alienSize, _alienSize, _alienSize);            
        }

        alienGO.name = $"Alien_{index}";
        alienGO.tag = Constants.TagConstans.AlienTag;
        alienGO.transform.position = new Vector3(pos.x, alienY, pos.z);
        alienGO.transform.SetParent(AlienCollectionGUI.transform);

        #endregion

        #region Physics Setup        
        var col = alienGO.GetComponent<Collider>();
        if (col == null)
        {
            // add a capsule collider as a sensible default
            var capsule = alienGO.AddComponent<CapsuleCollider>();
            capsule.height = _alienSize;
            capsule.radius = _alienSize / 2f;
            capsule.center = new Vector3(0f, _alienSize / 2f, 0f);
            col = capsule;
        }

        // Make sure transforms are synced so bounds are correct, then align bottom of collider to desired ground Y
        Physics.SyncTransforms();
        float bottomY = col.bounds.min.y;
        float deltaY = alienY - bottomY;
        if (Mathf.Abs(deltaY) > 0.0001f)
        {
            alienGO.transform.position += new Vector3(0f, deltaY + 0.001f, 0f);
        }

        // Ensure Rigidbody exists
        var rb = alienGO.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = alienGO.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        #endregion

        #region Alien Settings
        var alienData = GetAlienDataByType(alienTypesEnum);

        var alien = alienGO.AddComponent<Alien>();     
        alien.MaxHealth = alienData.Health;
        alien.CurentHealth = alienData.Health;
        alien.Damage = alienData.Damage;
        alien.Speed = alienData.Speed;
        alien.DetectionRange = alienData.DetectionRange;
        alien.AttackCooldown = alienData.AttackCooldown;
        alien.CanFly = alienData.CanFly;
        alien.isFlying = false; // Default value, can be changed later if needed
        alien.FlyHeight = alienData.FlyHeight;
        #endregion

        #region + компонет расчета урона
        var damageCalculator = alienGO.AddComponent<ShootingDamageCalculations>();
        damageCalculator.isAlien = true;
        damageCalculator.isHero = false;
        #endregion

        return alien;
    }

    private AlienData GetAlienDataByType(AlienTypesEnum alienType)
    {
        switch (alienType)
        {
            case AlienTypesEnum.level_0_Greys:
                return new AlienGreys();
            case AlienTypesEnum.level_0_Light_Drone:
                return new AlienLightDrone();
            case AlienTypesEnum.level_1_Heavy_Drone:
                return new AlienHeavyDrone();
            case AlienTypesEnum.level_2_Assault_Trooper:
                return new AlienAssaultTrooper ();
            case AlienTypesEnum.level_2_Insect:
                return new AlienInsect();
            case AlienTypesEnum.level_3_Master_Mind:
                return new AlienMasterMind ();
            case AlienTypesEnum.level_3_Assault_Machine:
                return new AlienAssaultMachine();
            case AlienTypesEnum.level_4_Insect_Matriarch:
                return new AlienInsectMatriarch();
            default:
                Debug.LogWarning($"Unknown alien type: {alienType}");
                return null;
        }
    }
}