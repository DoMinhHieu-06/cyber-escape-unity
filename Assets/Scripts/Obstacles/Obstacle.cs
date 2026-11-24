using UnityEngine;

namespace CyberEscape.Obstacles
{
    public enum ObstacleType
    {
        LowBarrier,   // Must jump over
        HighBarrier,  // Must slide under
        FullBlock,    // Must switch lane
        MovingBlock   // Moves horizontally across lanes
    }

    public class Obstacle : MonoBehaviour
    {
        [SerializeField] private ObstacleType type = ObstacleType.FullBlock;
        public ObstacleType Type => type;

        public void Initialize(ObstacleType obstacleType)
        {
            type = obstacleType;
        }
    }
}
