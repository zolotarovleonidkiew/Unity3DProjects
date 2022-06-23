using UnityEngine;

public class Waypoint
{
    public int Index { get; private set; }
    public bool WaypointReached { get; private set; }
    public GameObject WaypointObject { get; private set; }

    public Waypoint(int i, GameObject wp)
    {
        Index = i;
        WaypointReached = false;
        WaypointObject = wp;
    }

    public void SetWaypointReached(bool isReached)
    {
        WaypointReached = isReached;
    }
   
}
