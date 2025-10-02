using System.Collections.Generic;
using UnityEngine;


public class WeaponInventory : MonoBehaviour
{
    //[SerializeField] private List<WeaponData> weapons = new List<WeaponData>();
    //private WeaponData activeWeapon;

    //public void AddWeapon(WeaponData weapon)
    //{
    //    if (!weapons.Contains(weapon))
    //        weapons.Add(weapon);

    //    if (activeWeapon == null)
    //        activeWeapon = weapon;
    //}

    //public void SwitchWeapon(WeaponType type)
    //{
    //    var weapon = weapons.Find(w => w.weaponType == type);
    //    if (weapon != null)
    //    {
    //        activeWeapon = weapon;
    //        Debug.Log($"Switched to {type}");
    //    }
    //}

    //public void Shoot()
    //{
    //    if (activeWeapon == null) return;

    //    if (activeWeapon.CanShoot())
    //    {
    //        activeWeapon.Shoot();
    //        Debug.Log($"{activeWeapon.weaponType} pew! ammo left {activeWeapon.currentAmmo}");
    //    }
    //    else
    //    {
    //        Debug.Log("Click! Out of ammo.");
    //    }
    //}

    //public void Reload()
    //{
    //    if (activeWeapon != null)
    //        activeWeapon.Reload();
    //}

    //public WeaponData GetActiveWeapon() => activeWeapon;
}