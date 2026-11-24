using UnityEngine;

namespace CyberEscape.Player
{
    public class CameraFollow : MonoBehaviour
    {
        [Header("Target & Offsets")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 3.5f, -6f);
        [SerializeField] private float followSpeed = 10f;

        private static CameraFollow instance;
        private float shakeDuration = 0f;
        private float shakeMagnitude = 0.3f;

        private void Awake()
        {
            instance = this;
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private void LateUpdate()
        {
            if (target == null) return;

            // Target position keeps camera offset relative to player
            Vector3 targetPosition = new Vector3(
                Mathf.Lerp(transform.position.x, target.position.x + offset.x, Time.deltaTime * followSpeed),
                target.position.y + offset.y,
                target.position.z + offset.z
            );

            // Apply camera shake if active
            if (shakeDuration > 0f)
            {
                targetPosition += Random.insideUnitSphere * shakeMagnitude;
                shakeDuration -= Time.deltaTime;
            }

            transform.position = targetPosition;
            transform.LookAt(target.position + Vector3.up * 1.5f);
        }

        public static void TriggerShake(float duration = 0.3f, float magnitude = 0.4f)
        {
            if (instance != null)
            {
                instance.shakeDuration = duration;
                instance.shakeMagnitude = magnitude;
            }
        }
    }
}
