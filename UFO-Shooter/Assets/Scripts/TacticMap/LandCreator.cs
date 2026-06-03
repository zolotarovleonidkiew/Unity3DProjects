using Assets.Scripts.TacticMap.V05;
using Assets.Scripts.TacticMap.V05.Interfaces;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Creates land for the tactic map. This includes: platforms, obstacls, ramps, etc.
/// </summary>
public class LandCreator : ILandCreator
{
    public readonly IGridGenerator GridGenerator;
    public readonly IObstacleGenerator ObstacleGenerator;
    public readonly ILiftGenerator LiftGenerator;

    public LandCreator(IGridGenerator gridGenerator, IObstacleGenerator obstacleGenerator, ILiftGenerator  liftGenerator)
    {
        GridGenerator = gridGenerator;
        ObstacleGenerator = obstacleGenerator;
        LiftGenerator = liftGenerator;  
    }

    public IGroundHierarchy CreateLand(LandCreatorConfig landCreatorConfig)
    {
        var hierarchy = new GroundHierarchy { GroudLayers = new List<GroundLayer>() };

        // create main ground layer
        var mainLayer = GridGenerator.CreatePlatorm(landCreatorConfig.GridConfig, GroundHierarchyLevel.Level_1, "Main_Floor");
        hierarchy.GroudLayers.Add(mainLayer);

        var groundSmallBoxes = mainLayer.SmallBoxed;

        // create obstacles & ramps (they may modify smallBoxes)
        ObstacleGenerator.CreateObstaclesAndRamps(landCreatorConfig.GridConfig, mainLayer.GroundLevelObject, groundSmallBoxes, landCreatorConfig.ObstacleConfig);

        // create second-floor platforms for lifts (done by grid generator so generator owns platform creation)
        var liftLayers = GridGenerator.CreateUpperPlatform(landCreatorConfig.GridConfig, landCreatorConfig.LiftConfig, groundSmallBoxes);
        foreach (var ly in liftLayers)
        {
            hierarchy.GroudLayers.Add(ly.Value);
        }

        // now create the lifts themselves (buttons + lift platforms) - generator will use existing liftLayers for upper-floor info
        try
        {
            // Force enumeration so GenerateLifts body executes (yield-return is deferred)
            var lifts = LiftGenerator.GenerateLifts(landCreatorConfig.LiftConfig, groundSmallBoxes, liftLayers, landCreatorConfig.HeroesCollectionGUI).ToList();
        }
        catch (System.Exception ex)
        {
            UnityEngine.Debug.LogError($"[LandCreator] Error creating lifts: {ex.Message}");
        }   

        return hierarchy;
    }
}