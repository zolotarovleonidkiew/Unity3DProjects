using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates Heros on the land
/// </summary>
public class HeroFactory : MonoBehaviour
{
    private float _playerSize;
    private Material _heroMaterial;
    private List<WeaponData> _startingWeapons;
    private GameObject _bulletPrefab;
    private GameObject _heroPrefab;
    private BackgroundGenerationScript _ground;
    private GameObject _HeroesCollectionGUI;
    private Material _gridMaterialHighlited;

    public HeroFactory(
        float playerSize,
        Material heroMaterial,
        List<WeaponData> startingWeapons,
        GameObject bulletPrefab,
        GameObject heroPrefab,
        BackgroundGenerationScript ground,
        GameObject heroesCollectionGUI,
        Material gridMaterialHighlited
    )
    {
        _playerSize = playerSize;
        _heroMaterial = heroMaterial;
        _startingWeapons = startingWeapons;
        _bulletPrefab = bulletPrefab;
        _heroPrefab = heroPrefab;
        _ground = ground;
        _HeroesCollectionGUI = heroesCollectionGUI;
        _gridMaterialHighlited = gridMaterialHighlited;
    }

    public Hero CreateHero(GameObject targetCell, int index)
    {
        if (targetCell == null) return null;

        Vector3 pos = targetCell.transform.position;
        float playerY = _ground.bigHeight + _playerSize / 2f;

        GameObject player;

        // If ground provides a hero prefab - instantiate it, otherwise fall back to a primitive
        if (_ground != null && _heroPrefab != null)
        {
            player = Object.Instantiate(_heroPrefab);
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

        // Ensure Collider exists so the hero won't fall through the floor
        var col = player.GetComponent<Collider>();
        if (col == null)
        {
            // add a capsule collider as a sensible default
            var capsule = player.AddComponent<CapsuleCollider>();
            capsule.height = _playerSize;
            capsule.radius = _playerSize / 2f;
            capsule.center = new Vector3(0f, _playerSize / 2f, 0f);
            col = capsule;
        }

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

        // Ensure Hero component exists (prefab might already have it)
        var sh = player.GetComponent<Hero>();
        if (sh == null)
        {
            sh = player.AddComponent<Hero>();
        }
        sh.SettoShowAvailableMovementSquares();
        sh.SetGroundObject(_ground);
        sh.SetStartingWeapons(_startingWeapons);
        sh.SetBulletPrefab(_bulletPrefab);
        sh.SetHighlightMaterial(_gridMaterialHighlited);

        // Prefer existing FirePoint (common in prefab). Fallback to ShootPoint or create a new ShootPoint if none exist.
        Transform shootPointT = player.transform.Find("FirePoint");
        if (shootPointT == null)
            shootPointT = player.transform.Find("ShootPoint");

        // do not create a ShootPoint here; prefer prefab's FirePoint or existing ShootPoint
        sh.SetShootPoint(shootPointT);

        var hms = player.AddComponent<HeroMovement>();
        hms.SetMoveSpeed(3);

        player.transform.SetParent(_HeroesCollectionGUI.transform);
        return sh;
    }
}