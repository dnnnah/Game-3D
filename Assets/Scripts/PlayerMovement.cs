using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float initialDropSpeed = 1.5f;
    [SerializeField, Min(0f), Tooltip("Velocidad vertical fija que recibe la bola al aterrizar en un sector seguro.")]
    private float bounceSpeed = 5.5f;
    [SerializeField, Min(0f)] private float maximumFallSpeed = 12f;

    private Rigidbody body;
    private bool movementEnabled;
    private float verticalSpeedBeforePhysicsStep;

    public float VerticalSpeed => body != null ? body.linearVelocity.y : 0f;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.mass = 1f;
        body.useGravity = true;
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.constraints = RigidbodyConstraints.FreezePositionX
            | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezeRotation;
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        if (!enabled && !body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        body.isKinematic = !enabled;
        if (enabled)
        {
            body.linearVelocity = Vector3.down * initialDropSpeed;
            verticalSpeedBeforePhysicsStep = -initialDropSpeed;
        }
        else
        {
            verticalSpeedBeforePhysicsStep = 0f;
        }
    }

    public void SetMaximumFallSpeed(float speed)
    {
        maximumFallSpeed = Mathf.Max(0f, speed);
    }

    public void ResetBall(Vector3 startPosition)
    {
        movementEnabled = false;
        if (!body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        body.isKinematic = true;
        body.position = startPosition;
        verticalSpeedBeforePhysicsStep = 0f;
    }

    private void FixedUpdate()
    {
        if (!movementEnabled)
            return;

        Vector3 velocity = body.linearVelocity;
        velocity.x = 0f;
        velocity.z = 0f;
        velocity.y = Mathf.Max(velocity.y, -maximumFallSpeed);
        body.linearVelocity = velocity;
        verticalSpeedBeforePhysicsStep = velocity.y;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!movementEnabled || verticalSpeedBeforePhysicsStep >= -0.05f)
            return;

        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y > 0.55f)
            {
                Vector3 velocity = body.linearVelocity;
                velocity.x = 0f;
                velocity.y = bounceSpeed;
                velocity.z = 0f;
                body.linearVelocity = velocity;
                return;
            }
        }
    }
}
