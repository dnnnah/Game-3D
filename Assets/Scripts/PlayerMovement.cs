using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float lateralSpeed = 6f;
    [SerializeField, Min(0f)] private float forwardSpeed = 7f;
    [SerializeField, Min(0f)] private float jumpImpulse = 6f;

    private Rigidbody body;
    private MobileInputController input;
    private float automaticForwardSpeed;
    private bool movementEnabled;
    private bool groundedContact;
    private bool isGrounded;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.mass = 1f;
        body.useGravity = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        body.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void Configure(MobileInputController inputController, bool enabledOnStart)
    {
        input = inputController;
        movementEnabled = enabledOnStart;
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled;
        if (!enabled && body != null)
        {
            Vector3 velocity = body.linearVelocity;
            velocity.x = 0f;
            velocity.z = 0f;
            body.linearVelocity = velocity;
        }
    }

    public void SetAutomaticForwardSpeed(float speed)
    {
        automaticForwardSpeed = Mathf.Max(0f, speed);
    }

    private void FixedUpdate()
    {
        isGrounded = groundedContact;
        groundedContact = false;

        if (input == null)
            return;

        bool jumpRequested = input.ConsumeJumpRequest();
        if (!movementEnabled)
            return;

        Vector2 movement = Vector2.ClampMagnitude(input.Movement, 1f);
        Vector3 velocity = body.linearVelocity;
        velocity.x = movement.x * lateralSpeed;
        velocity.z = automaticForwardSpeed + movement.y * forwardSpeed;
        body.linearVelocity = velocity;

        if (jumpRequested && isGrounded)
        {
            velocity.y = Mathf.Max(velocity.y, 0f);
            body.linearVelocity = velocity;
            body.AddForce(Vector3.up * jumpImpulse, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter(Collision collision) => UpdateGroundContact(collision);
    private void OnCollisionStay(Collision collision) => UpdateGroundContact(collision);

    private void UpdateGroundContact(Collision collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal.y >= 0.5f)
            {
                groundedContact = true;
                return;
            }
        }
    }
}
