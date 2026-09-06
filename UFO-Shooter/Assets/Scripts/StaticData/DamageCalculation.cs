using UnityEngine;

public static class DamageCalculation
{
    public static int CalculateDamage(WeaponData weaponData)
    {        
        int minDamage = weaponData.minDamage;
        int maxDamage = weaponData.maxDamage;

        int damage = Random.Range(minDamage, maxDamage + 1 );

        //TO DO:
        //1. Add 10% luck chance to double damage
        //2. Add check for flang attack: return maxDamage;

        return damage;
    }
}