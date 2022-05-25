using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents viking Ulrick
/// TO DO:
/// Try to remove via all logic in CharacterController2D
/// </summary>
public class Viking1_Ulrick : Character
{    
    public Viking1_Ulrick() : base("Ulrick", CharacterType.viking_1)
    {
        CanJump = true;
    }

    /// <summary>
    /// Особое действие - прыгнуть
    /// </summary>
    public override void UseSpecificAction()
    {
        base.UseSpecificAction();
    }
}
