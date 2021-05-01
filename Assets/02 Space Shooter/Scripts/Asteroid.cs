using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Container component to hold important references
    /// </summary>
    public class Asteroid : MonoBehaviour
    {
        private static AsteroidGameController _runGameController;
        public SpriteRenderer spriteRenderer;
        public MovementObject movementObject;
        public AsteroidSize asteroidSize;

        private void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }
 //       private void LateUpdate()
 //       {
 //           _runGameController.ShipIntersection(spriteRenderer);
 //       }
    }

    public enum AsteroidSize
    {
        Large,
        Medium,
        Small
    }
}