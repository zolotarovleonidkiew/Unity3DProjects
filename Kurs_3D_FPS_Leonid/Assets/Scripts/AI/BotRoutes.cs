using System.Collections.Generic;
using UnityEngine;

public class BotRoutes
{
    /// <summary>
    /// waypoint reached - bool
    /// waypoint coord - GameObject
    /// </summary>
    public List<Waypoint> WalkRoute { get; set; }

    public BotRoutes(GameObject waypointsCollection)
    {
        WalkRoute = new List<Waypoint>();

        int childerCount = waypointsCollection.transform.childCount;

        for (int i = 0; i < waypointsCollection.transform.childCount; i++)
        {
            var child = waypointsCollection.transform.GetChild(i).gameObject;

            child.transform.position = new Vector3(child.transform.position.x, 1.57282f, child.transform.position.z);

            var wp = new Waypoint(i, child);

            WalkRoute.Add(wp);
        }
    }
}
