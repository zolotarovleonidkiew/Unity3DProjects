using static ObstacleFactory;

namespace Assets.Scripts.TacticMap.V04
{
    public interface IObstacleFactory
    {
        public void CreateObstacle(ObstacleConfig obstacleConfig);
    }
}