using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking1_Ulrick : Character
{    
    public Viking1_Ulrick() : base("Ulrick", CharacterType.viking_1)
    {
        CanJump = true;

        //CharacterSprite =  ???
    }

    /// <summary>
    /// Особое действие - прыгнуть
    /// </summary>
    public override void UseSpecificAction()
    {
        base.UseSpecificAction();
    }
}
