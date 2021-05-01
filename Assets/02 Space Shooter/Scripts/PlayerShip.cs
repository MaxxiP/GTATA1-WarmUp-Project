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
                takeDamage(10);
            }
        }

        public void takeDamage(int damage)
        {
            currentHealth -= damage;
            healthbar.SetHealth(currentHealth);
        }

        private void LateUpdate()
        {
            _runGameController.ShipIntersection(shipSprite);
        }

        private void OnCollisionEnter(Collision collision)
        {
            Debug.Log(collision);
            Debug.Log("I hit smth");
        }
    }
}