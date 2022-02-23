using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking2_Olaf : Character
{
    public Viking2_Olaf() : base("Olaf", CharacterType.viking_1)
    {
        CanDefendByShield = true;

        //CharacterSprite =  ???
    }

    /// <summary>
    /// Особое действие - поднять щит
    /// </summary>
    public override void UseSpecificAction()
    {
        base.UseSpecificAction();
    }
}
