namespace Assets.Scripts.TacticMap.V05.Interfaces
{
    public interface ILandCreator
    {
        public IGroundHierarchy CreateLand(LandCreatorConfig landCreatorConfig);
    }
}