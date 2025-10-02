
//NEW
[System.Serializable]
public class HeroWeapon
{
    public WeaponData data;
    public int currentAmmo;

    public HeroWeapon(WeaponData weaponData)
    {
        data = weaponData;
        currentAmmo = weaponData != null ? weaponData.maxAmmoPerMagazine : 0;
    }

    public bool CanShoot()
    {
        return currentAmmo > 0;
    }

    public void Shoot()
    {
        if (currentAmmo > 0)
            currentAmmo--;
    }

    public void Reload()
    {
        if (data != null) currentAmmo = data.maxAmmoPerMagazine;
    }

}