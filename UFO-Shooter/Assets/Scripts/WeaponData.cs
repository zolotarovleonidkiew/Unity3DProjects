using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;     // Имя оружия
    public WeaponType weaponType; // Тип
    public int minDamage;         // Мин ...
    public int maxDamage;         // и Макс урон
    public bool hasRadiusOfDamage; // Flag - наносит урон по площади?
    public float RadiusOfDamage;   // Радиус урона по площади

    [Header("Ammo Settings")]    
    public int maxMagazines;             // MAX магазинів можна носити
    public int currentMagazines;         // залишилось магазинів
    public int maxAmmoPerMagazine;       // MAX кількість патронів у магазині
    public int currentAmmoCount;         // залишилось патронів у магазині
    

    //Visuals
    [SerializeField] private Sprite spriteWeapon;
    public Sprite SpriteWeapon => spriteWeapon;
    [SerializeField] private GameObject bulltPrefab; //TO DO: add bullet prefab to weapon data
    public GameObject BulltPrefab => bulltPrefab;
}