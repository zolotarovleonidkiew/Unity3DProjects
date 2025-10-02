using System;
using UnityEngine;

/// <summary>
/// Заїзд на перешкоду (дім, холм та ін.)
/// </summary>
[Serializable]
public class Ramp
{
    public int i;
    public int j;
    public RampDirection direction;
    public Material material;

    [HideInInspector]
    public Transform parent;

    public Ramp(int i, int j, RampDirection direction, Transform parent, Material material)
    {
        this.i = i;
        this.j = j;
        this.direction = direction;
        this.parent = parent;
        this.material = material;
    }
}

[Serializable]
public enum RampDirection
{
    North, // знизу догори (вздовж осі Z+)
    South, // згори вниз (вздовж осі Z-)
    East,  // зліва направо (вздовж осі X+)
    West   // справа наліво (вздовж осі X-)
}
