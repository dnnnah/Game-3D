using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody))]
public sealed class TowerRotationController : MonoBehaviour
{
    [SerializeField, Min(0f)] private float degreesPerPixel = 0.45f;
    [FormerlySerializedAs("maximumDegreesPerFrame")]
    [SerializeField, Min(0f), Tooltip("Rotación máxima aplicada en cada paso de física.")]
    private float maximumDegreesPerStep = 90f;
    [SerializeField, Min(0.001f), Tooltip("Suavizado de la respuesta de la torre al arrastre táctil.")]
    private float rotationSmoothTime = 0.055f;

    private MobileInputController input;
    private Rigidbody body;
    private bool rotationEnabled;
    private Quaternion startingRotation;
    private float targetYaw;
    private float currentYaw;
    private float yawVelocity;

    private void Awake()
    {
        EnsurePhysicsSetup();
        startingRotation = body.rotation;
        ResetYawState();
    }

    public void Configure(MobileInputController inputController)
    {
        EnsurePhysicsSetup();
        input = inputController;
        startingRotation = body.rotation;
        ResetYawState();
    }

    public void EnsurePhysicsSetup()
    {
        body = GetComponent<Rigidbody>();
        if (body == null)
            body = gameObject.AddComponent<Rigidbody>();

        body.mass = 1f;
        body.useGravity = false;
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        body.constraints = RigidbodyConstraints.FreezePositionX
            | RigidbodyConstraints.FreezePositionY
            | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ;
        body.detectCollisions = true;
    }

    public void SetRotationEnabled(bool enabled)
    {
        rotationEnabled = enabled;
        if (!enabled)
            input?.Clear();
    }

    public void ResetRotation()
    {
        EnsurePhysicsSetup();
        body.rotation = startingRotation;
        ResetYawState();
        input?.Clear();
        Physics.SyncTransforms();
    }

    private void FixedUpdate()
    {
        if (input == null)
            return;

        float delta = input.ConsumeRotationDelta();
        if (!rotationEnabled)
            return;

        float degrees = Mathf.Clamp(delta * degreesPerPixel, -maximumDegreesPerStep, maximumDegreesPerStep);
        targetYaw -= degrees;
        float maximumSpeed = maximumDegreesPerStep / Mathf.Max(Time.fixedDeltaTime, 0.001f);
        currentYaw = Mathf.SmoothDamp(currentYaw, targetYaw, ref yawVelocity,
            rotationSmoothTime, maximumSpeed, Time.fixedDeltaTime);
        body.MoveRotation(startingRotation * Quaternion.AngleAxis(currentYaw, Vector3.up));
    }

    private void ResetYawState()
    {
        targetYaw = 0f;
        currentYaw = 0f;
        yawVelocity = 0f;
    }
}
