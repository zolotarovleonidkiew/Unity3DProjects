using UnityEngine;

/// <summary>
/// Creates Heros on the land
/// </summary>
public class HeroFactory : MonoBehaviour
{
    private float _playerSize;    
    private BackgroundGenerationScript _ground;
    private GameObject _HeroesCollectionGUI;
    private Material _gridMaterialHighlited;
    private Material _heroMaterial;

    public HeroFactory(
        float playerSize,
        Material heroMaterial,
        BackgroundGenerationScript ground,
        GameObject heroesCollectionGUI,
        Material gridMaterialHighlited
    )
    {
        _ground = ground;
        _playerSize = playerSize;
        _heroMaterial = heroMaterial;
        _HeroesCollectionGUI = heroesCollectionGUI;
        _gridMaterialHighlited = gridMaterialHighlited;
    }

    public Hero CreateHero(
        GameObject targetCell,
        int index,
        UILiveCreatureDisposition disposition,
        GameObject creaturePrefab = null)
    {
        if (targetCell == null) return null;

        Vector3 pos = targetCell.transform.position;
        float playerY = _ground.bigHeight + _playerSize / 2f;

        #region Hero Game Object
        var heroData = new HeroActivist();
        GameObject player;

        // If ground provides a hero prefab - instantiate it, otherwise fall back to a primitive
        if (_ground != null && creaturePrefab != null)
        {
            player = Object.Instantiate(creaturePrefab);
            player.name = $"Hero_{index}";
            // place prefab at desired position (keep prefab rotation)
            player.transform.position = new Vector3(pos.x, playerY, pos.z);
        }
        else
        {
            player = GameObject.CreatePrimitive(PrimitiveType.Cube);
            player.name = $"Hero_{index}";
            player.transform.localScale = new Vector3(_playerSize, _playerSize, _playerSize);
            player.transform.position = new Vector3(pos.x, playerY, pos.z);
        }
        player.tag = Constants.TagConstans.HeroTag;
        player.transform.SetParent(_HeroesCollectionGUI.transform);
        #endregion

        #region Physics Setup

        // Ensure Collider exists so the hero won't fall through the floor
        var col = player.GetComponent<Collider>(); //Capsule Collider in the Prefab

        // Make sure transforms are synced so bounds are correct, then align bottom of collider to desired ground Y
        Physics.SyncTransforms();
        float bottomY = col.bounds.min.y;
        float deltaY = playerY - bottomY;
        if (Mathf.Abs(deltaY) > 0.0001f)
        {
            player.transform.position += new Vector3(0f, deltaY + 0.001f, 0f);
        }

        // Ensure Rigidbody exists
        var rb = player.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = player.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        #endregion

        #region Hero Settings
        var hero = player.GetComponent<Hero>();
        if (hero == null)
        {
            hero = player.AddComponent<Hero>();

            //Setup parameters (hero data)
            hero.MaxHealth = heroData.Health;
            hero.CurentHealth = heroData.Health;
            hero.Speed = heroData.Speed;
            hero.DetectionRange = heroData.DetectionRange;

        }
        hero.SetFlag_ShowAvailableMovementSquares();
        hero.SetGroundObject(_ground);
        hero.SetStartingWeapons(disposition?.heroStartingWeapons);
        hero.SetGrenadeCount(disposition?.GrenadeCount ?? 0);
        hero.SetHighlightMaterial(_gridMaterialHighlited);

        // Prefer existing FirePoint (common in prefab). Fallback to ShootPoint or create a new ShootPoint if none exist.
        Transform shootPointT = player.transform.Find("FirePoint");
        if (shootPointT == null)
            shootPointT = player.transform.Find("ShootPoint");

        // do not create a ShootPoint here; prefer prefab's FirePoint or existing ShootPoint
        hero.SetShootPoint(shootPointT);

        var heroMovement = player.AddComponent<HeroMovement>();
        heroMovement.SetMoveSpeed(3);
        #endregion

        #region + компонет расчета урона
        var damageCalculator = player.AddComponent<ShootingDamageCalculations>();
        damageCalculator.isAlien = false;
        damageCalculator.isHero = true;
        #endregion
        return hero;
    }
}