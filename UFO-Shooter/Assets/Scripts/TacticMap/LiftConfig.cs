using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TacticMap
{
    public class LiftConfig
    {
        public float CellSize { get; set; }
        public float GroundHeight { get; set; }            // original bigHeight
        public float SmallBoxHeight { get; set; }          // originally SmallBoxHeight
        public float LiftAboveGround { get; set; }         // originally LiftAboveGround
        public List<Vector2Int> LiftPositions { get; set; }
        public Transform Parent { get; set; }
        public Material LiftMaterial { get; set; }
        public Material GridBoxMaterial  { get; set; }
        public Material GroundBoxMaterial { get; set; }

        public LiftConfig(float cellSize, float groundHeight, float smallBoxHeight, List<Vector2Int> liftPositions, 
            Transform parent, Material liftMaterial, Material gridBoxMaterial, Material groundBoxMaterial)
        {
            CellSize = cellSize;
            GroundHeight = groundHeight;
            SmallBoxHeight = smallBoxHeight;
            LiftPositions = liftPositions ?? new List<Vector2Int>();
            Parent = parent;
            LiftMaterial = liftMaterial; 
            // reasonable default if small box height wasn't set on GridConfig
            if (SmallBoxHeight <= 0f) SmallBoxHeight = 0.5f;
            // default lift offset above ground (kept similar to previous hard-coded value)
            LiftAboveGround = 0.3f;
            GridBoxMaterial = gridBoxMaterial;
            GroundBoxMaterial = groundBoxMaterial;
        }
    }
}