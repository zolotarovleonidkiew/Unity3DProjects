using System.Collections.Generic;

public interface IGroundHierarchy
{
    /// <summary>
    /// Main (and single!) component for Layers and their small boxes.
    /// </summary>
    public List<GroundLayer> GroudLayers { get; set; }

    /// <summary>
    /// Returns the GroundLayer matching the provided GroundHierarchyLevel.
    /// </summary>
    /// <param name="GroundLevel">The target ground hierarchy level to find.</param>
    /// <returns>The matching GroundLayer or null if not found.</returns>
    public GroundLayer GetGroundLayer(GroundHierarchyLevel GroundLevel);
}