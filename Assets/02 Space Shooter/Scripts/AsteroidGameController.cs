using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;
using System.Collections;

namespace Scripts
{
    /// <summary>
    /// Game controller handling asteroids and intersection of components.
    /// </summary>
    public class AsteroidGameController : MonoBehaviour
    {
        public Asteroid[] bigAsteroids;
        public Asteroid[] mediumAsteroids;
        public Asteroid[] smallAsteroids;

        [SerializeField] private Vector3 maximumSpeed, maximumSpin;
        [SerializeField] private PlayerShip playerShip;
        [SerializeField] private Transform spawnAnchor;

        private List<Asteroid> activeAsteroids;
        private Random random;

        private bool onTimeOut;
        public bool gameStarted;
        public bool gameOver;

        private void Start()
        {
            gameStarted = false;
            gameOver = false;
        }

        public void Update()
        {
            if (!gameStarted && Input.GetKeyDown(KeyCode.X))
            {
                gameStarted = true;
                gameOver = false;
                playerShip.gameReset = true;
                FindObjectOfType<AsteroidGameManager>().GameStart();
                StartGame();
                
            }

            if (gameStarted && (activeAsteroids.Count < 1))
            {
                gameStarted = false;
                FindObjectOfType<AsteroidGameManager>().GameWon();
            }

            if (gameOver && gameStarted)
            {
                gameStarted = false;
                FindObjectOfType<AsteroidGameManager>().GameOver();
            }
        }

        public void StartGame()
        {
            if (activeAsteroids != null)
            {
                //activeAsteroids.Clear();
                // clear the current astroid array to create new ones on reset
                // System.Array.Clear(activeAsteroids,0,activeAsteroids.Count);
                foreach (var asteroid in activeAsteroids)
                {
                    Destroy(asteroid.gameObject);
                }
                activeAsteroids.Clear();
            }
            activeAsteroids = new List<Asteroid>();
            random = new Random();
            // spawn some initial asteroids
            for (var i = 0; i < 5; i++)
            {
                SpawnAsteroid(bigAsteroids, Camera.main.OrthographicBounds());
            }
        }

        /// <summary>
        /// Behaviour to spawn an asteroid within the screen
        /// If there is a parent given, the velocity of that parent is put into consideration
        /// </summary>
        private void SpawnAsteroid(Asteroid[] prefabs, Bounds inLocation, Asteroid parent = null)
        {
            // get a random prefab from the list
            var prefab = prefabs[random.Next(prefabs.Length)];
            // create an instance of the prefab
            var newObject = Instantiate(prefab, spawnAnchor);
            // position it randomly within the box given (either the parent asteroid or the camera)
            newObject.transform.position = RandomPointInBounds(inLocation);
            // we can randomly invert the x/y scale to mirror the sprite. This creates overall more variety
            newObject.transform.localScale = new Vector3(UnityEngine.Random.value > 0.5f ? -1 : 1,
                UnityEngine.Random.value > 0.5f ? -1 : 1, 1);
            // renaming, I'm also sometimes lazy typing
            var asteroidSprite = newObject.spriteRenderer;

            // try to position the asteroid somewhere where it doesn't hit the player or another active asteroid
            for (var i = 0;
                playerShip.shipSprite.bounds.Intersects(asteroidSprite.bounds) ||
                activeAsteroids.Any(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(asteroidSprite.bounds));
                i++)
            {
                // give up after 15 tries.
                if (i > 15)
                {
                    DestroyImmediate(newObject.gameObject);
                    return;
                }

                newObject.transform.position = RandomPointInBounds(inLocation);
            }
            
            // take parent velocity into consideration
            if (parent != null)
            {
                var offset = parent.transform.position - newObject.transform.position;
                var parentVelocity = parent.movementObject.CurrentVelocity.magnitude *
                                     (UnityEngine.Random.value * 0.4f + 0.8f);
                newObject.movementObject.Impulse(offset.normalized * parentVelocity, RandomizeVector(maximumSpeed));
            }
            // otherwise randomize just some velocity
            else
            {
                newObject.movementObject.Impulse(RandomizeVector(maximumSpeed), RandomizeVector(maximumSpin));
            }

            activeAsteroids.Add(newObject);
        }



        /// <summary>
        /// Checks if a laser is intersecting with an asteroid and executes gameplay behaviour on that
        /// </summary>
        public void LaserIntersection(SpriteRenderer laser)
        {
            if (!gameStarted) return;
            
            // go through all asteroids, check if they intersect with a laser and stop after the first
            var asteroid = activeAsteroids
                .FirstOrDefault(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(laser.bounds));

            // premature exit: this laser hasn't hit anything
            if (asteroid == null)
            {
                return;
            }
            
            // otherwise remove the asteroid from the tracked asteroid
            activeAsteroids.Remove(asteroid);
            var bounds = asteroid.spriteRenderer.bounds;
            // get the correct set of prefabs to spawn asteroids in place of the asteroid that now explodes
            var prefabs = asteroid.asteroidSize switch
            {
                AsteroidSize.Large => mediumAsteroids,
                AsteroidSize.Medium => smallAsteroids,
                _ => null
            };
            // remote the asteroid gameobject with all its components
            Destroy(asteroid.gameObject);
            // premature exit: we have no prefabs (ie: small asteroids exploding)
            if (prefabs == null)
            {
                return;
            }

            // randomize two to six random asteroids
            var objectCountToSpawn = (int) (UnityEngine.Random.value * 4 + 2);
            for (var i = 0; i < objectCountToSpawn; i++)
            {
                SpawnAsteroid(prefabs, bounds);
            }
        
            // oh, also get rid of the laser now
            Destroy(laser.gameObject);
        }

        public void ShipIntersection(SpriteRenderer ship)
        {
            if (!gameStarted) return;
            
            // :thinking: this could be solved very similarly to a laser intersection
            // go through all asteroids, check if they intersect with a laser and stop after the first
            var asteroid = activeAsteroids
                .FirstOrDefault(x => x.GetComponent<SpriteRenderer>().bounds.Intersects(ship.bounds));
            
            
            if (asteroid == null)
            {
                // No asteroid is hitting the ship
                return;
            }

            if (onTimeOut)
            {
                return;
            }

            playerShip.takeDamage(10);
            onTimeOut = true;
            StartCoroutine(DamageTimeOut());
        }

        // Using a coroutine to timeout the damage on the ship, otherwise the hpo would decrease almost instantly due to the collision detection
        IEnumerator DamageTimeOut()
        {
            yield return new WaitForSeconds(1);
            onTimeOut = false;
        }

        private static float RandomPointOnLine(float min, float max)
        {
            return UnityEngine.Random.value * (max - min) + min;
        }

        private static Vector2 RandomPointInBox(Vector2 min, Vector2 max)
        {
            return new Vector2(RandomPointOnLine(min.x, max.x), RandomPointOnLine(min.y, max.y));
        }

        private static Vector2 RandomPointInBounds(Bounds bounds)
        {
            return RandomPointInBox(bounds.min, bounds.max);
        }

        private static Vector3 RandomizeVector(Vector3 maximum)
        {
            // that is an inline method - it's good enough to just get a float [-1...+1]
            float RandomValue()
            {
                return UnityEngine.Random.value - 0.5f * 2;
            }

            maximum.Scale(new Vector3(RandomValue(), RandomValue(), RandomValue()));
            return maximum;
        }
    }
}