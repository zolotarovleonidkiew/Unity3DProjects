using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{

}

/// <summary>
/// Represents Weapon type's для приибульців та героїв
/// </summary>
public enum WeaponType
{
    // Герой
    Pistol,
    ShotGun,
    Rifle,
    RocketLauncher,
    Lazer_Rifle,
    Lazer_ShotGun,
    Grenade,
    PoisonGrenade,

    // Прибульці
    Alien_SmallHandBlaster,

    // Обидва
    Blaster_Rifle,
    Blaster_ShotGun,
    BlasterGrenade
}

/// <summary>
/// Represents Weapon Owner (приибульці та герої)
/// </summary>
public enum WeaponOwner
{
    Hero,
    Alien,
    Both
}