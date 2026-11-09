using System;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class HazardSector : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private MaterialPropertyBlock propertyBlock;
    private MeshCollider sectorCollider;
    private Renderer sectorRenderer;
    private Action hazardCallback;
    private bool isDangerous;
    private bool reportedThisContact;

    public void Configure(bool dangerous, Color dangerColor, Action onHazardContact)
    {
        CacheComponents();

        isDangerous = dangerous;
        hazardCallback = onHazardContact;
        reportedThisContact = false;

        if (sectorCollider != null)
        {
            sectorCollider.enabled = true;
            sectorCollider.isTrigger = isDangerous;
        }

        if (sectorRenderer != null)
        {
            sectorRenderer.enabled = true;
            if (!isDangerous)
            {
                sectorRenderer.SetPropertyBlock(null);
            }
            else
            {
                sectorRenderer.GetPropertyBlock(propertyBlock);
                int colorProperty = sectorRenderer.sharedMaterial != null
                    && sectorRenderer.sharedMaterial.HasProperty(BaseColorId)
                    ? BaseColorId
                    : ColorId;
                propertyBlock.SetColor(colorProperty, dangerColor);
                sectorRenderer.SetPropertyBlock(propertyBlock);
            }
        }

        enabled = isDangerous;
    }

    private void OnTriggerEnter(Collider other) => TryReportHazard(other);
    private void OnTriggerStay(Collider other) => TryReportHazard(other);

    private void Awake() => CacheComponents();

    private void TryReportHazard(Collider other)
    {
        if (!isDangerous || reportedThisContact)
            return;

        if (other.GetComponentInParent<PlayerMovement>() == null)
            return;

        reportedThisContact = true;
        isDangerous = false;
        Vector3 impactPoint = sectorCollider != null
            ? sectorCollider.ClosestPoint(other.transform.position)
            : transform.position;
        if (sectorCollider != null)
            sectorCollider.enabled = false;
        if (sectorRenderer != null)
            sectorRenderer.enabled = false;
        HelixParticleEffects.Burst(impactPoint, new Color(1f, 0.08f, 0.02f, 1f), 20, 1.5f, 3.2f, 0.14f);
        hazardCallback?.Invoke();
    }

    private void CacheComponents()
    {
        if (sectorCollider == null)
            sectorCollider = GetComponent<MeshCollider>();
        if (sectorRenderer == null)
            sectorRenderer = GetComponentInChildren<Renderer>();
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();
    }
}
