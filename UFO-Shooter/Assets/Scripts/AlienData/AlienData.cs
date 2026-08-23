using UnityEngine;

[CreateAssetMenu(fileName = "AlienData", menuName = "Scriptable Objects/AlienData")]
public class AlienData : ScriptableObject
{
    //public AlienType alienType = AlienType.level_0_Greys; //defualt
    //public int health;
    //public int damage;
    //public float speed;
    //public float detectionRange;
    //public float attackCooldown = 1f;
    //public bool canFly = false;
}

public enum AlienType
{
    level_0_Greys,
    level_0_Light_Drone,
    level_1_Heavy_Drone,
    level_2_Assault_Trooper,
    level_2_Insect,
    level_3_Assault_Machine,
    level_3_Master_Mind,
    level_4_Insect_Matriarch,
}

/// <summary>
/// Alien 1 type - Greys
/// </summary>
public class AlienGreys : AlienData
{
    //public int health = 5;
    //public int damage = 5;
    //public float speed = 3f;
    //public float detectionRange = 15f;
    //public float attackCooldown = 1f;
}

public class AlienLightDrone : AlienData
{
    //public int health = 5;
    //public int damage = 5;
    //public float speed = 3f;
    //public float detectionRange = 15f;
    //public float attackCooldown = 1f;
}

//TO DO