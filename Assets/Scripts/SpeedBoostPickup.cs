using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SphereCollider))]
public sealed class SpeedBoostPickup : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private MaterialPropertyBlock propertyBlock;
    private SphereCollider pickupCollider;
    private Renderer pickupRenderer;
    private float speedMultiplier;
    private float boostDuration;
    private bool available;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        pickupCollider = GetComponent<SphereCollider>();
        pickupRenderer = GetComponent<Renderer>();
        pickupCollider.isTrigger = true;
        pickupCollider.radius = 0.6f;
    }

    public void Configure(Vector3 localPosition, float visualDiameter, float multiplier, float duration,
        Color color, bool isAvailable)
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
        if (pickupCollider == null)
            pickupCollider = GetComponent<SphereCollider>();
        if (pickupRenderer == null)
            pickupRenderer = GetComponent<Renderer>();

        transform.localPosition = localPosition;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one * Mathf.Max(0.1f, visualDiameter);
        speedMultiplier = Mathf.Clamp(multiplier, 1f, 3f);
        boostDuration = Mathf.Clamp(duration, 0.1f, 3f);

        if (pickupCollider != null)
        {
            pickupCollider.isTrigger = true;
            pickupCollider.radius = 0.6f;
        }

        if (pickupRenderer != null)
        {
            pickupRenderer.GetPropertyBlock(propertyBlock);
            int colorProperty = pickupRenderer.sharedMaterial != null
                && pickupRenderer.sharedMaterial.HasProperty(BaseColorId)
                ? BaseColorId
                : ColorId;
            propertyBlock.SetColor(colorProperty, color);
            pickupRenderer.SetPropertyBlock(propertyBlock);
        }

        SetAvailable(isAvailable);
    }

    public void SetAvailable(bool isAvailable)
    {
        available = isAvailable;
        if (gameObject.activeSelf != isAvailable)
            gameObject.SetActive(isAvailable);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!available)
            return;

        PlayerMovement movement = other.GetComponentInParent<PlayerMovement>();
        if (movement == null || !movement.ActivateSpeedBoost(speedMultiplier, boostDuration))
            return;

        SetAvailable(false);
        HelixParticleEffects.Burst(transform.position, new Color(0.12f, 0.95f, 1f, 1f), 12, 1f, 2f, 0.1f);
    }

    private void Update()
    {
        if (available)
            transform.Rotate(0f, 100f * Time.deltaTime, 0f, Space.Self);
    }
}
