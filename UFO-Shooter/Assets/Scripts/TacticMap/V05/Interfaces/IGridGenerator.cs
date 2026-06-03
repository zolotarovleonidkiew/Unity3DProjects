using System.Collections.Generic;

using UnityEngine;

namespace Assets.Scripts.TacticMap.V05
{
    public interface IGridGenerator
    {
        /// <summary>
        /// Creates empty platform/layer with small boxes (WITHOUT obstacles etc.)
        /// </summary>
        /// <param name="config">Layer parameters</param>
        /// <param name="level">Hierarchy leve</param>
        /// <param name="name">Name</param>
        GroundLayer CreatePlatorm(GridConfig config, GroundHierarchyLevel level, string name);

        /// <summary>
        /// Creates upper-floor platforms for lifts. 
        /// Uses liftConfig for positions and dimensions, and groundSmallBoxes to match positions. 
        /// Returns created layers (with small boxes) mapped by their grid position. 
        /// May be empty if no lifts or if no matching small-boxes found. 
        /// Note that platforms are created even if no lift will be placed on them (lift placement is done later by ILiftGenerator), so returned layers may be used by ILiftGenerator to compute lift button positions. 
        /// Also note that returned layers may be used by ObstacleGenerator to place obstacles on upper floor, so they should be created before obstacles.
        /// </summary>
        /// <param name="groundConfig">Layer parameters</param>
        /// <param name="liftConfig">Lifts parameters</param>
        /// <param name="groundSmallBoxes">small-boxes</param>
        public IDictionary<Vector2Int, GroundLayer> CreateUpperPlatform(GridConfig groundConfig, LiftConfig liftConfig, GameObject[,] groundSmallBoxes);
    }
}