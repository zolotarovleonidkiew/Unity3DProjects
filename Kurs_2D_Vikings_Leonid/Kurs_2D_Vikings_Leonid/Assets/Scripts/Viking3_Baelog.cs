using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viking3_Baelog : Character
{
    public Viking3_Baelog() : base("Baelog", CharacterType.viking_1)
    {
        CanShoot = true;

        //CharacterSprite =  ???
    }

    /// <summary>
    /// ������ �������� - ����������
    /// </summary>
    public override void UseSpecificAction()
    {
        base.UseSpecificAction();
    }
}
