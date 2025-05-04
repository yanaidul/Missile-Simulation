using UnityEngine;

namespace MissileSimulation.Plane
{
    public class AutoPlaneMovement : MonoBehaviour
    {
        public enum MovementMode
        {
            Circular,
            LeftRight
        }

        [Header("Movement Settings")]
        public MovementMode movementMode = MovementMode.Circular;
        public float maxSpeed = 30f;
        public float acceleration = 5f;
        public float deceleration = 8f;
        private float currentSpeed = 0f;

        [Header("Flight Physics")]
        public float pitchSpeed = 2f;
        public float rollSpeed = 3f;
        public float yawSpeed = 1f;
        public float bankAngleLimit = 45f;
        public float stability = 10f;

        [Header("Circular Movement")]
        public Vector3 orbitCenter = Vector3.zero;
        public float orbitRadius = 50f;
        private float currentOrbitAngle;

        [Header("Left-Right Movement")]
        public float travelDistance = 100f;
        public float endPauseDuration = 1f;
        private Vector3 movementStartPoint;
        private Vector3 currentTarget;
        private bool movingRight = true;
        private float pauseTimer = 0f;

        private Rigidbody rb;
        private Vector3 lastPosition;

        void Start()
        {
            rb = GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.useGravity = false;
                rb.linearDamping = 0.1f;
                rb.angularDamping = 0.5f;
            }

            InitializeMovement();
            lastPosition = transform.position;
        }

        void InitializeMovement()
        {
            int random = Random.Range(0, 2);

            switch(random)
            {
                case 0:
                    movementMode = MovementMode.Circular;
                    break;
                case 1:
                    movementMode = MovementMode.LeftRight; 
                    break;
            }

            switch (movementMode)
            {
                case MovementMode.Circular:
                    currentOrbitAngle = 0f;
                    break;
                case MovementMode.LeftRight:
                    movementStartPoint = transform.position;
                    currentTarget = movementStartPoint + Vector3.right * travelDistance;
                    break;
            }
        }

        void Update()
        {
            // Calculate actual movement direction based on position change
            Vector3 actualVelocity = (transform.position - lastPosition) / Time.deltaTime;
            lastPosition = transform.position;

            if (actualVelocity.magnitude > 0.1f)
            {
                ApplyAerodynamicRotation(actualVelocity.normalized);
            }

            // Gradually adjust speed
            float targetSpeed = maxSpeed;
            if (pauseTimer > 0) targetSpeed = 0f;

            if (currentSpeed < targetSpeed)
            {
                currentSpeed = Mathf.Min(currentSpeed + acceleration * Time.deltaTime, targetSpeed);
            }
            else if (currentSpeed > targetSpeed)
            {
                currentSpeed = Mathf.Max(currentSpeed - deceleration * Time.deltaTime, targetSpeed);
            }
        }

        void FixedUpdate()
        {
            switch (movementMode)
            {
                case MovementMode.Circular:
                    UpdateCircularMovement();
                    break;
                case MovementMode.LeftRight:
                    UpdateLeftRightMovement();
                    break;
            }
        }

        void UpdateCircularMovement()
        {
            if (pauseTimer > 0)
            {
                pauseTimer -= Time.deltaTime;
                return;
            }

            // Update position
            currentOrbitAngle += (currentSpeed / orbitRadius) * Time.fixedDeltaTime;
            Vector3 targetPosition = orbitCenter + new Vector3(
                Mathf.Sin(currentOrbitAngle) * orbitRadius,
                transform.position.y, // Maintain current altitude
                Mathf.Cos(currentOrbitAngle) * orbitRadius
            );

            // Move using physics
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.linearVelocity = direction * currentSpeed;
        }

        void UpdateLeftRightMovement()
        {
            if (pauseTimer > 0)
            {
                pauseTimer -= Time.deltaTime;
                return;
            }

            // Move toward target
            Vector3 direction = (currentTarget - transform.position).normalized;
            rb.linearVelocity = direction * currentSpeed;

            // Check if reached target
            if (Vector3.Distance(transform.position, currentTarget) < 5f)
            {
                movingRight = !movingRight;
                currentTarget = movementStartPoint +
                    (movingRight ? Vector3.right : Vector3.left) * travelDistance;
                pauseTimer = endPauseDuration;
            }
        }

        void ApplyAerodynamicRotation(Vector3 movementDirection)
        {
            // Calculate target rotation based on movement direction
            Quaternion targetRotation = Quaternion.LookRotation(movementDirection);

            // Add banking effect when turning
            float turnAmount = Vector3.Dot(transform.right, movementDirection);
            float bankAngle = Mathf.Clamp(turnAmount * bankAngleLimit, -bankAngleLimit, bankAngleLimit);
            targetRotation *= Quaternion.Euler(0, 0, -bankAngle);

            // Smoothly rotate towards target
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                stability * Time.deltaTime
            );

            // Add subtle pitch variations for realism
            float pitchVariation = Mathf.PerlinNoise(Time.time * 0.5f, 0) * 2f - 1f;
            transform.Rotate(pitchVariation * pitchSpeed * Time.deltaTime, 0, 0);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;

            if (movementMode == MovementMode.Circular)
            {
                Gizmos.DrawWireSphere(orbitCenter, orbitRadius);
                Gizmos.DrawLine(orbitCenter, transform.position);
            }
            else
            {
                Vector3 start = Application.isPlaying ? movementStartPoint : transform.position;
                Gizmos.DrawLine(start - Vector3.right * travelDistance,
                               start + Vector3.right * travelDistance);
                Gizmos.DrawSphere(start - Vector3.right * travelDistance, 0.5f);
                Gizmos.DrawSphere(start + Vector3.right * travelDistance, 0.5f);
            }
        }

        public void SwitchToCircular()
        {
            movementMode = MovementMode.Circular;
            InitializeMovement();
        }

        public void SwitchToLeftRight()
        {
            movementMode = MovementMode.LeftRight;
            InitializeMovement();
        }
    }
}