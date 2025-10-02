using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Game/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public WeaponType weaponType;
    public int damage;
    public float fireRate;
    public int magazineSize; // напр. 12
    public float reloadTime;

    [Header("Ammo Settings")]
    public int maxAmmoPerMagazine = 10;  // максимальна кількість патронів у магазині
    public int maxMagazines = 3;         // скільки магазинів можна носити
    public int currentAmmo;              // поточна кількість патронів у магазині
    public int currentMagazines;         // поточна кількість магазинів


    //OLD
    //public WeaponType weaponType;
    //public WeaponOwner owner;

    //[Header("Ammo Settings")]
    //public int maxAmmoPerMagazine = 10;  // максимальна кількість патронів у магазині
    //public int maxMagazines = 3;         // скільки магазинів можна носити
    //public int currentAmmo;              // поточна кількість патронів у магазині
    //public int currentMagazines;         // поточна кількість магазинів

    //[Header("References")]
    //public GameObject prefab;            // префаб зброї
    //public AnimationClip fireAnimation;  // анімація пострілу
    //public AudioClip fireSound;          // звук пострілу

    //public void Reload()
    //{
    //    if (currentMagazines > 0)
    //    {
    //        currentMagazines--;
    //        currentAmmo = maxAmmoPerMagazine;
    //    }
    //}

    //public bool CanShoot()
    //{
    //    return currentAmmo > 0;
    //}

    //public void Shoot()
    //{
    //    if (currentAmmo > 0)
    //    {
    //        currentAmmo--;
    //    }
    //}
}