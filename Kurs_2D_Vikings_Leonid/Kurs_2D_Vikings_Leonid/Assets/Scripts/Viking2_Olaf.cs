using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents viking Olaf
/// TO DO:
/// Try to remove via all logic in CharacterController2D
/// </summary>
public class Viking2_Olaf : Character
{
    public Viking2_Olaf() : base("Olaf", CharacterType.viking_1)
    {
        CanDefendByShield = true;
    }

    /// <summary>
    /// Особое действие - поднять щит
    /// </summary>
    public override void UseSpecificAction()
    {
        base.UseSpecificAction();
    }
}
