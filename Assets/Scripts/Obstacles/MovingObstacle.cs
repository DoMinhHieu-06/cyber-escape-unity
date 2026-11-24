using UnityEngine;

namespace CyberEscape.Obstacles
{
    public class MovingObstacle : Obstacle
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveRange = 3f;
        [SerializeField] private float moveSpeed = 3f;

        private float startX;

        private void Start()
        {
            startX = transform.position.x;
        }

        private void Update()
        {
            float newX = startX + Mathf.Sin(Time.time * moveSpeed) * moveRange;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}
