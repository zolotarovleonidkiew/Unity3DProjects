using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents viking Baealog
/// TO DO:
/// Try to remove via all logic in CharacterController2D
/// </summary>
public class Viking3_Baelog : Character
{
    public Viking3_Baelog() : base("Baelog", CharacterType.viking_1)
    {
        CanShoot = true;
    }

    /// <summary>
    /// Особое действие - выстрелить
    /// </summary>
    public override void UseSpecificAction()
    {
        base.UseSpecificAction();
    }
}
