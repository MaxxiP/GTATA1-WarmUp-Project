using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Scripts
{
    /// <summary>
    /// Controls the movement of the Character
    /// </summary>
    public class RunCharacterController : MonoBehaviour
    {
        public Transform Transform => character;
        public SpriteRenderer CharacterSprite => characterSprite;
        
        
        private Animator animator;
        [SerializeField] public Animator characterAnimator;
        private string currentAnimation;

        [SerializeField] public AudioSource walkingSound;
        [SerializeField] public AudioSource jumpSound;
        public AudioSource audioSource;
        
        public AudioClip[] characterSounds;
        
        // Animations
        private const string IDLE = "alienBlueIdle";
        private const string RUN = "alienBlueWalking";
        private const string JUMP = "alienBlueJump";
        private const string DEATH = "alienBlueDeath";
        
        /// <summary>
        /// Since the Character controller takes responsibility for triggering Input events, it also emits an
        /// event when it does so
        /// </summary>
        public Action onJump;
        
        [SerializeField] private float jumpHeight;
        [SerializeField] private float jumpDuration;
        /// <summary>
        /// Unity handles Arrays and Lists in the inspector correctly (but not Maps, Dictionaries or other Collections)
        /// </summary>
        [SerializeField] private KeyCode[] jumpKeys;
        /// <summary>
        /// We don't require anything else from the Character than its transform
        /// </summary>
        [SerializeField] private Transform character;
        [SerializeField] private SpriteRenderer characterSprite;
        [SerializeField] private AnimationCurve jumpPosition;
        [SerializeField] private RunGameManager gameManager;
        
        private bool canJump = true;

        private bool /*isRunning, isDead,*/ isInAir;

        public void Start()
        {
            // Condition not really worked out yet
            //isDead = false;
            //isRunning = true;
            isInAir = false;
            
            
            animator = characterAnimator.GetComponent<Animator>();
            walkingSound = GetComponent<AudioSource>();
        }

        /// <summary>
        /// Update is a Unity runtime function called *every rendered* frame before Rendering happens
        /// see: https://docs.unity3d.com/Manual/ExecutionOrder.html
        /// </summary>
        private void Update()
        {
            // Start the walking animation if the game hasStarted is true
            if (gameManager.hasStarted && canJump)
            {
                isInAir = false;
                ChangeAnimation(RUN);

                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(characterSounds[0], 0.5f);
                }
            }
            else if (!canJump)
            {
                ChangeAnimation(JUMP);


                if (!isInAir && !audioSource.isPlaying)
                {
                    isInAir = true;
                    audioSource.PlayOneShot(characterSounds[1], 0.5f);

                }
            }
            
            // Iff collidied is true, the player died, play death anim
            if (gameManager.collided)
            {
                ChangeAnimation(DEATH);
                
                // logic not implemented yet
                /*
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(characterSounds[2], 0.5f);
                }
                */
            }
            // If the player neither collided, nor the game hast started
            // the game is paused, so play idle
            else if (!gameManager.collided && !gameManager.hasStarted)
            {
                ChangeAnimation(IDLE);
            }
            

            if (!canJump)
            {
                return;
            }
            // here the input event counts - if there is any button pressed that were defined as jump keys, trigger a jump
            if (jumpKeys.Any(x => Input.GetKeyDown(x)))
            {   // first we disable the jump, then start the Coroutine that handles the jump and invoke the event
                canJump = false;
                StartCoroutine(JumpRoutine());
                onJump?.Invoke();
            }
        }

        /// <summary>
        /// OnDrawGizmosSelected is a Unity editor function called when the attached GameObject is selected and used to
        /// display debugging information in the Scene view
        /// see: https://docs.unity3d.com/Manual/ExecutionOrder.html
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            var upScale = transform.lossyScale;
            upScale.Scale(transform.up);
            Gizmos.DrawLine(transform.position, Vector3.up * jumpHeight * upScale.magnitude);
        }

        /// <summary>
        /// Handles the jump of a character
        /// 
        /// To be used in an Coroutine, this function is a generator (return IEnumerator) and has special syntactic
        /// sugar with "yield return"
        /// </summary>
        private IEnumerator JumpRoutine()
        {
            // the time this coroutine runs
            var totalTime = 0f;
            // low position is assumed to be a (0, 0, 0)
            var highPosition = character.up * jumpHeight;
            while (totalTime < jumpDuration)
            {
                totalTime += Time.deltaTime;
                // what's the normalized time [0...1] this coroutine runs at
                var sampleTime = totalTime / jumpDuration;
                // Lerp is a Linear Interpolation between a...b based on a value between 0...1
                character.localPosition = Vector3.Lerp(Vector3.zero, highPosition, jumpPosition.Evaluate(sampleTime));
                // we enable jumping again after we're almost done to remove some "stuck" behaviour when landing down
                if (sampleTime > 0.95f)
                {
                    canJump = true;
                }
                // yield return null waits a single frame
                yield return null;
            }
        }

        private void ChangeAnimation(string animation)
        {

            // return if the animation to be played is already playing
            if (currentAnimation == animation) return;
            //Debug.Log("Now changing animation to: " + animation);
            
            animator.Play(animation);

            currentAnimation = animation;
        }
    }
}