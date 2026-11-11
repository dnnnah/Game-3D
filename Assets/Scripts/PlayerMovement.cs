using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public sealed class PlayerMovement : MonoBehaviour
{
    [SerializeField, Min(0f)] private float initialDropSpeed = 1.5f;
    [SerializeField, Min(0f), Tooltip("Velocidad vertical fija que recibe la bola al aterrizar en un sector seguro.")]
    private float bounceSpeed = 5.5f;
    [SerializeField, Min(0f)] private float maximumFallSpeed = 12f;

    [Header("Potenciador de velocidad")]
    [SerializeField, Range(1f, 3f)] private float maximumBoostMultiplier = 2.5f;
    [SerializeField, Min(0f)] private float boostTrailTime = 0.35f;
    [SerializeField, Min(0f)] private float boostTrailWidth = 0.42f;

    private Rigidbody body;
    private SphereCollider ballCollider;
    private Renderer ballRenderer;
    private TrailRenderer boostTrail;
    private Material boostTrailMaterial;
    private bool movementEnabled;
    private bool isDead;
    private float verticalSpeedBeforePhysicsStep;
    private float speedBoostTimeRemaining;
    private float speedBoostMultiplier = 1f;

    public float VerticalSpeed => body != null ? body.linearVelocity.y : 0f;
    public bool IsSpeedBoostActive => speedBoostTimeRemaining > 0f;
    public float SpeedBoostTimeRemaining => Mathf.Max(0f, speedBoostTimeRemaining);

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        ballCollider = GetComponent<SphereCollider>();
        ballRenderer = GetComponent<Renderer>();
        body.mass = 1f;
        body.useGravity = true;
        body.isKinematic = true;
        body.interpolation = RigidbodyInterpolation.Interpolate;
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        body.constraints = RigidbodyConstraints.FreezePositionX
            | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezeRotation;
        ConfigureBoostTrail();
    }

    public void SetMovementEnabled(bool enabled)
    {
        movementEnabled = enabled && !isDead;
        if (!movementEnabled)
            DeactivateSpeedBoost();

        if (!movementEnabled && !body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        body.isKinematic = !movementEnabled;
        if (movementEnabled)
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

    public bool ActivateSpeedBoost(float multiplier, float duration)
    {
        if (!movementEnabled || isDead || body == null || body.isKinematic || IsSpeedBoostActive)
            return false;

        speedBoostMultiplier = Mathf.Clamp(multiplier, 1f, maximumBoostMultiplier);
        speedBoostTimeRemaining = Mathf.Clamp(duration, 0.1f, 3f);
        if (boostTrail != null)
        {
            boostTrail.Clear();
            boostTrail.enabled = true;
            boostTrail.emitting = true;
        }

        return true;
    }

    public void ResetBall(Vector3 startPosition)
    {
        isDead = false;
        movementEnabled = false;
        if (!body.isKinematic)
        {
            body.linearVelocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
        }

        body.isKinematic = true;
        body.position = startPosition;
        if (ballCollider != null)
            ballCollider.enabled = true;
        if (ballRenderer != null)
            ballRenderer.enabled = true;
        verticalSpeedBeforePhysicsStep = 0f;
        DeactivateSpeedBoost();
    }

    public void Die(Color effectColor)
    {
        if (isDead)
            return;

        isDead = true;
        SetMovementEnabled(false);
        HelixParticleEffects.Burst(transform.position, effectColor, 24, 2.2f, 4f, 0.16f);
        if (ballCollider != null)
            ballCollider.enabled = false;
        if (ballRenderer != null)
            ballRenderer.enabled = false;
    }

    private void FixedUpdate()
    {
        if (!movementEnabled)
            return;

        bool boosted = IsSpeedBoostActive;
        if (boosted)
            body.AddForce(Physics.gravity * (speedBoostMultiplier - 1f), ForceMode.Acceleration);

        Vector3 velocity = body.linearVelocity;
        velocity.x = 0f;
        velocity.z = 0f;
        float fallSpeedLimit = maximumFallSpeed * (boosted ? speedBoostMultiplier : 1f);
        velocity.y = Mathf.Max(velocity.y, -fallSpeedLimit);
        body.linearVelocity = velocity;
        verticalSpeedBeforePhysicsStep = velocity.y;

        if (boosted)
        {
            speedBoostTimeRemaining = Mathf.Max(0f, speedBoostTimeRemaining - Time.fixedDeltaTime);
            if (!IsSpeedBoostActive)
                DeactivateSpeedBoost();
        }
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

    private void ConfigureBoostTrail()
    {
        boostTrail = GetComponent<TrailRenderer>();
        if (boostTrail == null)
            boostTrail = gameObject.AddComponent<TrailRenderer>();

        boostTrail.time = Mathf.Clamp(boostTrailTime, 0.05f, 0.75f);
        boostTrail.minVertexDistance = 0.04f;
        boostTrail.widthMultiplier = boostTrailWidth;
        boostTrail.widthCurve = new AnimationCurve(new Keyframe(0f, 0.75f), new Keyframe(1f, 0f));
        boostTrail.numCapVertices = 4;
        boostTrail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        boostTrail.receiveShadows = false;
        boostTrail.colorGradient = CreateBoostGradient();

        Shader trailShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (trailShader == null)
            trailShader = Shader.Find("Sprites/Default");
        if (trailShader != null)
        {
            boostTrailMaterial = new Material(trailShader)
            {
                name = "Runtime Speed Boost Trail"
            };
            if (boostTrailMaterial.HasProperty("_BaseColor"))
                boostTrailMaterial.SetColor("_BaseColor", new Color(0.12f, 0.95f, 1f, 1f));
            else if (boostTrailMaterial.HasProperty("_Color"))
                boostTrailMaterial.SetColor("_Color", new Color(0.12f, 0.95f, 1f, 1f));
            boostTrail.sharedMaterial = boostTrailMaterial;
        }

        boostTrail.emitting = false;
        boostTrail.enabled = false;
    }

    private static Gradient CreateBoostGradient()
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(0.12f, 0.95f, 1f, 1f), 0f),
                new GradientColorKey(new Color(0.12f, 0.95f, 1f, 1f), 1f)
            },
            new[]
            {
                new GradientAlphaKey(0.9f, 0f),
                new GradientAlphaKey(0f, 1f)
            });
        return gradient;
    }

    private void DeactivateSpeedBoost()
    {
        speedBoostTimeRemaining = 0f;
        speedBoostMultiplier = 1f;
        if (boostTrail == null)
            return;

        boostTrail.emitting = false;
        boostTrail.Clear();
        boostTrail.enabled = false;
    }

    private void OnDestroy()
    {
        if (boostTrailMaterial == null)
            return;

        if (Application.isPlaying)
            Destroy(boostTrailMaterial);
        else
            DestroyImmediate(boostTrailMaterial);
    }
}
