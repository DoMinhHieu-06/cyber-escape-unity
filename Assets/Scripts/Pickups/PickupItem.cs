using UnityEngine;
using CyberEscape.Core;
using CyberEscape.Audio;

namespace CyberEscape.Pickups
{
    public class PickupItem : MonoBehaviour
    {
        [SerializeField] private PickupType type = PickupType.Coin;
        [SerializeField] private float rotationSpeed = 120f;
        [SerializeField] private float magnetAttractSpeed = 18f;
        [SerializeField] private float magnetDetectionRadius = 12f;

        private Transform playerTransform;

        public void Initialize(PickupType pickupType)
        {
            type = pickupType;
        }

        private void Start()
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        private void Update()
        {
            // Continuous idle rotation
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);

            // Magnet attraction logic for coins
            if (type == PickupType.Coin && GameManager.Instance != null && GameManager.Instance.IsMagnetActive)
            {
                if (playerTransform != null)
                {
                    float distance = Vector3.Distance(transform.position, playerTransform.position);
                    if (distance <= magnetDetectionRadius)
                    {
                        transform.position = Vector3.MoveTowards(
                            transform.position,
                            playerTransform.position + Vector3.up * 1f,
                            magnetAttractSpeed * Time.deltaTime
                        );
                    }
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Collect();
            }
        }

        public void Collect()
        {
            if (GameManager.Instance == null) return;

            switch (type)
            {
                case PickupType.Coin:
                    GameManager.Instance.AddCoins(1);
                    SoundManager.Instance?.PlayCoinSound();
                    break;

                case PickupType.Shield:
                    GameManager.Instance.ActivateShield();
                    SoundManager.Instance?.PlayPowerUpSound();
                    break;

                case PickupType.Magnet:
                    GameManager.Instance.ActivateMagnet(8f);
                    SoundManager.Instance?.PlayPowerUpSound();
                    break;

                case PickupType.SlowMotion:
                    GameManager.Instance.ActivateSlowMotion(5f);
                    SoundManager.Instance?.PlayPowerUpSound();
                    break;
            }

            Destroy(gameObject);
        }
    }
}
