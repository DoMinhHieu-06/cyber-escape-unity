using System.Collections;
using UnityEngine;
using CyberEscape.Core;
using CyberEscape.Audio;

namespace CyberEscape.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Lane Settings")]
        [SerializeField] private float laneDistance = 3f;
        [SerializeField] private float laneChangeSpeed = 15f;
        private int currentLane = 1; // 0 = Left (-laneDistance), 1 = Middle (0), 2 = Right (+laneDistance)

        [Header("Jump & Physics Settings")]
        [SerializeField] private float jumpHeight = 2.5f;
        [SerializeField] private float gravity = 25f;
        private float verticalVelocity = 0f;

        [Header("Slide Settings")]
        [SerializeField] private float slideDuration = 0.75f;
        [SerializeField] private float slideColliderHeight = 0.8f;
        [SerializeField] private Vector3 slideColliderCenter = new Vector3(0, 0.4f, 0);
        private bool isSliding = false;
        private float originalColliderHeight;
        private Vector3 originalColliderCenter;
        private Vector3 originalScale;

        [Header("Visual Feedback")]
        [SerializeField] private GameObject shieldVisual;
        [SerializeField] private MeshRenderer playerRenderer;

        private CharacterController controller;
        private Vector3 targetPosition;

        private void Awake()
        {
            controller = GetComponent<CharacterController>();
            originalColliderHeight = controller.height;
            originalColliderCenter = controller.center;
            originalScale = transform.localScale;

            if (shieldVisual != null)
            {
                shieldVisual.SetActive(false);
            }
        }

        private void Start()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnShieldChanged += HandleShieldChanged;
            }
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnShieldChanged -= HandleShieldChanged;
            }
        }

        private void Update()
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
                return;

            HandleInput();
            ApplyMovement();
        }

        private void HandleInput()
        {
            // Lane Switch: Left
            if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && currentLane > 0)
            {
                currentLane--;
                SoundManager.Instance?.PlayLaneChangeSound();
            }

            // Lane Switch: Right
            if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && currentLane < 2)
            {
                currentLane++;
                SoundManager.Instance?.PlayLaneChangeSound();
            }

            // Jump
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow)) && controller.isGrounded && !isSliding)
            {
                verticalVelocity = Mathf.Sqrt(2 * jumpHeight * gravity);
                SoundManager.Instance?.PlayJumpSound();
            }

            // Slide
            if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && !isSliding)
            {
                StartCoroutine(PerformSlide());
                SoundManager.Instance?.PlaySlideSound();
            }
        }

        private void ApplyMovement()
        {
            // Target X position based on current lane
            float targetX = (currentLane - 1) * laneDistance;
            float newX = Mathf.MoveTowards(transform.position.x, targetX, laneChangeSpeed * Time.deltaTime);

            // Gravity & Vertical Velocity
            if (controller.isGrounded)
            {
                if (verticalVelocity < 0) verticalVelocity = -1f;
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            // Forward speed from GameManager
            float forwardSpeed = GameManager.Instance.CurrentSpeed;

            // Move CharacterController
            Vector3 moveDirection = new Vector3(newX - transform.position.x, verticalVelocity * Time.deltaTime, forwardSpeed * Time.deltaTime);
            controller.Move(moveDirection);
        }

        private IEnumerator PerformSlide()
        {
            isSliding = true;

            // Adjust collider
            controller.height = slideColliderHeight;
            controller.center = slideColliderCenter;

            // Visual squash effect
            transform.localScale = new Vector3(originalScale.x * 1.2f, originalScale.y * 0.5f, originalScale.z * 1.4f);

            // Fast descend if in air
            if (!controller.isGrounded)
            {
                verticalVelocity = -gravity;
            }

            yield return new WaitForSeconds(slideDuration);

            // Restore collider & scale
            controller.height = originalColliderHeight;
            controller.center = originalColliderCenter;
            transform.localScale = originalScale;

            isSliding = false;
        }

        private void HandleShieldChanged(bool active)
        {
            if (shieldVisual != null)
            {
                shieldVisual.SetActive(active);
            }
        }

        public void SetShieldVisual(GameObject visual)
        {
            shieldVisual = visual;
        }

        public void SetMeshRenderer(MeshRenderer renderer)
        {
            playerRenderer = renderer;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
                return;

            if (hit.gameObject.CompareTag("Obstacle"))
            {
                HandleObstacleHit(hit.gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (GameManager.Instance == null || GameManager.Instance.CurrentState != GameState.Playing)
                return;

            if (other.CompareTag("Obstacle"))
            {
                HandleObstacleHit(other.gameObject);
            }
        }

        private void HandleObstacleHit(GameObject obstacle)
        {
            // Check if player has active shield
            if (GameManager.Instance.ConsumeShield())
            {
                SoundManager.Instance?.PlayShieldBreakSound();
                CameraFollow.TriggerShake(0.3f, 0.4f);
                // Disable hit obstacle to prevent repeated collisions
                obstacle.SetActive(false);
                StartCoroutine(FlashDamage());
            }
            else
            {
                SoundManager.Instance?.PlayGameOverSound();
                CameraFollow.TriggerShake(0.5f, 0.7f);
                GameManager.Instance.GameOver();
            }
        }

        private IEnumerator FlashDamage()
        {
            if (playerRenderer != null)
            {
                Color originalColor = playerRenderer.material.color;
                for (int i = 0; i < 3; i++)
                {
                    playerRenderer.material.color = Color.red;
                    yield return new WaitForSeconds(0.08f);
                    playerRenderer.material.color = originalColor;
                    yield return new WaitForSeconds(0.08f);
                }
            }
        }
    }
}
