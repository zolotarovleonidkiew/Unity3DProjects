using Assets.Scripts.TacticMap;
using UnityEngine;
using static ObstacleFactory;

public class LandCreatorConfig
{
    public GridConfig GridConfig {get; set;}
    public ObstacleConfig ObstacleConfig {get; set;}
    public LiftConfig LiftConfig{get; set;}
    public GameObject HeroesCollectionGUI { get; set; }
}