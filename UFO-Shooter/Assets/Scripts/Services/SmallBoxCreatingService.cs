using UnityEngine;

/// <summary>
/// Service responsible for creating small box GameObjects (when required) + Default Waypoint creation
/// </summary>
public class SmallBoxCreatingService
{
    /// <summary>
    /// Create Small Boxes + WayPoints
    /// </summary>
    public GameObject CreateSmallBox(
        string name,
        Vector3 position,
        Vector3 scale,
        Transform parent,
        Material material,
        int x,
        int y,
        bool useLocalPosition = false,
        bool hideRenderer = true)
    {
        GameObject smallBox = GameObject.CreatePrimitive(PrimitiveType.Cube);
        smallBox.name = name;

        if (useLocalPosition)
        {
            smallBox.transform.SetParent(parent, false);
            smallBox.transform.localPosition = position;

            Vector3 parentScale = parent.lossyScale;
            smallBox.transform.localScale = new Vector3(
                scale.x / parentScale.x,
                scale.y / parentScale.y,
                scale.z / parentScale.z);
        }
        else
        {
            smallBox.transform.localScale = scale;
            smallBox.transform.position = position;
            smallBox.transform.SetParent(parent);
        }

        smallBox.tag = Constants.TagConstans.FloorGridTag;

        var renderer = smallBox.GetComponent<Renderer>();
        renderer.material = material;
        renderer.enabled = !hideRenderer;

        smallBox.GetComponent<BoxCollider>().isTrigger = true;

        //Default Waypoint creation
        var wayPoint = smallBox.AddComponent<WayPoint>();
        wayPoint.CreateDefaultWayPoint(x, y);

        return smallBox;
    }
}