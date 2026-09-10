
using UnityEngine;

[System.Serializable]
public class HeroWeapon
{
    public WeaponData data;

    public HeroWeapon(WeaponData weaponData)
    {
        data = weaponData != null ? Object.Instantiate(weaponData) : null;
    }

    public bool CanShoot()
    {
        return data != null && data.currentAmmoCount > 0;
    }

    public void Shoot()
    {
        if (CanShoot())
            data.currentAmmoCount--;
    }

    public void Reload()
    {
        if (data == null || data.currentAmmoCount == data.maxAmmoPerMagazine || data.currentMagazines <= 0)
            return;

        data.currentMagazines--;
        data.currentAmmoCount = data.maxAmmoPerMagazine;
    }

}