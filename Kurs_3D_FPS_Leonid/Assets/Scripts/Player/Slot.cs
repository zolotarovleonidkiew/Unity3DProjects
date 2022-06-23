using System;

[Serializable]
public class Slot
{
    public int index { get; private set; }
    public bool isActive;
    public int ammo;

    public Slot(int i)
    {
        isActive = false;
        ammo = 0;
        index = i;
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
        this.ammo += ammo;
    }
}