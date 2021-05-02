using System;
using UnityEngine;

namespace Scripts
{
    
    
    /// <summary>
    /// Container component to keep references to common components on a ship
    /// </summary>
    internal class PlayerShip : MonoBehaviour
    {
        private static AsteroidGameController _runGameController;
        
        public HealthBar healthbar;
        
        public MovementObject movementObject;
        public SpriteRenderer shipSprite;

        public int maxHealth = 100;
        public int currentHealth;
        public bool gameReset;

        private void Start()
        {
            if (_runGameController == null) _runGameController = FindObjectOfType<AsteroidGameController>();
            currentHealth = maxHealth;
            healthbar.SetMaxHealth(maxHealth);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                takeDamage(20);
            }

            if (gameReset)
            {
                healthbar.SetHealth(maxHealth);
                currentHealth = maxHealth;
                gameReset = false;
            }
        }

        public void takeDamage(int damage)
        {
            currentHealth -= damage;
            healthbar.SetHealth(currentHealth);

            if (currentHealth <= 0)
            {
                _runGameController.gameOver = true;
            }
        }

        private void LateUpdate()
        {
            _runGameController.ShipIntersection(shipSprite);
        }

    }
}