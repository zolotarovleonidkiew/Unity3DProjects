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
    private BackgroundGenerationScript _ground;
    private GameObject _HeroesCollectionGUI;
    private Material _gridMaterialHighlited;

    public HeroFactory(
        float playerSize,
        Material heroMaterial,
        List<WeaponData> startingWeapons,
        GameObject bulletPrefab,
        BackgroundGenerationScript ground,
        GameObject heroesCollectionGUI,
        Material gridMaterialHighlited
    )
    {
        _playerSize = playerSize;
        _heroMaterial = heroMaterial;
        _startingWeapons = startingWeapons;
        _bulletPrefab = bulletPrefab;
        _ground = ground;
        _HeroesCollectionGUI = heroesCollectionGUI;
        _gridMaterialHighlited = gridMaterialHighlited;
    }

    public Hero CreateHero(GameObject targetCell, int index)
    {
        if (targetCell == null) return null;

        Vector3 pos = targetCell.transform.position;
        float playerY = _ground.bigHeight + _playerSize / 2f;

        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Cube);
        player.name = $"Hero_{index}";
        player.transform.localScale = new Vector3(_playerSize, _playerSize, _playerSize);
        player.transform.position = new Vector3(pos.x, playerY, pos.z);
        player.tag = Constants.TagConstans.HeroTag;
        var rend = player.GetComponent<Renderer>();
        if (_heroMaterial != null)
            rend.material = new Material(_heroMaterial); // створюємо новий інстанс матеріалу, щоб не ділити його
        else
            rend.material = new Material(Shader.Find("Standard"));

        var rb = player.AddComponent<Rigidbody>();
        rb.mass = 1f;
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        var sh = player.AddComponent<Hero>();
        sh.SettoShowAvailableMovementSquares();
        sh.SetGroundObject(_ground);
        sh.SetStartingWeapons(_startingWeapons);
        sh.SetBulletPrefab(_bulletPrefab);
        sh.SetHighlightMaterial(_gridMaterialHighlited);

        GameObject shootPoint = new GameObject("ShootPoint");
        shootPoint.transform.SetParent(player.transform);
        shootPoint.transform.localPosition = Vector3.up * 0.5f;
        sh.SetShootPoint(shootPoint.transform);

        var hms = player.AddComponent<HeroMovement>();
        hms.SetMoveSpeed(3);

        player.transform.SetParent(_HeroesCollectionGUI.transform);
        return sh;
    }
}