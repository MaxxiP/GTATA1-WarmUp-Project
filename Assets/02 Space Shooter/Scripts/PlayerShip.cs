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

        // On start set the HP of the player to the specified max health value
        //
        private void Start()
        {
            if (_runGameController == null) _runGameController = FindObjectOfType<AsteroidGameController>();
            currentHealth = maxHealth;
            healthbar.SetMaxHealth(maxHealth);
        }

        private void Update()
        {
            // if gameReset is set to true, the health will be set back to the maximum
            if (gameReset)
            {
                healthbar.SetHealth(maxHealth);
                currentHealth = maxHealth;
                gameReset = false;
            }
        }

        // calculate current health by applying damage, if health falls to or below zero the gameOver bool of the gamecontroller gets triggered to end the game
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