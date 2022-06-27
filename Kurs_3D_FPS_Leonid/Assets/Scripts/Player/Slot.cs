using System;
using UnityEngine;

public class Slot
{
    public PickableItemTypes ItemType { get; private set; }
    public int index { get; private set; }
    public bool isActive;
   
    public int Ammo
    {
        get { return ammo; }
        set
        {
            if (value < 0)
            {
                ammo = 0;
                Debug.LogWarning($"Out of ammo ({ItemType})");
            }
            else
            {
                ammo = value;
            }
            //if (ammo >1)
            //    ammo = value;
            //else
            //{
            //    Debug.LogWarning($"Out of ammo ({ItemType})");
            //}
        }
    }
    private int ammo;

    public Slot(int i, PickableItemTypes itemType)
    {
        isActive = false;
        ammo = 0;
        index = i;
        ItemType = itemType;
    }

    public bool GetSlotActive()
    {
        return isActive;
    }

    public void SetSlotActive()
    {
        isActive = true;
    }

    public void AddAmmo(int ammo)
    {
        Ammo += ammo;
    }
}