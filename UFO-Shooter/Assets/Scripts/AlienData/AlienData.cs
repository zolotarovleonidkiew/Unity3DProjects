/// <summary>
/// Basic alien data class. All alien types inherit from this class.
/// </summary>
public abstract class AlienData
{
    public AlienTypesEnum AlienType;
    public int Health;
    public int Damage;
    public float Speed;
    public float DetectionRange;
    public float AttackCooldown = 1f;
    public bool CanFly = false;
    public float FlyHeight = 0f;
}


/// <summary>
/// Alien 1 type - Greys
/// </summary>
public class AlienGreys : AlienData
{
    public AlienGreys()
    {
        AlienType = AlienTypesEnum.level_0_Greys;
        Health = 3;
        Damage = 5;
        Speed = 4f;
        DetectionRange = 8f;
        AttackCooldown = 1f;
        CanFly = false;
    }
}

/// <summary>
/// Alien 2 type - Light Drone
/// </summary>
public class AlienLightDrone : AlienData
{
    public AlienLightDrone()
    {
        AlienType = AlienTypesEnum.level_0_Light_Drone;
        Health = 6;
        Damage = 5;
        Speed = 8f;
        DetectionRange = 15f;
        AttackCooldown = 1f;
        CanFly = true;
        FlyHeight = 2f;
    }
}

/// <summary>
/// Alien 3 type - Heavy Drone
/// </summary>
public class AlienHeavyDrone : AlienData
{
    public AlienHeavyDrone()
    {
        AlienType = AlienTypesEnum.level_1_Heavy_Drone;
        Health = 15;
        Damage = 10;
        Speed = 6f;
        DetectionRange = 10f;
        AttackCooldown = 1f;
        CanFly = true;
        FlyHeight = 2f;
    }
}

/// <summary>
/// Alien 4 type - Assault Trooper
/// </summary>
public class AlienAssaultTrooper : AlienData
{
    public AlienAssaultTrooper()
    {
        AlienType = AlienTypesEnum.level_2_Assault_Trooper;
        Health = 20;
        Damage = 10;
        Speed = 3f;
        DetectionRange = 8f;
        AttackCooldown = 1f;
        CanFly = false;
    }
}

/// <summary>
/// Alien 5 type - Insect
/// </summary>
public class AlienInsect : AlienData
{
    public AlienInsect()
    {
        AlienType = AlienTypesEnum.level_2_Insect;
        Health = 6;
        Damage = 15;
        Speed = 12f;
        DetectionRange = 8f;
        AttackCooldown = 1f;
        CanFly = false;
    }
}

/// <summary>
/// Alien 6 type - Assault Machine
/// </summary>
public class AlienAssaultMachine : AlienData
{
    public AlienAssaultMachine()
    {
        AlienType = AlienTypesEnum.level_3_Assault_Machine;
        Health = 30;
        Damage = 20;
        Speed = 2f;
        DetectionRange = 15f;
        AttackCooldown = 1f;
        CanFly = false;
    }
}

/// <summary>
/// Alien 7 type - Master Mind
/// </summary>
public class AlienMasterMind : AlienData
{
    public AlienMasterMind()
    {
        AlienType = AlienTypesEnum.level_3_Master_Mind;
        Health = 15;
        Damage = 15;
        Speed = 2f;
        DetectionRange = 20f;
        AttackCooldown = 1f;
        CanFly = true;
        FlyHeight = 2f;
    }
}

/// <summary>
/// Alien 8 type - Insect Matriarch
/// </summary>
public class AlienInsectMatriarch : AlienData
{
    public AlienInsectMatriarch()
    {
        AlienType = AlienTypesEnum.level_4_Insect_Matriarch;
        Health = 25;
        Damage = 10;
        Speed = 31f;
        DetectionRange = 25f;
        AttackCooldown = 1f;
        CanFly = false;
    }
}